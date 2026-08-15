export function AboutPage() {
  return (
    <div className="mx-auto max-w-3xl px-4 py-10">
      <h1 className="text-3xl font-semibold tracking-tight text-ink">About JobScout</h1>
      <p className="mt-4 text-slate-700">
        JobScout is a free job discovery tool designed to make searching for relevant job opportunities easier.
      </p>
      <ul className="mt-6 grid gap-2 text-slate-700">
        <li>Search jobs</li>
        <li>Filter results</li>
        <li>Optional CV matching</li>
        <li>Estimated skill matching</li>
        <li>No account required</li>
        <li>Free to use</li>
      </ul>
      <p className="mt-6 text-slate-700">
        Users are redirected to original job sources to apply. JobScout does not provide an internal application system.
      </p>
      <section id="demo-data" className="mt-10 scroll-mt-24 rounded-2xl border border-amber-200 bg-amber-50 p-5">
        <h2 className="text-lg font-semibold text-ink">Demo listings</h2>
        <p className="mt-2 text-sm text-slate-700">
          V1 ships with a Demo job provider. Sample roles are clearly labelled as demo data so the product can be used
          and reviewed without paid job APIs. They are not live vacancies. Always treat the original source as the
          authority when live providers are added later.
        </p>
      </section>
    </div>
  )
}
