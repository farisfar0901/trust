import { useMemo, useState } from "react";
import { Container, FilterTabs, FeatureCard, SectionHeading } from "@/components/common";
import { blogCategories, blogPosts, recentPosts } from "@/constants/siteContent";

export default function BlogsPage() {
  const [activeCategory, setActiveCategory] = useState("All");
  const options = useMemo(() => ["All", ...blogCategories], []);
  const posts = useMemo(
    () => blogPosts.filter((post) => activeCategory === "All" || post.category === activeCategory),
    [activeCategory],
  );

  return (
    <div className="space-y-14 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow="Blogs"
          title="Readable content for donors, volunteers, and the public"
          description="The blog area emphasizes clean cards, category filters, and recent-post visibility."
        />
      </Container>

      <Container>
        <div className="grid gap-6 lg:grid-cols-[1fr_0.42fr]">
          <div className="rounded-[2rem] border border-blue-100 bg-white p-6 shadow-soft">
            <FilterTabs options={options} active={activeCategory} onChange={setActiveCategory} />
            <div className="mt-6 grid gap-6 md:grid-cols-2">
              {posts.map((post) => (
                <FeatureCard key={post.title} label={post.date} title={post.title} description={post.excerpt} />
              ))}
            </div>
          </div>

          <aside className="rounded-[2rem] border border-blue-100 bg-white p-6 shadow-soft">
            <p className="text-sm font-semibold uppercase tracking-[0.24em] text-blue-700">Categories</p>
            <div className="mt-4 flex flex-wrap gap-2">
              {blogCategories.map((category) => (
                <span key={category} className="rounded-full bg-blue-50 px-4 py-2 text-sm font-semibold text-blue-800">
                  {category}
                </span>
              ))}
            </div>
            <div className="mt-8 rounded-[1.5rem] bg-slate-950 p-5 text-white">
              <p className="text-sm font-semibold uppercase tracking-[0.24em] text-sky-300">Recent Posts</p>
              <div className="mt-4 grid gap-3 text-sm text-slate-300">
                {recentPosts.map((post) => (
                  <p key={post} className="rounded-2xl bg-white/5 px-4 py-3">
                    {post}
                  </p>
                ))}
              </div>
            </div>
          </aside>
        </div>
      </Container>
    </div>
  );
}
