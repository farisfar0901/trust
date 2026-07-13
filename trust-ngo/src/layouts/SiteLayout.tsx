import { Outlet } from "react-router-dom";
import { SiteFooter, SiteHeader } from "@/components/layout";

export default function SiteLayout() {
  return (
    <>
      <SiteHeader />
      <main className="flex-1">
        <Outlet />
      </main>
      <SiteFooter />
    </>
  );
}
