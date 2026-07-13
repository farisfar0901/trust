export function SectionHeading({
  eyebrow,
  title,
  description,
  align = "left",
}: Readonly<{
  eyebrow: string;
  title: string;
  description: string;
  align?: "left" | "center";
}>) {
  return (
    <div className={align === "center" ? "mx-auto max-w-3xl text-center" : "max-w-3xl"}>
      <p className="text-sm font-semibold uppercase tracking-[0.28em] text-amber-700">{eyebrow}</p>
      <h2 className="mt-3 text-3xl font-semibold tracking-tight text-slate-950 sm:text-4xl">
        {title}
      </h2>
      <p className="mt-4 text-base leading-7 text-slate-600 sm:text-lg">{description}</p>
    </div>
  );
}
