import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import {
  Container,
  FeatureCard,
  PageHero,
  SectionHeading,
  StatCard,
} from "@/components/common";
import { ROUTES } from "@/constants/routes";
import { homeGalleryPreviewImages } from "@/constants/media";

interface StatItem {
  label: string;
  value: string;
}

interface TitledItem {
  title: string;
  description: string;
}

interface EventItem {
  date: string;
  title: string;
  location: string;
  status?: string;
}

interface TestimonialItem {
  quote: string;
  name: string;
  role: string;
}

interface GalleryPreviewItem {
  title: string;
  label: string;
}

export default function HomePage() {
  const { t } = useTranslation(["home", "activities", "events"]);

  const heroStats = t("home:heroStats", { returnObjects: true }) as StatItem[];
  const trustHighlights = t("home:trustHighlights", { returnObjects: true }) as TitledItem[];
  const missionPoints = t("home:missionPoints", { returnObjects: true }) as string[];
  const visionPoints = t("home:visionPoints", { returnObjects: true }) as string[];
  const activityCards = t("activities:cards", { returnObjects: true }) as (TitledItem & {
    category: string;
  })[];
  const upcomingEvents = t("events:upcomingEvents", { returnObjects: true }) as EventItem[];
  const donationCtaLines = t("home:donationCta.lines", { returnObjects: true }) as string[];
  const testimonials = t("home:testimonials", { returnObjects: true }) as TestimonialItem[];
  const galleryPreview = t("home:galleryPreview", { returnObjects: true }) as GalleryPreviewItem[];

  return (
    <div className="space-y-20 py-8 sm:py-10">
      <Container>
        <PageHero
          eyebrow={t("home:hero.eyebrow")}
          title={t("home:hero.title")}
          description={t("home:hero.description")}
          accent={t("home:hero.accent")}
          primaryCta={{ label: t("home:hero.primaryCta"), href: ROUTES.donation }}
          secondaryCta={{ label: t("home:hero.secondaryCta"), href: ROUTES.volunteer }}
        />
      </Container>

      <Container>
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {heroStats.map((stat) => (
            <StatCard key={stat.label} {...stat} />
          ))}
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("home:aboutSection.eyebrow")}
          title={t("home:aboutSection.title")}
          description={t("home:aboutSection.description")}
        />
        <div className="mt-8 grid gap-6 lg:grid-cols-3">
          {trustHighlights.map((item) => (
            <FeatureCard key={item.title} {...item} />
          ))}
        </div>
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-2">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("home:missionSection.eyebrow")}
              title={t("home:missionSection.title")}
              description={t("home:missionSection.description")}
            />
            <div className="mt-6 grid gap-4">
              {missionPoints.map((point) => (
                <p key={point} className="rounded-2xl bg-blue-50 px-4 py-4 text-sm leading-7 text-slate-700">
                  {point}
                </p>
              ))}
            </div>
          </div>
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("home:visionSection.eyebrow")}
              title={t("home:visionSection.title")}
              description={t("home:visionSection.description")}
            />
            <div className="mt-6 grid gap-4">
              {visionPoints.map((point) => (
                <p key={point} className="rounded-2xl bg-sky-50 px-4 py-4 text-sm leading-7 text-slate-700">
                  {point}
                </p>
              ))}
            </div>
          </div>
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("home:activitiesSection.eyebrow")}
          title={t("home:activitiesSection.title")}
          description={t("home:activitiesSection.description")}
        />
        <div className="mt-8 grid gap-6 md:grid-cols-2 xl:grid-cols-3">
          {activityCards.map((item) => (
            <FeatureCard key={item.title} label={item.category} title={item.title} description={item.description} />
          ))}
        </div>
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-[1.2fr_0.8fr]">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("home:eventsSection.eyebrow")}
              title={t("home:eventsSection.title")}
              description={t("home:eventsSection.description")}
            />
            <div className="mt-6 grid gap-4">
              {upcomingEvents.map((event) => (
                <div key={event.title} className="flex flex-col gap-4 rounded-2xl border border-blue-100 p-5 sm:flex-row sm:items-center sm:justify-between">
                  <div>
                    <p className="text-sm font-semibold text-blue-700">{event.date}</p>
                    <h3 className="mt-1 text-lg font-semibold text-slate-950">{event.title}</h3>
                    <p className="mt-1 text-sm text-slate-600">{event.location}</p>
                  </div>
                  <span className="inline-flex w-fit rounded-full bg-blue-50 px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-blue-700">
                    {t("events:statusUpcoming")}
                  </span>
                </div>
              ))}
            </div>
          </div>
          <div className="rounded-[2rem] border border-blue-100 bg-slate-950 p-8 text-white shadow-soft">
            <SectionHeading
              eyebrow={t("home:donationCta.eyebrow")}
              title={t("home:donationCta.title")}
              description={t("home:donationCta.description")}
              align="left"
            />
            <div className="mt-6 grid gap-4 text-sm text-slate-300">
              {donationCtaLines.map((line) => (
                <p key={line} className="rounded-2xl bg-white/5 px-4 py-4 leading-7">
                  {line}
                </p>
              ))}
            </div>
            <Link
              to={ROUTES.donation}
              className="mt-8 inline-flex rounded-full bg-white px-6 py-3 text-sm font-semibold text-slate-950 transition hover:bg-slate-100"
            >
              {t("home:donationCta.cta")}
            </Link>
          </div>
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("home:newsSection.eyebrow")}
          title={t("home:newsSection.title")}
          description={t("home:newsSection.description")}
        />
        <div className="mt-8 grid gap-6 lg:grid-cols-3">
          {(t("blogs:posts", { returnObjects: true, ns: "blogs" }) as never as {
            title: string;
            excerpt: string;
            categoryKey: string;
          }[])
            .slice(0, 3)
            .map((post) => (
              <FeatureCard
                key={post.title}
                label={t(`blogs:categoryLabels.${post.categoryKey}`, { ns: "blogs" })}
                title={post.title}
                description={post.excerpt}
              />
            ))}
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("home:testimonialsSection.eyebrow")}
          title={t("home:testimonialsSection.title")}
          description={t("home:testimonialsSection.description")}
        />
        <div className="mt-8 grid gap-6 lg:grid-cols-3">
          {testimonials.map((item) => (
            <div key={item.name} className="rounded-[1.75rem] border border-blue-100 bg-white p-6 shadow-soft">
              <p className="text-sm leading-7 text-slate-600">“{item.quote}”</p>
              <div className="mt-6 border-t border-blue-100 pt-4">
                <p className="text-sm font-semibold text-slate-950">{item.name}</p>
                <p className="text-xs uppercase tracking-[0.2em] text-slate-500">{item.role}</p>
              </div>
            </div>
          ))}
        </div>
      </Container>

      <Container>
        <SectionHeading
          eyebrow={t("home:galleryPreviewSection.eyebrow")}
          title={t("home:galleryPreviewSection.title")}
          description={t("home:galleryPreviewSection.description")}
        />
        <div className="mt-8 grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {galleryPreview.map((item, index) => (
            <div
              key={item.title}
              className={`group relative overflow-hidden rounded-[1.75rem] border border-amber-100 p-5 shadow-soft ${
                index % 2 === 0 ? "bg-gradient-to-br from-amber-50 to-white" : "bg-gradient-to-br from-yellow-50 to-white"
              }`}
            >
              <div className="relative h-40 overflow-hidden rounded-[1.5rem] bg-[linear-gradient(135deg,rgba(217,119,6,0.16),rgba(255,255,255,0.92))] ring-1 ring-inset ring-amber-100">
                <img
                  src={homeGalleryPreviewImages[index]}
                  alt={item.title}
                  loading="lazy"
                  className="h-full w-full object-cover"
                />
              </div>
              <p className="mt-4 text-sm font-semibold text-amber-700">{item.label}</p>
              <h3 className="mt-1 text-lg font-semibold text-slate-950">{item.title}</h3>
            </div>
          ))}
        </div>
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-2">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("home:volunteerCta.eyebrow")}
              title={t("home:volunteerCta.title")}
              description={t("home:volunteerCta.description")}
            />
            <Link
              to={ROUTES.volunteer}
              className="mt-6 inline-flex rounded-full bg-blue-700 px-6 py-3 text-sm font-semibold text-white transition hover:bg-blue-800"
            >
              {t("home:volunteerCta.cta")}
            </Link>
          </div>
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow={t("home:contactCta.eyebrow")}
              title={t("home:contactCta.title")}
              description={t("home:contactCta.description")}
            />
            <div className="mt-6 grid gap-3 text-sm text-slate-600">
              <p>{t("home:contactCta.address")}</p>
              <p>{t("home:contactCta.email")}</p>
              <p>{t("home:contactCta.phone")}</p>
            </div>
            <Link
              to={ROUTES.contact}
              className="mt-6 inline-flex rounded-full border border-blue-200 px-6 py-3 text-sm font-semibold text-blue-800 transition hover:bg-blue-50"
            >
              {t("home:contactCta.cta")}
            </Link>
          </div>
        </div>
      </Container>
    </div>
  );
}
