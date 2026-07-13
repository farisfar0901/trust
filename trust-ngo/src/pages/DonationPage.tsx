import { Container, FeatureCard, SectionHeading, StatCard } from "@/components/common";
import { bankDetails, donationMethods, impactStats } from "@/constants/siteContent";

export default function DonationPage() {
  return (
    <div className="space-y-14 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow="Donation"
          title="Clear payment options, bank details, and impact reporting"
          description="The donation page is designed to build trust through clarity, not pressure."
        />
      </Container>

      <Container>
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {impactStats.map((stat) => (
            <StatCard key={stat.label} {...stat} />
          ))}
        </div>
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-[0.95fr_1.05fr]">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <SectionHeading
              eyebrow="Payment Options"
              title="Give through the method that feels most convenient"
              description="The trust supports familiar, low-friction donation methods so supporters can contribute safely."
            />
            <div className="mt-6 grid gap-4">
              {donationMethods.map((method) => (
                <p key={method} className="rounded-2xl bg-blue-50 px-4 py-4 text-sm font-medium text-slate-700">
                  {method}
                </p>
              ))}
            </div>
          </div>

          <div className="grid gap-6">
            <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
              <SectionHeading
                eyebrow="Bank Details"
                title="Official donation account"
                description="Use these details for direct bank transfers and verified support."
              />
              <div className="mt-6 grid gap-3">
                {bankDetails.map((item) => (
                  <div key={item.label} className="flex items-center justify-between gap-4 rounded-2xl border border-blue-100 px-4 py-3 text-sm">
                    <span className="font-medium text-slate-500">{item.label}</span>
                    <span className="font-semibold text-slate-950">{item.value}</span>
                  </div>
                ))}
              </div>
            </div>

            <div className="grid gap-6 sm:grid-cols-2">
              <FeatureCard label="UPI" title="Fast digital transfer" description="A clean section for UPI IDs or QR code placement." />
              <FeatureCard label="QR Code" title="Scan and contribute" description="Reserve this panel for a scannable QR image or secure payment link." />
            </div>
          </div>
        </div>
      </Container>
    </div>
  );
}
