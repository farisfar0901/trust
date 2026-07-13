# Trust Admin Panel — Architecture & Implementation Plan

Stack: React (Vite, existing `trust-ngo`) + ASP.NET Core (existing `trust-api`) + PostgreSQL.
Planning only — no code, no packages.

## 0. Admin Shell (shared across all modules)

- **Layout**: `AdminLayout` (exists) — sidebar nav + topbar (admin user, logout) + content outlet.
- **Auth guard**: route wrapper redirecting unauthenticated users to `/admin/login`.
- **Sidebar nav** (fixed order, matches modules below):
  Dashboard → Volunteer Requests → Members → Events → Event Gallery → Gallery → Contact Messages → Announcements → Testimonials → Settings.
- **Shared UI primitives** (used by every module, not per-module): DataTable (sort/paginate/search), ConfirmDialog, StatusBadge, FormModal/Drawer, FileUpload, RichTextEditor (Announcements/Events/Testimonials descriptions).

---

## 1. Dashboard
- **Purpose**: at-a-glance operational overview.
- **CRUD**: read-only.
- **Pages**: `/admin` — stat cards (pending volunteers, unread messages, upcoming events, total members) + recent-activity feed (latest 5 per module) + quick links.
- **Forms**: none.
- **Tables**: "Recent Activity" summary table (mixed source, not a CRUD table).
- **Actions**: click-through to each module's filtered list (e.g. "12 pending volunteers" → Volunteer Requests filtered by status=Pending).
- **Nav flow**: entry point after login → deep-links into other modules.

## 2. Volunteer Requests
- **Purpose**: review and process public volunteer applications.
- **CRUD**: Read, Update (status), Delete. No Create (submitted publicly).
- **Pages**: List (`/admin/volunteers`), Detail/Review (`/admin/volunteers/:id`).
- **Forms**: Status-update form (Pending → Approved/Rejected + internal note).
- **Tables**: Volunteer list — Name, Email, Phone, City, Preferred Area, Status, Applied Date; filter by status/date.
- **Actions**: Approve, Reject, Delete, Export CSV, view full message.
- **Nav flow**: List → row click → Detail → status action → back to List.

## 3. Members Management
- **Purpose**: manage registered/enrolled trust members (new module — no existing entity).
- **CRUD**: full C/R/U/D.
- **Pages**: List (`/admin/members`), Add/Edit (`/admin/members/new`, `/admin/members/:id/edit`).
- **Forms**: Member form — Name, Email, Phone, Address, Membership Type, Join Date, Status (Active/Inactive), Photo.
- **Tables**: Members list — Name, Membership Type, Status, Join Date, Contact.
- **Actions**: Add, Edit, Deactivate/Activate, Delete, Search/filter by type/status.
- **Nav flow**: List → Add/Edit form → Save → back to List.

## 4. Events Management
- **Purpose**: manage published events shown on the public site.
- **CRUD**: full C/R/U/D.
- **Pages**: List (`/admin/events`), Add/Edit (`/admin/events/new`, `/admin/events/:id/edit`).
- **Forms**: Event form — Title, Slug, Category, Description, Location, Event Date, Cover Image, Is Upcoming, Is Published.
- **Tables**: Events list — Title, Category, Date, Location, Published status.
- **Actions**: Add, Edit, Publish/Unpublish, Delete, "Manage Gallery" (→ Event Gallery scoped to this event).
- **Nav flow**: List → Edit → Save; List row → "Manage Gallery" → Event Gallery module.

## 5. Event Gallery
- **Purpose**: manage photos/videos tied to a *specific* event (event-scoped media, distinct from the general library).
- **CRUD**: Create, Read, Delete (photos are uploaded/removed, rarely "edited" beyond caption).
- **Pages**: `/admin/events/:eventId/gallery` — grid view scoped to one event.
- **Forms**: Upload form — Files (multi), Caption, Media Type (Photo/Video), Taken Date.
- **Tables**: Media grid (not tabular) — thumbnail, caption, type, published toggle.
- **Actions**: Upload, Delete, Reorder, Publish/Unpublish, "View in Gallery Management".
- **Nav flow**: reached only from an Event's detail/edit page; back-link returns to that Event.

## 6. Gallery Management
- **Purpose**: manage the general/global media library (independent of any single event; the public Gallery page's photo+video year filter).
- **CRUD**: full C/R/U/D.
- **Pages**: List/Grid (`/admin/gallery`), Upload (`/admin/gallery/new`).
- **Forms**: Photo/Video form — Title, Caption, Image/Video file, Media Type, Year, Linked Event (optional), Published.
- **Tables**: Gallery grid — thumbnail, title, year, media type, linked event, published.
- **Actions**: Upload, Edit metadata, Delete, Publish/Unpublish, filter by year/type/event.
- **Nav flow**: List → Upload/Edit → Save → back to List.

## 7. Contact Messages
- **Purpose**: view and manage public contact-form submissions.
- **CRUD**: Read, Update (mark read), Delete. No Create.
- **Pages**: List (`/admin/messages`), Detail (`/admin/messages/:id`).
- **Forms**: none (reply happens via external email, not stored).
- **Tables**: Messages list — Name, Email, Subject, Received Date, Read status.
- **Actions**: Mark Read/Unread, Delete, Reply (mailto link), filter unread.
- **Nav flow**: List → row click marks read + opens Detail → back to List.

## 8. Announcements
- **Purpose**: manage short, time-bound notices/banners shown on the public site (distinct from long-form Blog posts).
- **CRUD**: full C/R/U/D.
- **Pages**: List (`/admin/announcements`), Add/Edit (`/admin/announcements/new`, `/:id/edit`).
- **Forms**: Announcement form — Title, Message, Start Date, End Date, Priority/Type (Info/Urgent), Is Active.
- **Tables**: Announcements list — Title, Active window (start–end), Priority, Status.
- **Actions**: Add, Edit, Activate/Deactivate, Delete.
- **Nav flow**: List → Add/Edit → Save → back to List.

## 9. Testimonials
- **Purpose**: manage donor/volunteer/beneficiary testimonials shown publicly.
- **CRUD**: full C/R/U/D.
- **Pages**: List (`/admin/testimonials`), Add/Edit (`/admin/testimonials/new`, `/:id/edit`).
- **Forms**: Testimonial form — Author Name, Role/Relation (Donor/Volunteer/Beneficiary), Quote, Photo, Rating (optional), Is Published.
- **Tables**: Testimonials list — Author, Role, Excerpt, Published status.
- **Actions**: Add, Edit, Publish/Unpublish, Delete, Reorder (display priority).
- **Nav flow**: List → Add/Edit → Save → back to List.

## 10. Settings
- **Purpose**: site-wide configuration and admin account management.
- **CRUD**: Update only (singleton config); admin-users sub-section is full CRUD.
- **Pages**: `/admin/settings` (tabs: Organization Info, Social Links, Admin Users, Change Password).
- **Forms**: Org Info form (name, tagline, address, phone, email, logo); Social Links form; Admin User form (name, email, role); Password change form.
- **Tables**: Admin Users list — Name, Email, Role, Last Login, Status.
- **Actions**: Save settings, Add/Remove admin user, Reset password, toggle role.
- **Nav flow**: tab-based single page; no drill-down.

---

## Frontend Folder Structure (`trust-ngo/src`)

Extends the existing structure — only additive folders under `pages/admin` and `components/admin` are new.

```
src/
  layouts/
    AdminLayout.tsx          (exists)
    SiteLayout.tsx            (exists)
  pages/
    admin/
      DashboardPage.tsx
      volunteers/
        VolunteerListPage.tsx
        VolunteerDetailPage.tsx
      members/
        MemberListPage.tsx
        MemberFormPage.tsx
      events/
        EventListPage.tsx
        EventFormPage.tsx
        EventGalleryPage.tsx        (scoped: /admin/events/:eventId/gallery)
      gallery/
        GalleryListPage.tsx
        GalleryUploadPage.tsx
      messages/
        MessageListPage.tsx
        MessageDetailPage.tsx
      announcements/
        AnnouncementListPage.tsx
        AnnouncementFormPage.tsx
      testimonials/
        TestimonialListPage.tsx
        TestimonialFormPage.tsx
      settings/
        SettingsPage.tsx
      login/
        AdminLoginPage.tsx
  components/
    admin/
      AdminSidebar.tsx
      AdminTopbar.tsx
      DataTable/
      FormModal/
      ConfirmDialog/
      FileUpload/
      StatusBadge/
      RichTextEditor/
  routes/
    adminRoutes.tsx           (route table + auth-guard wiring)
  api/
    admin/
      membersApi.ts
      eventsApi.ts
      galleryApi.ts
      volunteersApi.ts
      messagesApi.ts
      announcementsApi.ts
      testimonialsApi.ts
      settingsApi.ts
      authApi.ts
  types/
    admin/
      member.ts
      event.ts
      gallery.ts
      volunteer.ts
      message.ts
      announcement.ts
      testimonial.ts
      settings.ts
```

## Backend Folder Structure (`trust-api`)

Extends existing structure; new controllers/entities only where no equivalent exists today.

```
trust-api/
  Controllers/
    Admin/
      AuthController.cs            (new — admin login/session)
      DashboardController.cs       (new — aggregated stats)
      MembersController.cs         (new)
      AnnouncementsController.cs   (new)
      TestimonialsController.cs    (new)
      SettingsController.cs        (new)
    EventsController.cs            (exists — extend for gallery-per-event)
    GalleryController.cs           (exists)
    VolunteersController.cs        (exists — extend with status update)
    ContactMessagesController.cs   (exists — extend with mark-read)
    BlogsController.cs             (exists, unrelated to this module set)
    DonationsController.cs         (exists, unrelated to this module set)
    AdminController.cs             (existing — reconcile with new Admin/* controllers)
  Domain/
    Entities.cs                    (extend: Member, Announcement, Testimonial, AdminUser)
  Data/
    TrustDbContext.cs              (extend DbSets + EF migrations for new entities)
  Infrastructure/
    SlugHelper.cs                  (exists)
    Auth/                          (new — JWT/cookie auth for admin)
```

## New PostgreSQL Tables Needed

Existing tables already cover Volunteers, Events, Gallery, Contact Messages, Blogs, Donations.
New tables required: `Members`, `Announcements`, `Testimonials`, `AdminUsers` (+ `Settings` as a single-row config table or key/value table).

## Routing Map (frontend)

```
/admin/login
/admin                              → Dashboard
/admin/volunteers                   → list
/admin/volunteers/:id                → detail/review
/admin/members                      → list
/admin/members/new | /:id/edit      → form
/admin/events                       → list
/admin/events/new | /:id/edit       → form
/admin/events/:eventId/gallery       → event-scoped gallery
/admin/gallery                      → general gallery list
/admin/gallery/new                  → upload
/admin/messages                     → list
/admin/messages/:id                  → detail
/admin/announcements                → list
/admin/announcements/new | /:id/edit → form
/admin/testimonials                 → list
/admin/testimonials/new | /:id/edit  → form
/admin/settings                     → tabbed settings page
```
