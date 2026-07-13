import { useTranslation } from "react-i18next";
import { Container, FeatureCard, SectionHeading } from "@/components/common";

interface ActivityCard {
  category: string;
  title: string;
  description: string;
}

export default function ActivitiesPage() {
  const { t } = useTranslation("activities");
  const cards = t("cards", { returnObjects: true }) as ActivityCard[];

  return (
    <div className="space-y-14 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow={t("heading.eyebrow")}
          title={t("heading.title")}
          description={t("heading.description")}
        />
      </Container>

      <Container>
        <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
          {cards.map((item) => (
            <FeatureCard key={item.title} label={item.category} title={item.title} description={item.description} />
          ))}
        </div>
      </Container>
    </div>
  );
}
