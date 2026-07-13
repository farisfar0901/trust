export function StatCard({
  label,
  value,
  description,
}: Readonly<{
  label: string;
  value: string;
  description?: string;
}>) {
  return (
    <div className="rounded-3xl border border-amber-100 bg-white p-6 shadow-soft">
      <p className="text-sm font-medium text-slate-500">{label}</p>
      <p className="mt-3 text-3xl font-semibold tracking-tight text-slate-950">{value}</p>
      {description ? <p className="mt-2 text-sm leading-6 text-slate-600">{description}</p> : null}
    </div>
  );
}
