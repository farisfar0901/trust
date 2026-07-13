import { useMemo, useState } from "react";
import { Container, FilterTabs, SectionHeading } from "@/components/common";
import { galleryPhotos, galleryVideos, galleryYears } from "@/constants/siteContent";

export default function GalleryPage() {
  const [activeYear, setActiveYear] = useState("All");
  const options = useMemo(() => ["All", ...galleryYears], []);
  const photos = useMemo(
    () => galleryPhotos.filter((photo) => activeYear === "All" || photo.year === activeYear),
    [activeYear],
  );
  const videos = useMemo(
    () => galleryVideos.filter((video) => activeYear === "All" || video.year === activeYear),
    [activeYear],
  );

  return (
    <div className="space-y-14 py-8 sm:py-10">
      <Container>
        <SectionHeading
          eyebrow="Gallery"
          title="Photos, videos, and year-based filtering"
          description="A gallery should feel calm and elegant, with filters that are obvious and content that is easy to scan."
        />
      </Container>

      <Container>
        <div className="rounded-[2rem] border border-blue-100 bg-white p-6 shadow-soft">
          <FilterTabs options={options} active={activeYear} onChange={setActiveYear} />

          <div className="mt-8 grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {photos.map((photo) => (
              <div key={photo.title} className="overflow-hidden rounded-[1.75rem] border border-amber-100 bg-slate-50 p-4">
                <div className="relative h-48 overflow-hidden rounded-[1.5rem] bg-[linear-gradient(135deg,rgba(217,119,6,0.16),rgba(255,255,255,0.96))]">
                  <img
                    src={photo.src}
                    alt={photo.title}
                    loading="lazy"
                    className="h-full w-full object-cover"
                  />
                </div>
                <p className="mt-4 text-xs font-semibold uppercase tracking-[0.2em] text-amber-700">{photo.year}</p>
                <h3 className="mt-2 text-lg font-semibold text-slate-950">{photo.title}</h3>
                <p className="mt-2 text-sm text-slate-600">{photo.type}</p>
              </div>
            ))}
          </div>

          <div className="mt-8 grid gap-4 md:grid-cols-2">
            {videos.map((video) => (
              <div key={video.title} className="rounded-[1.75rem] border border-blue-100 bg-slate-950 p-6 text-white shadow-soft">
                <p className="text-xs font-semibold uppercase tracking-[0.2em] text-sky-300">Video</p>
                <h3 className="mt-3 text-xl font-semibold">{video.title}</h3>
                <p className="mt-2 text-sm text-slate-300">Duration: {video.duration}</p>
              </div>
            ))}
          </div>
        </div>
      </Container>
    </div>
  );
}
