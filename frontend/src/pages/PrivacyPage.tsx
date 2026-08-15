const points = [
  {
    title: 'Passwords',
    body: 'JobScout does not collect passwords or login credentials.',
  },
  {
    title: 'Temporary CV processing',
    body: 'CV uploads are processed in memory while a request is handled. JobScout V1 does not write uploaded files to disk or a database.',
  },
  {
    title: 'Verify listings',
    body: 'Job availability may change. Users should verify information on the original listing.',
  },
  {
    title: 'Minimal data',
    body: 'No unnecessary personal information should be collected.',
  },
]

export function PrivacyPage() {
  return (
    <div className="page-wrap max-w-3xl py-12 sm:py-16">
      <p className="eyebrow">Privacy</p>
      <h1 className="display mt-3 text-4xl text-ink">How JobScout handles information.</h1>
      <p className="mt-4 text-base leading-7 text-muted">
        The product is built to stay useful without storing a profile. Read this page before you upload a CV.
      </p>
      <div className="mt-6">
        {points.map((point, index) => (
          <section key={point.title} className={index === 0 ? 'pb-4' : 'border-t border-line py-4'}>
            <h2 className="text-sm font-semibold text-ink">{point.title}</h2>
            <p className="mt-1.5 text-[15px] leading-7 text-muted">{point.body}</p>
          </section>
        ))}
      </div>
    </div>
  )
}
