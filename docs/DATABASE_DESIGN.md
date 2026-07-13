# Trust Management System — PostgreSQL Database Design (Reviewed)

Planning only — no SQL scripts, no EF models, no backend/frontend code. `snake_case` naming throughout.
This revision follows a technical review of the first draft. See **Review Summary** for what changed and why; the schema sections below are the final, implementation-ready result.

---

## Review Summary

**Missing tables** — two gaps found and added:
- No table stored admin refresh/password-reset tokens → added `admin_auth_tokens`.
- No generic history of who-did-what-when across modules → added `audit_logs`.

**Missing columns** — found and fixed:
- `volunteer_requests` had no place to record the reviewer's internal note, even though the architecture doc's own form spec called for one → added `review_note`.
- Public-facing submission forms (`volunteer_requests`, `contact_messages`) had no abuse/spam signal → added `ip_address`, and `is_spam` on `contact_messages`.
- `admin_users` had no brute-force protection or MFA hooks → added `failed_login_attempts`, `locked_until`, `mfa_enabled`, `mfa_secret`, `last_login_ip`, `password_changed_at`.
- `gallery_media` had no file metadata → added `mime_type`, `file_size_bytes`.

**Incorrect relationships** — one classification error corrected:
- `members` and `volunteers` were originally grouped with "public-submitted" data (no admin attribution). That's wrong — both are created and maintained *by admin staff*, not submitted by the public. Moved them into the admin-authored bucket and added `created_by_admin_id` / `updated_by_admin_id`.
- `admin_users` gained a self-referential `created_by_admin_id` (which admin provisioned this account) — the original draft had no way to answer "who added this admin?"

**Naming consistency** — one inconsistency fixed:
- `contact_messages.name` renamed to `full_name` to match `members.full_name` / `volunteers.full_name` / `volunteer_requests.full_name`. (`testimonials.author_name` is kept distinct on purpose — it names the quote's author, not a system user.)
- Confirmed the `created_by_admin_id` / `updated_by_admin_id` / `reviewed_by_admin_id` naming pattern is now applied uniformly; the original doc's summary text mentioned `updated_by_admin_id` on tables that didn't actually have it — that drift is fixed by actually adding the column everywhere the label promised it.

**Normalization issues**:
- `events.category` remains free text (not a lookup table) — deliberate, see **Deferred**.
- `events.is_upcoming` was a stored, manually-set boolean derived from `event_date` — this can silently go stale (an event stays "upcoming" after its date passes unless something remembers to flip it). **Removed as a stored column**; compute it at query time as `event_date >= now()` instead. The existing index on `event_date` already makes that cheap.
- `settings` as a key/value table is intentionally denormalized for schema flexibility — flagged explicitly rather than left implicit, see security note below.

**Scalability**: `BIGSERIAL` surrogate keys are sufficient at this org's scale; no partitioning or sharding needed now. `audit_logs` is the one table with unbounded growth — noted for future archiving, not solved today (see **Deferred**).

**Security considerations**:
- **`settings` can accidentally become a secrets store** (e.g. an admin pastes an SMTP password into a "setting"). Added `is_secret BOOLEAN`, and the explicit rule: real secrets (SMTP credentials, API keys) belong in environment variables / a secrets manager, never in this table, secret or not.
- Sequential IDs are fine for publicly-displayed content (`events`, `gallery_media`, `announcements`, `testimonials` — nothing sensitive to enumerate); access control for PII-bearing tables (`members`, `volunteers`, `contact_messages`, `volunteer_requests`) must come from admin-auth middleware, not from ID obscurity.
- Added brute-force/MFA columns to `admin_users` (above) and a dedicated `admin_auth_tokens` table so refresh/reset tokens are hashed, expiring, and revocable server-side rather than stateless-forever JWTs.
- Soft-deleting PII (see below) does **not** by itself satisfy "right to erasure" requests — a genuine hard-delete/purge path is still required for compliance; soft delete is for accidental-deletion recovery, not privacy compliance.

**Audit fields**: standardized `created_at` everywhere, `updated_at` on every mutable table, and `created_by_admin_id`/`updated_by_admin_id` on every admin-authored table (see relationship fix above). `audit_logs` adds a cross-cutting action trail on top of these per-row fields.

**Soft delete support**: the original draft had none. Added `deleted_at TIMESTAMPTZ NULL` to every table representing a recoverable business record (`admin_users`, `members`, `volunteer_requests`, `volunteers`, `events`, `gallery_media`, `contact_messages`, `announcements`, `testimonials`). Not added to `settings` (config rows are upserted/overwritten, not "recovered") or `admin_auth_tokens`/`audit_logs` (tokens and log rows are never soft-deleted, only expired or immutable). **Consequence caught during review**: every `UNIQUE` constraint on a soft-deletable table (`admin_users.email`, `members.email`, `volunteers.email`, `events.slug`) must become a **partial unique index `WHERE deleted_at IS NULL`**, otherwise a soft-deleted row permanently blocks reuse of that email/slug.

**Performance concerns / index recommendations**: consolidated into one rule instead of repeating it ten times — every soft-deletable table's indexes should be partial (`WHERE deleted_at IS NULL`), since that's the filter nearly every query will apply. Added composite indexes for the actual admin list-view query shapes (status/published + sort column together), not just single-column indexes.

---

## Conventions (revised)

- Surrogate key: `id BIGSERIAL PRIMARY KEY` on every table (swap for `UUID DEFAULT gen_random_uuid()` if non-sequential public IDs are later required).
- `created_at TIMESTAMPTZ NOT NULL DEFAULT now()` on every table; `updated_at TIMESTAMPTZ NOT NULL DEFAULT now()` on every mutable table.
- `deleted_at TIMESTAMPTZ NULL` on every recoverable business table (listed above). All indexes and `UNIQUE` constraints on those tables are **partial**, scoped to `WHERE deleted_at IS NULL` — stated once here, not repeated per table below.
- Status/category fields use `VARCHAR` + `CHECK (... IN (...))` rather than native `ENUM`, so adding a new value is a data change, not a type migration.
- Two different "state" shapes are used on purpose: a `status` enum-like column where a field has more than two meaningful states or a workflow (`Pending/Approved/Rejected`, `Active/Inactive`), and a plain `is_x BOOLEAN` where the field is genuinely binary (`is_published`, `is_read`). Not a naming bug — different states have different shapes.
- Admin-authored/managed tables (`members`, `volunteers`, `events`, `gallery_media`, `announcements`, `testimonials`) carry both `created_by_admin_id` and `updated_by_admin_id`. Publicly-submitted tables (`volunteer_requests`, `contact_messages`) carry no `created_by` (the public has no admin identity) but do carry a reviewer/actor field where a workflow exists (`reviewed_by_admin_id` on `volunteer_requests`).
- Every `created_by_admin_id` / `updated_by_admin_id` / `reviewed_by_admin_id` FK uses `ON DELETE SET NULL` — removing a staff account never deletes or orphans their historical work, only un-attributes it.
- Email columns get a light format `CHECK (email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$')`. Phone columns deliberately do **not** get a `CHECK` regex — international phone formats vary too much for a reliable DB-level pattern; validate phone format at the application layer instead.

---

## 1. `admin_users`
**Purpose**: authentication/authorization for staff operating the admin panel.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| full_name | VARCHAR(150) | NOT NULL |
| email | VARCHAR(255) | NOT NULL |
| password_hash | VARCHAR(255) | NOT NULL — bcrypt/argon2id output, never reversible encryption |
| role | VARCHAR(30) | NOT NULL, DEFAULT 'Editor' |
| is_active | BOOLEAN | NOT NULL, DEFAULT TRUE — temporary suspend, distinct from `deleted_at` |
| mfa_enabled | BOOLEAN | NOT NULL, DEFAULT FALSE |
| mfa_secret | VARCHAR(255) | NULL |
| failed_login_attempts | SMALLINT | NOT NULL, DEFAULT 0 |
| locked_until | TIMESTAMPTZ | NULL |
| password_changed_at | TIMESTAMPTZ | NULL |
| last_login_at | TIMESTAMPTZ | NULL |
| last_login_ip | INET | NULL |
| created_by_admin_id | BIGINT | NULL, FK → admin_users.id (self-referential: who provisioned this account) |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at / updated_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: created_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: partial UNIQUE(email) WHERE deleted_at IS NULL; CHECK role IN ('SuperAdmin','Admin','Editor'); email format CHECK
- **Indexes**: partial unique index on email; index on is_active; index on deleted_at
- **Relationships**: root identity table. Referenced by `created_by_admin_id`/`updated_by_admin_id`/`reviewed_by_admin_id` across nearly every other table, and by `admin_auth_tokens`/`audit_logs`.

## 2. `admin_auth_tokens` — new
**Purpose**: server-side-revocable refresh and password-reset tokens for admin login, so sessions can be invalidated without waiting for a stateless JWT to expire.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| admin_id | BIGINT | NOT NULL, FK → admin_users.id |
| token_type | VARCHAR(20) | NOT NULL |
| token_hash | VARCHAR(255) | NOT NULL — store a hash, never the raw token |
| expires_at | TIMESTAMPTZ | NOT NULL |
| used_at | TIMESTAMPTZ | NULL |
| revoked_at | TIMESTAMPTZ | NULL |
| created_ip | INET | NULL |
| created_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: admin_id → admin_users.id, **ON DELETE CASCADE** (unlike content tables, a token is meaningless without its account — this is the one FK in the schema that should cascade)
- **Constraints**: CHECK token_type IN ('RefreshToken','PasswordReset')
- **Indexes**: index on admin_id; unique index on token_hash; index on expires_at (for a scheduled cleanup job purging expired rows)
- **Relationships**: many tokens per admin (1 : N).

## 3. `members`
**Purpose**: roster of registered/enrolled trust members, added and maintained by admin staff.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| full_name | VARCHAR(150) | NOT NULL |
| email | VARCHAR(255) | NOT NULL |
| phone | VARCHAR(20) | NOT NULL |
| address | TEXT | NULL |
| membership_type | VARCHAR(30) | NOT NULL |
| status | VARCHAR(20) | NOT NULL, DEFAULT 'Active' |
| joined_date | DATE | NOT NULL, DEFAULT CURRENT_DATE |
| photo_url | VARCHAR(500) | NULL |
| created_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| updated_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at / updated_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: created_by_admin_id, updated_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: partial UNIQUE(email) WHERE deleted_at IS NULL; CHECK membership_type IN ('General','Lifetime','Honorary','Patron'); CHECK status IN ('Active','Inactive'); email format CHECK
- **Indexes**: partial index on status; partial index on membership_type; partial unique index on email

## 4. `volunteer_requests`
**Purpose**: incoming public applications to volunteer, prior to approval.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| full_name | VARCHAR(150) | NOT NULL |
| email | VARCHAR(255) | NOT NULL |
| phone | VARCHAR(20) | NOT NULL |
| city | VARCHAR(100) | NULL |
| preferred_area | VARCHAR(150) | NULL |
| message | TEXT | NULL |
| status | VARCHAR(20) | NOT NULL, DEFAULT 'Pending' |
| review_note | TEXT | NULL — reviewer's internal note (required by the admin form spec, missing in the first draft) |
| reviewed_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| reviewed_at | TIMESTAMPTZ | NULL |
| ip_address | INET | NULL — submission source, for abuse/spam triage |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: reviewed_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: CHECK status IN ('Pending','Approved','Rejected'); email format CHECK
- **Indexes**: partial index on status; partial index on created_at; index on email (deliberately **not unique** — the same person may reapply over time)
- **Relationships**: one request optionally becomes one `volunteers` row once approved (1 : 0..1).

## 5. `volunteers`
**Purpose**: roster of approved/active volunteers — distinct from raw requests, maintained by admin staff.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| volunteer_request_id | BIGINT | NULL, UNIQUE, FK → volunteer_requests.id |
| full_name | VARCHAR(150) | NOT NULL |
| email | VARCHAR(255) | NOT NULL |
| phone | VARCHAR(20) | NOT NULL |
| city | VARCHAR(100) | NULL |
| area_of_work | VARCHAR(150) | NULL |
| status | VARCHAR(20) | NOT NULL, DEFAULT 'Active' |
| joined_date | DATE | NOT NULL, DEFAULT CURRENT_DATE |
| created_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| updated_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at / updated_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: volunteer_request_id → volunteer_requests.id, ON DELETE SET NULL, UNIQUE (at most one volunteer per request); created_by_admin_id, updated_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: partial UNIQUE(email) WHERE deleted_at IS NULL; CHECK status IN ('Active','Inactive'); email format CHECK
- **Indexes**: partial index on status; partial unique index on email
- **Relationships**: 1 : 0..1 with volunteer_requests (nullable — an admin can add a volunteer directly, without a prior public request).

## 6. `events`
**Purpose**: public events listing (Activities/Events pages).

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| title | VARCHAR(200) | NOT NULL |
| slug | VARCHAR(220) | NOT NULL |
| category | VARCHAR(100) | NULL — free text by design, see Deferred |
| description | TEXT | NULL |
| location | VARCHAR(255) | NULL |
| event_date | TIMESTAMPTZ | NOT NULL |
| is_published | BOOLEAN | NOT NULL, DEFAULT TRUE |
| cover_image_url | VARCHAR(500) | NULL |
| created_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| updated_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at / updated_at | TIMESTAMPTZ | NOT NULL |

- **Removed**: `is_upcoming` — was a stored boolean that could go stale after an event's date passed. Derive it in queries instead: `WHERE event_date >= now()`.
- **PK**: id
- **FK**: created_by_admin_id, updated_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: partial UNIQUE(slug) WHERE deleted_at IS NULL
- **Indexes**: partial unique index on slug; partial index on event_date; composite partial index on (is_published, event_date) — covers the actual "published + upcoming/past, sorted by date" admin/public query shape
- **Relationships**: one event → many `gallery_media` rows (1 : N).

## 7. `gallery_media`
**Purpose**: unified photo/video library — this single table powers **both** the "Event Gallery" screen (filtered by `event_id`) and the "Gallery Management" screen (all rows). No separate table for each; `event_id` nullability is what distinguishes event-scoped media from general-library media.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| event_id | BIGINT | NULL, FK → events.id |
| title | VARCHAR(200) | NULL |
| caption | TEXT | NULL |
| media_url | VARCHAR(500) | NOT NULL |
| media_type | VARCHAR(10) | NOT NULL, DEFAULT 'Photo' |
| mime_type | VARCHAR(100) | NULL |
| file_size_bytes | BIGINT | NULL |
| year | SMALLINT | NOT NULL — kept alongside `taken_at` since exact date is often unknown while the year is; not redundant |
| taken_at | DATE | NULL |
| display_order | INT | NOT NULL, DEFAULT 0 |
| is_published | BOOLEAN | NOT NULL, DEFAULT TRUE |
| created_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| updated_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at / updated_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: event_id → events.id, ON DELETE SET NULL (deleting an event detaches, not destroys, its photos — and since events are soft-deleted in normal operation, this FK action only matters for a genuine hard-purge job); created_by_admin_id, updated_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: CHECK media_type IN ('Photo','Video'); CHECK year >= 2000
- **Indexes**: partial index on event_id; partial index on year; partial index on media_type; composite partial index on (is_published, created_at DESC) — covers "published items, newest first" list views; composite partial index on (event_id, display_order) — covers ordered event-gallery reads

## 8. `contact_messages`
**Purpose**: public contact-form submissions.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| full_name | VARCHAR(150) | NOT NULL — renamed from `name` for consistency |
| email | VARCHAR(255) | NOT NULL |
| subject | VARCHAR(200) | NULL |
| message | TEXT | NOT NULL |
| is_read | BOOLEAN | NOT NULL, DEFAULT FALSE |
| is_spam | BOOLEAN | NOT NULL, DEFAULT FALSE |
| ip_address | INET | NULL |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: none — standalone
- **Constraints**: email format CHECK
- **Indexes**: composite partial index on (is_read, created_at DESC) — covers "unread, newest first," the actual admin list-view query

## 9. `announcements`
**Purpose**: short, time-bound public notices/banners (separate from long-form blog content).

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| title | VARCHAR(200) | NOT NULL |
| message | TEXT | NOT NULL |
| priority | VARCHAR(20) | NOT NULL, DEFAULT 'Info' |
| start_date | DATE | NOT NULL, DEFAULT CURRENT_DATE |
| end_date | DATE | NULL |
| is_active | BOOLEAN | NOT NULL, DEFAULT TRUE |
| created_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| updated_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at / updated_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: created_by_admin_id, updated_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: CHECK priority IN ('Info','Urgent'); CHECK (end_date IS NULL OR end_date >= start_date)
- **Indexes**: composite partial index on (is_active, start_date, end_date) — covers "currently active" lookups directly

## 10. `testimonials`
**Purpose**: donor/volunteer/beneficiary testimonials shown publicly.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| author_name | VARCHAR(150) | NOT NULL |
| role | VARCHAR(30) | NOT NULL |
| quote | TEXT | NOT NULL |
| photo_url | VARCHAR(500) | NULL |
| rating | SMALLINT | NULL |
| display_order | INT | NOT NULL, DEFAULT 0 |
| is_published | BOOLEAN | NOT NULL, DEFAULT TRUE |
| created_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| updated_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| deleted_at | TIMESTAMPTZ | NULL |
| created_at / updated_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: created_by_admin_id, updated_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: CHECK role IN ('Donor','Volunteer','Beneficiary','Other'); CHECK (rating IS NULL OR rating BETWEEN 1 AND 5)
- **Indexes**: composite partial index on (is_published, display_order)

## 11. `settings`
**Purpose**: site-wide **non-sensitive** configuration, modeled as key/value pairs so new settings don't require schema migrations.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| setting_key | VARCHAR(100) | NOT NULL, UNIQUE |
| setting_value | TEXT | NULL |
| setting_group | VARCHAR(50) | NULL (e.g. 'Organization', 'Social') |
| is_secret | BOOLEAN | NOT NULL, DEFAULT FALSE — marks a value the UI should mask; **real secrets (SMTP credentials, API keys) must not be stored here at all — use environment variables or a secrets manager** |
| updated_by_admin_id | BIGINT | NULL, FK → admin_users.id |
| updated_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: updated_by_admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: UNIQUE(setting_key)
- **Indexes**: unique index on setting_key; index on setting_group
- **No `deleted_at`**: config keys are upserted/overwritten, not recovered from accidental deletion the way a business record would be.

## 12. `audit_logs` — new
**Purpose**: immutable, cross-cutting trail of admin actions — who did what, to which record, when. Complements the per-row `created_by`/`updated_by` columns (which only show the *latest* actor) with full history.

| Column | Type | Notes |
|---|---|---|
| id | BIGSERIAL | PK |
| admin_id | BIGINT | NULL, FK → admin_users.id |
| action | VARCHAR(20) | NOT NULL |
| entity_type | VARCHAR(50) | NOT NULL — e.g. 'Event', 'Member', 'GalleryMedia' |
| entity_id | BIGINT | NULL |
| metadata | JSONB | NULL — optional before/after snapshot or free-form detail |
| ip_address | INET | NULL |
| created_at | TIMESTAMPTZ | NOT NULL |

- **PK**: id
- **FK**: admin_id → admin_users.id, ON DELETE SET NULL
- **Constraints**: CHECK action IN ('Create','Update','Delete','Publish','Unpublish','Approve','Reject','Login','LoginFailed')
- **Indexes**: index on admin_id; composite index on (entity_type, entity_id); index on created_at
- **Relationships**: `(entity_type, entity_id)` is an intentional polymorphic reference, not a strict FK — a single audit table can't have one FK target across ten different entity tables. This is a standard, accepted tradeoff for a generic audit log.
- **Note**: this table grows unboundedly. Not partitioned today (not needed at this org's scale) — flagged in Deferred for future monthly/yearly partitioning if volume grows.

---

## Relationships, in plain language

- **`admin_users` is the root staff table**, and now also references itself (`created_by_admin_id`) to record which admin provisioned another admin's account.
- **`admin_auth_tokens` belongs entirely to one admin** — it's the only table that cascade-deletes with its parent, because a login token has no meaning once the account is gone.
- **`volunteer_requests` → `volunteers` is a one-way promotion.** A public application sits in `volunteer_requests` with status Pending/Approved/Rejected and an optional `review_note`. When staff approve it, a row is created in `volunteers` that optionally points back at the original request. A volunteer can also be added directly by an admin, without ever having filed a request.
- **`members` and `volunteers` are independent rosters**, both admin-authored — nothing forces a Member to also be a Volunteer.
- **`events` → `gallery_media` is one-to-many.** One event can have many photos/videos. The "Event Gallery" admin screen is just `gallery_media` filtered to one `event_id`; "Gallery Management" is the same table unfiltered. One table, two views — not two tables.
- **`contact_messages` and `announcements`/`testimonials`/`settings` are standalone** — they don't reference each other, only optionally the admin who acted on them.
- **`audit_logs` loosely references everything** via `(entity_type, entity_id)`, and directly references `admin_users` for who performed the action.
- **Deleting an admin never deletes their work.** Every `created_by`/`updated_by`/`reviewed_by` FK uses `ON DELETE SET NULL` — the one exception is `admin_auth_tokens`, which cascades, because a token belongs to exactly one login session and has no independent meaning.
- **Soft delete is recoverability, not privacy compliance.** A "deleted" member/volunteer/message still exists in the table (`deleted_at` set) so an accidental delete can be undone; a genuine privacy-erasure request still requires a hard-delete/purge process outside normal admin-panel usage.

---

## Deferred — considered, not adopted (with reasons)

- **`categories` lookup table for `events.category`**: would prevent typos/casing drift, but at this org's scale (a handful of admin-managed category values) it's added complexity without proportionate benefit. Revisit if the category list grows large or needs multi-language labels.
- **Granular roles/permissions tables** (`roles`, `permissions`, `role_permissions`): the 3-value `role` enum on `admin_users` is sufficient for a small trust admin team. A full RBAC schema is a reasonable future extension if per-module permissions are ever needed, not now.
- **Phone number format `CHECK` constraint**: rejected — international formats vary too much for a reliable DB-level regex; validate at the application layer instead.
- **Slugs on `testimonials`/`announcements`/`gallery_media`**: these are shown as lists/sections, not individually-routed detail pages (unlike `events`), so no slug is needed.
- **Full-text search indexes** (GIN/trigram on `events.description`, `testimonials.quote`, `contact_messages.message`): premature at current content volume; add if/when the admin panel needs a real search box over these fields.
- **Partitioning `audit_logs`**: not needed yet; revisit (by month or year) if the table grows into the tens of millions of rows.
