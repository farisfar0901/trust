export function TimelineCard({
  year,
  title,
  description,
}: Readonly<{
  year: string;
  title: string;
  description: string;
}>) {
  return (
    <div className="relative rounded-[1.75rem] border border-amber-100 bg-white p-6 pl-8 shadow-soft">
      <div className="absolute left-4 top-8 h-3 w-3 rounded-full bg-amber-600" />
      <p className="text-sm font-semibold uppercase tracking-[0.25em] text-amber-700">{year}</p>
      <h3 className="mt-3 text-xl font-semibold text-slate-950">{title}</h3>
      <p className="mt-3 text-sm leading-7 text-slate-600">{description}</p>
    </div>
  );
}
