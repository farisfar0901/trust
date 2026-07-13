import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Container } from "@/components/common";
import { ROUTES } from "@/constants/routes";

export function SiteFooter() {
  const { t } = useTranslation(["common", "nav", "footer"]);

  const exploreLinks: { href: string; label: string }[] = [
    { href: ROUTES.home, label: t("nav:home") },
    { href: ROUTES.about, label: t("nav:about") },
    { href: ROUTES.activities, label: t("nav:activities") },
    { href: ROUTES.events, label: t("nav:events") },
    { href: ROUTES.gallery, label: t("nav:gallery") },
  ];

  return (
    <footer className="border-t border-amber-100 bg-slate-950 text-slate-200">
      <Container className="grid gap-10 py-14 lg:grid-cols-[1.3fr_0.9fr_0.9fr]">
        <div>
          <p className="text-lg font-semibold text-white">{t("common:siteName")}</p>
          <p className="mt-4 max-w-md text-sm leading-7 text-slate-300">{t("footer:description")}</p>
        </div>

        <div>
          <p className="text-sm font-semibold uppercase tracking-[0.24em] text-amber-300">
            {t("footer:exploreHeading")}
          </p>
          <div className="mt-4 grid gap-3 text-sm text-slate-300">
            {exploreLinks.map((item) => (
              <Link key={item.href} to={item.href} className="transition hover:text-white">
                {item.label}
              </Link>
            ))}
          </div>
        </div>

        <div>
          <p className="text-sm font-semibold uppercase tracking-[0.24em] text-amber-300">
            {t("footer:contactHeading")}
          </p>
          <div className="mt-4 grid gap-3 text-sm text-slate-300">
            <p>{t("footer:address")}</p>
            <p>{t("footer:phone")}</p>
            <p>{t("footer:email")}</p>
          </div>
        </div>
      </Container>
      <div className="border-t border-white/10 py-4 text-center text-xs text-slate-400">
        {t("footer:copyright")}
      </div>
    </footer>
  );
}
