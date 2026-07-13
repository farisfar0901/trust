import { Link } from "react-router-dom";

export function PageHero({
  eyebrow,
  title,
  description,
  primaryCta,
  secondaryCta,
  accent,
}: Readonly<{
  eyebrow: string;
  title: string;
  description: string;
  primaryCta: { label: string; href: string };
  secondaryCta?: { label: string; href: string };
  accent?: string;
}>) {
  return (
    <section className="relative overflow-hidden rounded-[2rem] border border-amber-100 bg-[linear-gradient(135deg,rgba(255,255,255,0.98),rgba(253,236,200,0.95))] px-6 py-14 shadow-soft sm:px-10 sm:py-16 lg:px-14">
      <div className="absolute inset-0 trust-grid opacity-45" />
      <div className="absolute -right-20 top-0 h-56 w-56 rounded-full bg-amber-200/40 blur-3xl" />
      <div className="absolute bottom-0 left-0 h-56 w-56 rounded-full bg-yellow-200/50 blur-3xl" />
      <div className="relative max-w-3xl">
        <p className="text-sm font-semibold uppercase tracking-[0.3em] text-amber-700">{eyebrow}</p>
        <h1 className="mt-4 text-4xl font-semibold tracking-tight text-slate-950 sm:text-5xl lg:text-6xl">
          {title}
        </h1>
        <p className="mt-6 max-w-2xl text-base leading-8 text-slate-600 sm:text-lg">
          {description}
        </p>
        {accent ? (
          <p className="mt-5 max-w-2xl text-sm font-medium text-amber-800">{accent}</p>
        ) : null}
        <div className="mt-8 flex flex-col gap-3 sm:flex-row">
          <Link
            to={primaryCta.href}
            className="inline-flex items-center justify-center rounded-full bg-amber-700 px-6 py-3 text-sm font-semibold text-white shadow-lg shadow-amber-200 transition hover:bg-amber-800"
          >
            {primaryCta.label}
          </Link>
          {secondaryCta ? (
            <Link
              to={secondaryCta.href}
              className="inline-flex items-center justify-center rounded-full border border-amber-200 bg-white px-6 py-3 text-sm font-semibold text-amber-800 transition hover:border-amber-300 hover:bg-amber-50"
            >
              {secondaryCta.label}
            </Link>
          ) : null}
        </div>
      </div>
    </section>
  );
}
