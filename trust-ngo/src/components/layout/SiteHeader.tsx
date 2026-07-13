import { useMemo, useState } from "react";
import { Link, useLocation } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Container } from "@/components/common";
import { LanguageSwitcher } from "@/components/layout/LanguageSwitcher";
import { ROUTES } from "@/constants/routes";
// import  logo from'../../assets/images/vk.jpg'
import logo from '../../assets/vk.jpg'
export function SiteHeader() {
  const { t } = useTranslation(["common", "nav"]);
  const location = useLocation();
  const [menuOpen, setMenuOpen] = useState(false);
  const activeHref = useMemo(() => location.pathname, [location.pathname]);

  const navItems: { href: string; label: string }[] = [
    { href: ROUTES.home, label: t("nav:home") },
    { href: ROUTES.about, label: t("nav:about") },
    { href: ROUTES.activities, label: t("nav:activities") },
    { href: ROUTES.events, label: t("nav:events") },
    { href: ROUTES.gallery, label: t("nav:gallery") },
    { href: ROUTES.blogs, label: t("nav:blogs") },
    { href: ROUTES.volunteer, label: t("nav:volunteer") },
    { href: ROUTES.donation, label: t("nav:donation") },
    { href: ROUTES.contact, label: t("nav:contact") },
  ];

  return (
    <header className="sticky top-0 z-50 border-b border-amber-100/70 bg-white/85 shadow-soft backdrop-blur-xl">
      <Container className="flex items-center justify-between gap-4 py-4">
        <div className="flex min-w-0 items-center gap-8">
          <Link to={ROUTES.home} className="flex shrink-0 items-center gap-3">
        
            <img src = {logo} alt={t("common:siteName")}  width='50px' height='50px'/>
            <div>
              <p className="text-sm font-semibold text-slate-950">{t("common:siteName")}</p>
              <p className="text-xs text-slate-500">{t("common:tagline")}</p>
            </div>
          </Link>

          <nav className="hidden items-center gap-1 overflow-x-auto lg:flex">
            {navItems.map((item) => {
              const selected =
                item.href === ROUTES.home
                  ? activeHref === ROUTES.home
                  : activeHref === item.href || activeHref.startsWith(`${item.href}/`);

              return (
                <Link
                  key={item.href}
                  to={item.href}
                  className={`shrink-0 rounded-full px-4 py-2 text-sm font-medium transition ${
                    selected ? "bg-amber-50 text-amber-800" : "text-slate-600 hover:bg-slate-100"
                  }`}
                >
                  {item.label}
                </Link>
              );
            })}
          </nav>
        </div>

        <div className="hidden shrink-0 md:block">
          <LanguageSwitcher compact />
        </div>

        <button
          type="button"
          aria-expanded={menuOpen}
          aria-label={t("common:openMenu")}
          onClick={() => setMenuOpen((value) => !value)}
          className="inline-flex h-11 w-11 items-center justify-center rounded-2xl border border-amber-100 bg-white text-slate-700 shadow-sm lg:hidden"
        >
          <span className="sr-only">{t("common:openMenu")}</span>
          <span className="space-y-1.5">
            <span className="block h-0.5 w-5 rounded-full bg-current" />
            <span className="block h-0.5 w-5 rounded-full bg-current" />
            <span className="block h-0.5 w-5 rounded-full bg-current" />
          </span>
        </button>
      </Container>

      {menuOpen ? (
        <div className="border-t border-amber-100 bg-white lg:hidden">
          <Container className="grid gap-2 py-4">
            <LanguageSwitcher className="mb-2 w-fit" />
            {navItems.map((item) => (
              <Link
                key={item.href}
                to={item.href}
                onClick={() => setMenuOpen(false)}
                className="rounded-2xl px-4 py-3 text-sm font-medium text-slate-700 hover:bg-amber-50"
              >
                {item.label}
              </Link>
            ))}
          </Container>
        </div>
      ) : null}
    </header>
  );
}
