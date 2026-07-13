import { useTranslation } from "react-i18next";
import { changeLanguage, type SupportedLanguage } from "@/i18n";

export function LanguageSwitcher({
  className = "",
  compact = false,
}: Readonly<{ className?: string; compact?: boolean }>) {
  const { t, i18n } = useTranslation("common");
  const currentLanguage = i18n.language as SupportedLanguage;

  const options: { code: SupportedLanguage; label: string }[] = [
    { code: "en", label: t("english") },
    { code: "ta", label: t("tamil") },
  ];

  return (
    <div
      role="group"
      aria-label={t("language")}
      className={`inline-flex items-center rounded-full border border-amber-200 bg-white ${
        compact ? "gap-[3.5px] p-[3.5px]" : "gap-1 p-1"
      } ${className}`}
    >
      {options.map((option) => {
        const selected = currentLanguage === option.code;
        return (
          <button
            key={option.code}
            type="button"
            aria-pressed={selected}
            onClick={() => changeLanguage(option.code)}
            style={compact ? { fontSize: "10.5px" } : undefined}
            className={`rounded-full font-semibold uppercase tracking-[0.1em] transition ${
              compact ? "px-[10.5px] py-[5.25px] text-[10.5px]" : "px-3 py-1.5 text-xs"
            } ${selected ? "bg-amber-700 text-white shadow-sm" : "text-slate-600 hover:bg-amber-50"}`}
          >
            {option.label}
          </button>
        );
      })}
    </div>
  );
}
