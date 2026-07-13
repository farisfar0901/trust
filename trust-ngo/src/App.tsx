import { Route, Routes } from "react-router-dom";
import SiteLayout from "@/layouts/SiteLayout";
import AdminLayout from "@/layouts/AdminLayout";
import { ROUTES } from "@/constants/routes";
import HomePage from "@/pages/HomePage";
import AboutPage from "@/pages/AboutPage";
import ActivitiesPage from "@/pages/ActivitiesPage";
import EventsPage from "@/pages/EventsPage";
import GalleryPage from "@/pages/GalleryPage";
import BlogsPage from "@/pages/BlogsPage";
import VolunteerPage from "@/pages/VolunteerPage";
import DonationPage from "@/pages/DonationPage";
import ContactPage from "@/pages/ContactPage";
import AdminDashboardPage from "@/pages/admin/AdminDashboardPage";

export default function App() {
  return (
    <Routes>
      <Route element={<SiteLayout />}>
        <Route path={ROUTES.home} element={<HomePage />} />
        <Route path={ROUTES.about} element={<AboutPage />} />
        <Route path={ROUTES.activities} element={<ActivitiesPage />} />
        <Route path={ROUTES.events} element={<EventsPage />} />
        <Route path={ROUTES.gallery} element={<GalleryPage />} />
        <Route path={ROUTES.blogs} element={<BlogsPage />} />
        <Route path={ROUTES.volunteer} element={<VolunteerPage />} />
        <Route path={ROUTES.donation} element={<DonationPage />} />
        <Route path={ROUTES.contact} element={<ContactPage />} />
      </Route>
      <Route element={<AdminLayout />}>
        <Route path={ROUTES.admin} element={<AdminDashboardPage />} />
      </Route>
    </Routes>
  );
}
