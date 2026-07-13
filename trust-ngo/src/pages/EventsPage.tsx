import { useTranslation } from "react-i18next";
import { Container, FeatureCard, SectionHeading } from "@/components/common";

interface EventItem {
  date: string;
  title: string;
  location: string;
}

interface EventDetailItem {
  title: string;
  date: string;
  description: string;
}

export default function EventsPage() {
  const { t } = useTranslation("events");

  const upcomingEvents = t("upcomingEvents", { returnObjects: true }) as EventItem[];
  const completedEvents = t("completedEvents", { returnObjects: true }) as EventItem[];
  const eventDetails = t("eventDetails", { returnObjects: true }) as EventDetailItem[];

  return (
    <div className="space-y-16 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow={t("heading.eyebrow")}
          title={t("heading.title")}
          description={t("heading.description")}
        />
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("upcomingSection.eyebrow")}
          title={t("upcomingSection.title")}
          description={t("upcomingSection.description")}
        />
        <div className="mt-8 grid gap-6 lg:grid-cols-3">
          {upcomingEvents.map((event) => (
            <FeatureCard key={event.title} label={event.date} title={event.title} description={event.location} />
          ))}
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("completedSection.eyebrow")}
          title={t("completedSection.title")}
          description={t("completedSection.description")}
        />
        <div className="mt-8 grid gap-6 lg:grid-cols-2">
          {completedEvents.map((event) => (
            <div key={event.title} className="rounded-[1.75rem] border border-blue-100 bg-white p-6 shadow-soft">
              <p className="text-sm font-semibold text-blue-700">{event.date}</p>
              <h3 className="mt-2 text-xl font-semibold text-slate-950">{event.title}</h3>
              <p className="mt-2 text-sm text-slate-600">{event.location}</p>
              <span className="mt-5 inline-flex rounded-full bg-blue-50 px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-blue-700">
                {t("statusCompleted")}
              </span>
            </div>
          ))}
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("detailsSection.eyebrow")}
          title={t("detailsSection.title")}
          description={t("detailsSection.description")}
        />
        <div className="mt-8 grid gap-6 md:grid-cols-2 xl:grid-cols-3">
          {eventDetails.map((event) => (
            <FeatureCard key={event.title} label={event.date} title={event.title} description={event.description} />
          ))}
        </div>
      </Container>
    </div>
  );
}
