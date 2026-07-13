import { Link, Outlet } from "react-router-dom";
import { ROUTES } from "@/constants/routes";
import { adminModules, adminStats, siteName } from "@/constants/siteContent";

export default function AdminLayout() {
  return (
    <div className="min-h-screen bg-slate-950 text-white lg:grid lg:grid-cols-[280px_1fr]">
      <aside className="border-b border-white/10 bg-slate-950 p-6 lg:min-h-screen lg:border-b-0 lg:border-r">
        <Link to={ROUTES.admin} className="block">
          <p className="text-lg font-semibold">{siteName}</p>
          <p className="mt-1 text-sm text-slate-400">Admin Dashboard</p>
        </Link>

        <div className="mt-8 grid gap-3">
          {adminModules.map((module) => (
            <div key={module} className="rounded-2xl bg-white/5 px-4 py-3 text-sm text-slate-200">
              {module}
            </div>
          ))}
        </div>

        <div className="mt-8 grid gap-3">
          {adminStats.map((stat) => (
            <div key={stat.label} className="rounded-2xl border border-white/10 bg-white/5 px-4 py-4">
              <p className="text-xs uppercase tracking-[0.22em] text-slate-400">{stat.label}</p>
              <p className="mt-2 text-xl font-semibold text-white">{stat.value}</p>
            </div>
          ))}
        </div>
      </aside>

      <section className="bg-slate-950 p-6 lg:p-8">
        <Outlet />
      </section>
    </div>
  );
}
