export function formatDate(value: string | Date, locale = "en-IN"): string {
  const date = typeof value === "string" ? new Date(value) : value;
  return date.toLocaleDateString(locale, {
    year: "numeric",
    month: "short",
    day: "2-digit",
  });
}
