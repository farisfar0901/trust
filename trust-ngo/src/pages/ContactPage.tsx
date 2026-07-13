import { Container, SectionHeading } from "@/components/common";
import { contactDetails, socialLinks } from "@/constants/siteContent";

export default function ContactPage() {
  return (
    <div className="space-y-14 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow="Contact"
          title="Map, form, office details, and social links"
          description="The contact page stays direct and practical, with enough structure to support every visitor type."
        />
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-[1fr_0.95fr]">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
            <div className="h-72 rounded-[1.75rem] border border-blue-100 bg-[linear-gradient(135deg,rgba(29,78,216,0.16),rgba(255,255,255,0.98))] p-4">
              <div className="flex h-full items-center justify-center rounded-[1.4rem] border border-dashed border-blue-200 bg-white/60 text-center text-sm text-slate-500">
                Google Map Embed Placeholder
              </div>
            </div>
            <div className="mt-8 grid gap-3 sm:grid-cols-2">
              {contactDetails.map((item) => (
                <div key={item.label} className="rounded-2xl border border-blue-100 px-4 py-4">
                  <p className="text-xs font-semibold uppercase tracking-[0.22em] text-blue-700">{item.label}</p>
                  <p className="mt-2 text-sm leading-7 text-slate-700">{item.value}</p>
                </div>
              ))}
            </div>
          </div>

          <div className="grid gap-6">
            <div className="rounded-[2rem] border border-blue-100 bg-white p-8 shadow-soft">
              <p className="text-sm font-semibold uppercase tracking-[0.24em] text-blue-700">Contact Form</p>
              <div className="mt-6 grid gap-4">
                <div className="grid gap-4 sm:grid-cols-2">
                  <input type="text" placeholder="Your name" />
                  <input type="email" placeholder="Your email" />
                </div>
                <input type="text" placeholder="Subject" />
                <textarea rows={6} placeholder="Write your message" />
                <button type="button" className="rounded-full bg-blue-700 px-6 py-3 text-sm font-semibold text-white shadow-lg shadow-blue-200 transition hover:bg-blue-800">
                  Send message
                </button>
              </div>
            </div>

            <div className="rounded-[2rem] border border-blue-100 bg-slate-950 p-8 text-white shadow-soft">
              <p className="text-sm font-semibold uppercase tracking-[0.24em] text-sky-300">Social Media</p>
              <div className="mt-5 flex flex-wrap gap-3">
                {socialLinks.map((social) => (
                  <span key={social} className="rounded-full bg-white/5 px-4 py-2 text-sm font-medium text-slate-200">
                    {social}
                  </span>
                ))}
              </div>
            </div>
          </div>
        </div>
      </Container>
    </div>
  );
}
