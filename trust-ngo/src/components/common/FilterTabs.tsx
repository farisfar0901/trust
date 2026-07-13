export interface FilterOption {
  value: string;
  label: string;
}

export function FilterTabs({
  options,
  active,
  onChange,
}: Readonly<{
  options: FilterOption[];
  active: string;
  onChange: (value: string) => void;
}>) {
  return (
    <div className="flex flex-wrap gap-2">
      {options.map((option) => {
        const selected = option.value === active;
        return (
          <button
            key={option.value}
            type="button"
            onClick={() => onChange(option.value)}
            className={`rounded-full px-4 py-2 text-sm font-semibold transition ${
              selected
                ? "bg-amber-700 text-white shadow-lg shadow-amber-200"
                : "bg-white text-slate-600 ring-1 ring-inset ring-amber-100 hover:bg-amber-50"
            }`}
          >
            {option.label}
          </button>
        );
      })}
    </div>
  );
}
