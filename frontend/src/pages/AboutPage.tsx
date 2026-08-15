export function AboutPage() {
  return (
    <div className="page-wrap max-w-3xl py-12 sm:py-16">
      <p className="eyebrow">About</p>
      <h1 className="display mt-3 text-4xl text-ink">JobScout is a free job discovery tool.</h1>
      <p className="mt-5 text-base leading-7 text-muted">
        It is designed to make searching for relevant opportunities easier, then send you to the original listing to apply.
      </p>

      <div className="mt-10 grid gap-6 sm:grid-cols-2">
        <section className="card p-5">
          <h2 className="text-sm font-semibold text-ink">What it does</h2>
          <ul className="mt-3 grid gap-2 text-sm leading-6 text-muted">
            <li>Search jobs by title, location, and experience</li>
            <li>Filter and sort results</li>
            <li>Optionally match a PDF CV</li>
            <li>Show estimated skill overlap and gaps</li>
          </ul>
        </section>
        <section className="card p-5">
          <h2 className="text-sm font-semibold text-ink">What it does not do</h2>
          <ul className="mt-3 grid gap-2 text-sm leading-6 text-muted">
            <li>No saved profiles</li>
            <li>No guarantee of employment</li>
          </ul>
        </section>
      </div>

      <p className="mt-8 text-[15px] leading-7 text-muted">
        You are redirected to original job sources to apply.
      </p>
    </div>
  )
}
