export function FeatureCard({
  title,
  description,
  label,
}: Readonly<{
  title: string;
  description: string;
  label?: string;
}>) {
  return (
    <div className="rounded-[1.75rem] border border-amber-100 bg-white p-6 shadow-soft transition hover:-translate-y-1 hover:border-amber-200">
      {label ? (
        <span className="inline-flex rounded-full bg-amber-50 px-3 py-1 text-xs font-semibold uppercase tracking-[0.22em] text-amber-700">
          {label}
        </span>
      ) : null}
      <h3 className="mt-4 text-xl font-semibold text-slate-950">{title}</h3>
      <p className="mt-3 text-sm leading-7 text-slate-600">{description}</p>
    </div>
  );
}
