import { Container, FeatureCard, SectionHeading, StatCard } from "@/components/common";
import { adminStats, heroStats, newsPosts, upcomingEvents } from "@/constants/siteContent";

export default function AdminDashboardPage() {
  return (
    <div className="space-y-10 text-white">
      <Container className="px-0 lg:px-0">
        <div className="rounded-[2rem] border border-white/10 bg-white/5 p-8 shadow-soft">
          <p className="text-sm font-semibold uppercase tracking-[0.24em] text-sky-300">Dashboard Overview</p>
          <h1 className="mt-4 text-4xl font-semibold tracking-tight">Operations, reporting, and content management</h1>
          <p className="mt-4 max-w-3xl text-sm leading-7 text-slate-300">
            A polished admin home for tracking events, gallery assets, blog posts, volunteers, and donations.
          </p>
        </div>
      </Container>

      <Container className="px-0 lg:px-0">
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {adminStats.map((stat) => (
            <div key={stat.label} className="rounded-[1.75rem] border border-white/10 bg-white/5 p-6 shadow-soft">
              <p className="text-sm text-slate-400">{stat.label}</p>
              <p className="mt-3 text-3xl font-semibold">{stat.value}</p>
            </div>
          ))}
        </div>
      </Container>

      <Container className="px-0 lg:px-0">
        <div className="grid gap-6 xl:grid-cols-2">
          <div className="rounded-[2rem] border border-white/10 bg-white p-8 text-slate-950 shadow-soft">
            <SectionHeading
              eyebrow="Manage Events"
              title="Upcoming event queue"
              description="A compact management area for the latest outreach, volunteer, and donor events."
            />
            <div className="mt-6 grid gap-4">
              {upcomingEvents.map((event) => (
                <div key={event.title} className="rounded-2xl border border-slate-200 px-4 py-4">
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <p className="text-sm font-semibold text-blue-700">{event.date}</p>
                      <p className="mt-1 font-semibold text-slate-950">{event.title}</p>
                      <p className="mt-1 text-sm text-slate-600">{event.location}</p>
                    </div>
                    <span className="rounded-full bg-blue-50 px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-blue-700">
                      {event.status}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="rounded-[2rem] border border-white/10 bg-white p-8 text-slate-950 shadow-soft">
            <SectionHeading
              eyebrow="Manage Gallery"
              title="Featured assets and media status"
              description="The gallery area keeps photos and videos organized by year and publication state."
            />
            <div className="mt-6 grid gap-4 sm:grid-cols-2">
              {heroStats.slice(0, 4).map((stat) => (
                <StatCard key={stat.label} {...stat} />
              ))}
            </div>
          </div>
        </div>
      </Container>

      <Container className="px-0 lg:px-0">
        <div className="grid gap-6 xl:grid-cols-3">
          {newsPosts.map((post) => (
            <div key={post.title} className="rounded-[1.75rem] border border-white/10 bg-white p-6 text-slate-950 shadow-soft">
              <p className="text-xs font-semibold uppercase tracking-[0.22em] text-blue-700">Manage Blogs</p>
              <h3 className="mt-3 text-xl font-semibold">{post.title}</h3>
              <p className="mt-3 text-sm leading-7 text-slate-600">{post.excerpt}</p>
            </div>
          ))}
        </div>
      </Container>

      <Container className="px-0 lg:px-0">
        <div className="grid gap-6 xl:grid-cols-2">
          <FeatureCard label="Manage Volunteers" title="Applications and onboarding" description="A structured review queue can help track applicant status, training, and participation." />
          <FeatureCard label="Manage Donations" title="Donation ledger and reporting" description="The admin layer can summarize contributions, payment modes, and donor follow-ups." />
        </div>
      </Container>

      <Container className="px-0 lg:px-0">
        <div className="rounded-[2rem] border border-white/10 bg-white p-8 text-slate-950 shadow-soft">
          <SectionHeading
            eyebrow="Settings"
            title="Organization profile, branding, and access controls"
            description="Keep the trust identity, contact details, and operational preferences consistent from one dashboard."
          />
        </div>
      </Container>
    </div>
  );
}
