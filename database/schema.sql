-- =============================================================================
-- Trust Management System — PostgreSQL 16 Schema
-- Generated from: docs/DATABASE_DESIGN.md (final, reviewed design)
-- Planning-to-implementation artifact only. No seed data, no EF models,
-- no application code. Safe to run against an empty database.
-- =============================================================================

BEGIN;

-- =============================================================================
-- 1. admin_users
-- Root staff identity table. Self-referential created_by_admin_id records
-- which admin provisioned another admin's account.
-- =============================================================================
CREATE TABLE admin_users (
    id                      BIGSERIAL PRIMARY KEY,
    full_name               VARCHAR(150)  NOT NULL,
    email                   VARCHAR(255)  NOT NULL,
    password_hash           VARCHAR(255)  NOT NULL,
    role                    VARCHAR(30)   NOT NULL DEFAULT 'Editor',
    is_active               BOOLEAN       NOT NULL DEFAULT TRUE,
    mfa_enabled             BOOLEAN       NOT NULL DEFAULT FALSE,
    mfa_secret              VARCHAR(255)  NULL,
    failed_login_attempts   SMALLINT      NOT NULL DEFAULT 0,
    locked_until            TIMESTAMPTZ   NULL,
    password_changed_at     TIMESTAMPTZ   NULL,
    last_login_at           TIMESTAMPTZ   NULL,
    last_login_ip           INET          NULL,
    created_by_admin_id     BIGINT        NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_admin_users_role CHECK (role IN ('SuperAdmin', 'Admin', 'Editor')),
    CONSTRAINT chk_admin_users_email_format CHECK (email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    CONSTRAINT fk_admin_users_created_by FOREIGN KEY (created_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

-- Partial: only one active (non-deleted) account may hold a given email.
CREATE UNIQUE INDEX ux_admin_users_email ON admin_users (email) WHERE deleted_at IS NULL;
CREATE INDEX ix_admin_users_is_active ON admin_users (is_active) WHERE deleted_at IS NULL;
-- Not partial by design: this index exists to find soft-deleted rows, so it
-- must not exclude them.
CREATE INDEX ix_admin_users_deleted_at ON admin_users (deleted_at);


-- =============================================================================
-- 2. admin_auth_tokens
-- Server-side-revocable refresh / password-reset tokens. Not soft-deletable —
-- tokens are only ever expired, used, or revoked, never "recovered".
-- =============================================================================
CREATE TABLE admin_auth_tokens (
    id              BIGSERIAL PRIMARY KEY,
    admin_id        BIGINT        NOT NULL,
    token_type      VARCHAR(20)   NOT NULL,
    token_hash      VARCHAR(255)  NOT NULL,
    expires_at      TIMESTAMPTZ   NOT NULL,
    used_at         TIMESTAMPTZ   NULL,
    revoked_at      TIMESTAMPTZ   NULL,
    created_ip      INET          NULL,
    created_at      TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_admin_auth_tokens_type CHECK (token_type IN ('RefreshToken', 'PasswordReset')),
    CONSTRAINT uq_admin_auth_tokens_token_hash UNIQUE (token_hash),
    -- Only FK in the schema that cascades: a token has no meaning without its account.
    CONSTRAINT fk_admin_auth_tokens_admin FOREIGN KEY (admin_id)
        REFERENCES admin_users (id) ON DELETE CASCADE
);

CREATE INDEX ix_admin_auth_tokens_admin_id ON admin_auth_tokens (admin_id);
CREATE INDEX ix_admin_auth_tokens_expires_at ON admin_auth_tokens (expires_at);


-- =============================================================================
-- 3. members
-- Roster of registered/enrolled trust members. Admin-authored.
-- =============================================================================
CREATE TABLE members (
    id                      BIGSERIAL PRIMARY KEY,
    full_name               VARCHAR(150)  NOT NULL,
    email                   VARCHAR(255)  NOT NULL,
    phone                   VARCHAR(20)   NOT NULL,
    address                 TEXT          NULL,
    membership_type         VARCHAR(30)   NOT NULL,
    status                  VARCHAR(20)   NOT NULL DEFAULT 'Active',
    joined_date              DATE          NOT NULL DEFAULT CURRENT_DATE,
    photo_url               VARCHAR(500)  NULL,
    created_by_admin_id     BIGINT        NULL,
    updated_by_admin_id     BIGINT        NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_members_membership_type CHECK (membership_type IN ('General', 'Lifetime', 'Honorary', 'Patron')),
    CONSTRAINT chk_members_status CHECK (status IN ('Active', 'Inactive')),
    CONSTRAINT chk_members_email_format CHECK (email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    CONSTRAINT fk_members_created_by FOREIGN KEY (created_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL,
    CONSTRAINT fk_members_updated_by FOREIGN KEY (updated_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE UNIQUE INDEX ux_members_email ON members (email) WHERE deleted_at IS NULL;
CREATE INDEX ix_members_status ON members (status) WHERE deleted_at IS NULL;
CREATE INDEX ix_members_membership_type ON members (membership_type) WHERE deleted_at IS NULL;


-- =============================================================================
-- 4. volunteer_requests
-- Public volunteer applications, prior to approval. Not admin-authored —
-- the public has no admin identity, so there is no created_by column here.
-- =============================================================================
CREATE TABLE volunteer_requests (
    id                      BIGSERIAL PRIMARY KEY,
    full_name               VARCHAR(150)  NOT NULL,
    email                   VARCHAR(255)  NOT NULL,
    phone                   VARCHAR(20)   NOT NULL,
    city                    VARCHAR(100)  NULL,
    preferred_area          VARCHAR(150)  NULL,
    message                 TEXT          NULL,
    status                  VARCHAR(20)   NOT NULL DEFAULT 'Pending',
    review_note             TEXT          NULL,
    reviewed_by_admin_id    BIGINT        NULL,
    reviewed_at             TIMESTAMPTZ   NULL,
    ip_address              INET          NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_volunteer_requests_status CHECK (status IN ('Pending', 'Approved', 'Rejected')),
    CONSTRAINT chk_volunteer_requests_email_format CHECK (email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    CONSTRAINT fk_volunteer_requests_reviewed_by FOREIGN KEY (reviewed_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE INDEX ix_volunteer_requests_status ON volunteer_requests (status) WHERE deleted_at IS NULL;
CREATE INDEX ix_volunteer_requests_created_at ON volunteer_requests (created_at) WHERE deleted_at IS NULL;
-- Deliberately not unique: the same person may submit more than one request over time.
CREATE INDEX ix_volunteer_requests_email ON volunteer_requests (email) WHERE deleted_at IS NULL;


-- =============================================================================
-- 5. volunteers
-- Roster of approved/active volunteers. Admin-authored; optionally traces
-- back to the request it was promoted from.
-- =============================================================================
CREATE TABLE volunteers (
    id                      BIGSERIAL PRIMARY KEY,
    volunteer_request_id    BIGINT        NULL,
    full_name               VARCHAR(150)  NOT NULL,
    email                   VARCHAR(255)  NOT NULL,
    phone                   VARCHAR(20)   NOT NULL,
    city                    VARCHAR(100)  NULL,
    area_of_work            VARCHAR(150)  NULL,
    status                  VARCHAR(20)   NOT NULL DEFAULT 'Active',
    joined_date              DATE          NOT NULL DEFAULT CURRENT_DATE,
    created_by_admin_id     BIGINT        NULL,
    updated_by_admin_id     BIGINT        NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_volunteers_status CHECK (status IN ('Active', 'Inactive')),
    CONSTRAINT chk_volunteers_email_format CHECK (email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$'),
    -- Intentionally a plain (non-partial) UNIQUE: a request may be promoted to
    -- a volunteer at most once, ever — soft-deleting the volunteer row must not
    -- allow the same request to be "re-promoted" into a second volunteer.
    CONSTRAINT uq_volunteers_volunteer_request_id UNIQUE (volunteer_request_id),
    CONSTRAINT fk_volunteers_request FOREIGN KEY (volunteer_request_id)
        REFERENCES volunteer_requests (id) ON DELETE SET NULL,
    CONSTRAINT fk_volunteers_created_by FOREIGN KEY (created_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL,
    CONSTRAINT fk_volunteers_updated_by FOREIGN KEY (updated_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE UNIQUE INDEX ux_volunteers_email ON volunteers (email) WHERE deleted_at IS NULL;
CREATE INDEX ix_volunteers_status ON volunteers (status) WHERE deleted_at IS NULL;


-- =============================================================================
-- 6. events
-- Public events listing. `is_upcoming` is deliberately NOT a stored column —
-- derive it at query time as `event_date >= now()` to avoid stale data.
-- =============================================================================
CREATE TABLE events (
    id                      BIGSERIAL PRIMARY KEY,
    title                   VARCHAR(200)  NOT NULL,
    slug                    VARCHAR(220)  NOT NULL,
    category                VARCHAR(100)  NULL,
    description             TEXT          NULL,
    location                VARCHAR(255)  NULL,
    event_date              TIMESTAMPTZ   NOT NULL,
    is_published             BOOLEAN       NOT NULL DEFAULT TRUE,
    cover_image_url         VARCHAR(500)  NULL,
    created_by_admin_id     BIGINT        NULL,
    updated_by_admin_id     BIGINT        NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT fk_events_created_by FOREIGN KEY (created_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL,
    CONSTRAINT fk_events_updated_by FOREIGN KEY (updated_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE UNIQUE INDEX ux_events_slug ON events (slug) WHERE deleted_at IS NULL;
CREATE INDEX ix_events_event_date ON events (event_date) WHERE deleted_at IS NULL;
-- Covers the actual admin/public query shape: published events sorted by date.
CREATE INDEX ix_events_published_date ON events (is_published, event_date) WHERE deleted_at IS NULL;


-- =============================================================================
-- 7. gallery_media
-- Unified photo/video library. Powers BOTH the "Event Gallery" screen
-- (filtered by event_id) and "Gallery Management" (unfiltered) — one table,
-- two views.
-- =============================================================================
CREATE TABLE gallery_media (
    id                      BIGSERIAL PRIMARY KEY,
    event_id                BIGINT        NULL,
    title                   VARCHAR(200)  NULL,
    caption                 TEXT          NULL,
    media_url               VARCHAR(500)  NOT NULL,
    media_type              VARCHAR(10)   NOT NULL DEFAULT 'Photo',
    mime_type                VARCHAR(100)  NULL,
    file_size_bytes         BIGINT        NULL,
    year                    SMALLINT      NOT NULL,
    taken_at                DATE          NULL,
    display_order           INT           NOT NULL DEFAULT 0,
    is_published             BOOLEAN       NOT NULL DEFAULT TRUE,
    created_by_admin_id     BIGINT        NULL,
    updated_by_admin_id     BIGINT        NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_gallery_media_type CHECK (media_type IN ('Photo', 'Video')),
    CONSTRAINT chk_gallery_media_year CHECK (year >= 2000),
    -- SET NULL, not CASCADE: deleting an event detaches its media rather than
    -- destroying them. Only matters for a genuine hard-purge job, since events
    -- are soft-deleted in normal operation.
    CONSTRAINT fk_gallery_media_event FOREIGN KEY (event_id)
        REFERENCES events (id) ON DELETE SET NULL,
    CONSTRAINT fk_gallery_media_created_by FOREIGN KEY (created_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL,
    CONSTRAINT fk_gallery_media_updated_by FOREIGN KEY (updated_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE INDEX ix_gallery_media_event_id ON gallery_media (event_id) WHERE deleted_at IS NULL;
CREATE INDEX ix_gallery_media_year ON gallery_media (year) WHERE deleted_at IS NULL;
CREATE INDEX ix_gallery_media_media_type ON gallery_media (media_type) WHERE deleted_at IS NULL;
-- Covers "published items, newest first" list views.
CREATE INDEX ix_gallery_media_published_created ON gallery_media (is_published, created_at DESC) WHERE deleted_at IS NULL;
-- Covers ordered event-gallery reads.
CREATE INDEX ix_gallery_media_event_display_order ON gallery_media (event_id, display_order) WHERE deleted_at IS NULL;


-- =============================================================================
-- 8. contact_messages
-- Public contact-form submissions. Standalone — no foreign keys.
-- =============================================================================
CREATE TABLE contact_messages (
    id              BIGSERIAL PRIMARY KEY,
    full_name       VARCHAR(150)  NOT NULL,
    email           VARCHAR(255)  NOT NULL,
    subject         VARCHAR(200)  NULL,
    message         TEXT          NOT NULL,
    is_read         BOOLEAN       NOT NULL DEFAULT FALSE,
    is_spam         BOOLEAN       NOT NULL DEFAULT FALSE,
    ip_address      INET          NULL,
    deleted_at      TIMESTAMPTZ   NULL,
    created_at      TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_contact_messages_email_format CHECK (email ~* '^[^@\s]+@[^@\s]+\.[^@\s]+$')
);

-- Covers "unread, newest first" — the actual admin list-view query.
CREATE INDEX ix_contact_messages_read_created ON contact_messages (is_read, created_at DESC) WHERE deleted_at IS NULL;


-- =============================================================================
-- 9. announcements
-- Short, time-bound public notices. Admin-authored.
-- =============================================================================
CREATE TABLE announcements (
    id                      BIGSERIAL PRIMARY KEY,
    title                   VARCHAR(200)  NOT NULL,
    message                 TEXT          NOT NULL,
    priority                VARCHAR(20)   NOT NULL DEFAULT 'Info',
    start_date              DATE          NOT NULL DEFAULT CURRENT_DATE,
    end_date                DATE          NULL,
    is_active               BOOLEAN       NOT NULL DEFAULT TRUE,
    created_by_admin_id     BIGINT        NULL,
    updated_by_admin_id     BIGINT        NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_announcements_priority CHECK (priority IN ('Info', 'Urgent')),
    CONSTRAINT chk_announcements_date_range CHECK (end_date IS NULL OR end_date >= start_date),
    CONSTRAINT fk_announcements_created_by FOREIGN KEY (created_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL,
    CONSTRAINT fk_announcements_updated_by FOREIGN KEY (updated_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

-- Covers "currently active" lookups directly.
CREATE INDEX ix_announcements_active_window ON announcements (is_active, start_date, end_date) WHERE deleted_at IS NULL;


-- =============================================================================
-- 10. testimonials
-- Donor/volunteer/beneficiary testimonials. Admin-authored.
-- =============================================================================
CREATE TABLE testimonials (
    id                      BIGSERIAL PRIMARY KEY,
    author_name             VARCHAR(150)  NOT NULL,
    role                    VARCHAR(30)   NOT NULL,
    quote                   TEXT          NOT NULL,
    photo_url               VARCHAR(500)  NULL,
    rating                  SMALLINT      NULL,
    display_order           INT           NOT NULL DEFAULT 0,
    is_published             BOOLEAN       NOT NULL DEFAULT TRUE,
    created_by_admin_id     BIGINT        NULL,
    updated_by_admin_id     BIGINT        NULL,
    deleted_at              TIMESTAMPTZ   NULL,
    created_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_testimonials_role CHECK (role IN ('Donor', 'Volunteer', 'Beneficiary', 'Other')),
    CONSTRAINT chk_testimonials_rating CHECK (rating IS NULL OR rating BETWEEN 1 AND 5),
    CONSTRAINT fk_testimonials_created_by FOREIGN KEY (created_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL,
    CONSTRAINT fk_testimonials_updated_by FOREIGN KEY (updated_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE INDEX ix_testimonials_published_order ON testimonials (is_published, display_order) WHERE deleted_at IS NULL;


-- =============================================================================
-- 11. settings
-- Site-wide NON-SENSITIVE configuration as key/value pairs. Real secrets
-- (SMTP credentials, API keys) must never be stored here, is_secret or not —
-- use environment variables / a secrets manager instead. No deleted_at: rows
-- are upserted/overwritten, not "recovered".
-- =============================================================================
CREATE TABLE settings (
    id                      BIGSERIAL PRIMARY KEY,
    setting_key             VARCHAR(100)  NOT NULL,
    setting_value           TEXT          NULL,
    setting_group           VARCHAR(50)   NULL,
    is_secret               BOOLEAN       NOT NULL DEFAULT FALSE,
    updated_by_admin_id     BIGINT        NULL,
    updated_at              TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT uq_settings_setting_key UNIQUE (setting_key),
    CONSTRAINT fk_settings_updated_by FOREIGN KEY (updated_by_admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE INDEX ix_settings_group ON settings (setting_group);


-- =============================================================================
-- 12. audit_logs
-- Immutable, cross-cutting trail of admin actions. (entity_type, entity_id)
-- is an intentional polymorphic reference, not a strict FK — a generic audit
-- table cannot target ten different entity tables with one foreign key.
-- =============================================================================
CREATE TABLE audit_logs (
    id              BIGSERIAL PRIMARY KEY,
    admin_id        BIGINT        NULL,
    action          VARCHAR(20)   NOT NULL,
    entity_type     VARCHAR(50)   NOT NULL,
    entity_id       BIGINT        NULL,
    metadata        JSONB         NULL,
    ip_address      INET          NULL,
    created_at      TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT chk_audit_logs_action CHECK (
        action IN ('Create', 'Update', 'Delete', 'Publish', 'Unpublish', 'Approve', 'Reject', 'Login', 'LoginFailed')
    ),
    CONSTRAINT fk_audit_logs_admin FOREIGN KEY (admin_id)
        REFERENCES admin_users (id) ON DELETE SET NULL
);

CREATE INDEX ix_audit_logs_admin_id ON audit_logs (admin_id);
CREATE INDEX ix_audit_logs_entity ON audit_logs (entity_type, entity_id);
CREATE INDEX ix_audit_logs_created_at ON audit_logs (created_at);

COMMIT;
