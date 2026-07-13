import { useTranslation } from "react-i18next";
import { Container, SectionHeading, StatCard, TimelineCard } from "@/components/common";

interface StatItem {
  label: string;
  value: string;
}

interface TimelineItem {
  year: string;
  title: string;
  description: string;
}

export default function AboutPage() {
  const { t } = useTranslation(["about", "home"]);

  const heroStats = t("home:heroStats", { returnObjects: true }) as StatItem[];
  const founderNotes = t("about:founderNotes", { returnObjects: true }) as string[];
  const objectives = t("about:objectives", { returnObjects: true }) as string[];
  const timeline = t("about:timeline", { returnObjects: true }) as TimelineItem[];

  return (
    <div className="space-y-16 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow={t("about:heading.eyebrow")}
          title={t("about:heading.title")}
          description={t("about:heading.description")}
        />
      </Container>

      <Container>
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {heroStats.slice(0, 4).map((stat) => (
            <StatCard key={stat.label} {...stat} />
          ))}
        </div>
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("about:historySection.eyebrow")}
              title={t("about:historySection.title")}
              description={t("about:historySection.description")}
            />
            <div className="mt-6 grid gap-4">
              {founderNotes.map((note) => (
                <p key={note} className="rounded-2xl bg-blue-50 px-4 py-4 text-sm leading-7 text-slate-700">
                  {note}
                </p>
              ))}
            </div>
          </div>
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("about:founderSection.eyebrow")}
              title={t("about:founderSection.title")}
              description={t("about:founderSection.description")}
            />
            <div className="mt-6 rounded-[1.5rem] bg-gradient-to-br from-blue-700 to-sky-500 p-6 text-white">
              <p className="text-sm uppercase tracking-[0.24em] text-blue-100">
                {t("about:founderSection.statementLabel")}
              </p>
              <p className="mt-4 text-lg leading-8">“{t("about:founderSection.statement")}”</p>
            </div>
          </div>
        </div>
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-2">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("about:missionSection.eyebrow")}
              title={t("about:missionSection.title")}
              description={t("about:missionSection.description")}
            />
            <div className="mt-6 grid gap-4">
              {objectives.slice(0, 2).map((objective) => (
                <p key={objective} className="rounded-2xl bg-blue-50 px-4 py-4 text-sm leading-7 text-slate-700">
                  {objective}
                </p>
              ))}
            </div>
          </div>
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("about:visionSection.eyebrow")}
              title={t("about:visionSection.title")}
              description={t("about:visionSection.description")}
            />
            <div className="mt-6 grid gap-4">
              {objectives.slice(2).map((objective) => (
                <p key={objective} className="rounded-2xl bg-sky-50 px-4 py-4 text-sm leading-7 text-slate-700">
                  {objective}
                </p>
              ))}
            </div>
          </div>
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("about:objectivesSection.eyebrow")}
          title={t("about:objectivesSection.title")}
          description={t("about:objectivesSection.description")}
        />
        <div className="mt-8 grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          {objectives.map((objective, index) => (
            <div key={objective} className="rounded-[1.5rem] border border-blue-100 bg-white p-6 shadow-soft">
              <p className="text-xs font-semibold uppercase tracking-[0.24em] text-blue-700">0{index + 1}</p>
              <p className="mt-3 text-sm leading-7 text-slate-700">{objective}</p>
            </div>
          ))}
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("about:timelineSection.eyebrow")}
          title={t("about:timelineSection.title")}
          description={t("about:timelineSection.description")}
        />
        <div className="mt-8 grid gap-4 lg:grid-cols-2">
          {timeline.map((item) => (
            <TimelineCard key={item.year} {...item} />
          ))}
        </div>
      </Container>
    </div>
  );
}
