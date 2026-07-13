import { Container, SectionHeading } from "@/components/common";
import { volunteerBenefits, volunteerFaq } from "@/constants/siteContent";

export default function VolunteerPage() {
  return (
    <div className="space-y-14 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow="Volunteer"
          title="A registration experience designed for trust and clarity"
          description="This page pairs a clean registration form with benefits and FAQ content so people can decide quickly."
        />
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-[0.95fr_1.05fr]">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <p className="text-sm font-semibold uppercase tracking-[0.24em] text-blue-700">Registration Form</p>
            <div className="mt-6 grid gap-4">
              <div className="grid gap-4 sm:grid-cols-2">
                <input type="text" placeholder="Full name" />
                <input type="email" placeholder="Email address" />
              </div>
              <div className="grid gap-4 sm:grid-cols-2">
                <input type="tel" placeholder="Phone number" />
                <input type="text" placeholder="City / location" />
              </div>
              <select defaultValue="">
                <option value="" disabled>
                  Preferred volunteering area
                </option>
                <option>Education</option>
                <option>Health</option>
                <option>Relief</option>
                <option>Events</option>
              </select>
              <textarea rows={5} placeholder="Tell us about your interests and availability" />
              <button type="button" className="rounded-full bg-blue-700 px-6 py-3 text-sm font-semibold text-white shadow-lg shadow-blue-200 transition hover:bg-blue-800">
                Submit volunteer application
              </button>
            </div>
          </div>

          <div className="grid gap-6">
            <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
              <SectionHeading
                eyebrow="Benefits"
                title="Why volunteers join the trust"
                description="Clear benefits make the volunteer path feel organized and worthwhile."
              />
              <div className="mt-6 grid gap-4">
                {volunteerBenefits.map((benefit) => (
                  <p key={benefit} className="rounded-2xl bg-blue-50 px-4 py-4 text-sm leading-7 text-slate-700">
                    {benefit}
                  </p>
                ))}
              </div>
            </div>

            <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
              <SectionHeading
                eyebrow="FAQ"
                title="Common questions answered simply"
                description="A short FAQ reduces friction and improves conversion for volunteer signups."
              />
              <div className="mt-6 grid gap-4">
                {volunteerFaq.map((item) => (
                  <div key={item.question} className="rounded-2xl border border-blue-100 p-4">
                    <p className="font-semibold text-slate-950">{item.question}</p>
                    <p className="mt-2 text-sm leading-7 text-slate-600">{item.answer}</p>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </Container>
    </div>
  );
}
