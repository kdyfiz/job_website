import { Link, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { SearchForm } from '../components/SearchForm'
import { defaultSearchParams } from '../services/api'
import type { SearchParams } from '../types/job'
import { jobsPath } from '../utils/searchParams'

const steps = [
  {
    step: '01',
    title: 'Search',
    body: 'Enter a role, location, and experience level. Results are filtered from available listings.',
  },
  {
    step: '02',
    title: 'Match',
    body: 'Optionally upload a PDF CV. Skills are detected in memory and scored against each job.',
  },
  {
    step: '03',
    title: 'Apply',
    body: 'Open the original listing. JobScout does not host applications.',
  },
]

export function HomePage() {
  const navigate = useNavigate()
  const [params, setParams] = useState<SearchParams>(defaultSearchParams())

  function search(next: SearchParams) {
    navigate(jobsPath(next))
  }

  return (
    <div>
      <section className="relative overflow-hidden border-b border-line">
        <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_top,_rgb(18_84_78_/_0.07),_transparent_58%)]" />
        <div className="page-wrap relative py-16 sm:py-20 lg:py-24">
          <div className="mx-auto max-w-2xl text-center">
            <p className="eyebrow">Search · Match · Apply</p>
            <h1 className="display mt-4 text-4xl text-ink sm:text-5xl lg:text-[3.55rem]">
              Find jobs that fit your skills.
            </h1>
            <p className="mx-auto mt-5 max-w-xl text-base leading-7 text-muted sm:text-lg">
              Search opportunities, estimate a skill match from a PDF CV, then apply on the original listing.
            </p>
          </div>
          <div className="card mx-auto mt-10 max-w-3xl p-5 shadow-[0_16px_40px_rgb(22_21_19_/_0.05)] sm:p-7">
            <SearchForm value={params} onChange={setParams} onSubmit={search} />
          </div>
        </div>
      </section>

      <section className="page-wrap py-16 sm:py-20">
        <div className="max-w-xl">
          <p className="eyebrow">How it works</p>
          <h2 className="display mt-3 text-3xl text-ink sm:text-4xl">Search, match, then apply at the source.</h2>
        </div>
        <div className="mt-12 grid gap-8 md:grid-cols-3">
          {steps.map((item) => (
            <div key={item.step} className="border-t border-line pt-6">
              <p className="display text-3xl text-brand/55">{item.step}</p>
              <h3 className="mt-4 text-base font-semibold text-ink">{item.title}</h3>
              <p className="mt-2 text-sm leading-6 text-muted">{item.body}</p>
            </div>
          ))}
        </div>
      </section>

      <section className="border-y border-line bg-brand-dark text-paper">
        <div className="page-wrap grid gap-8 py-16 sm:py-20 lg:grid-cols-[1.15fr_0.85fr] lg:items-center">
          <div>
            <p className="text-[11px] font-semibold uppercase tracking-[0.18em] text-white/45">Optional</p>
            <h2 className="display mt-3 text-3xl sm:text-4xl">See which roles match your CV.</h2>
            <p className="mt-4 max-w-xl text-sm leading-7 text-white/70 sm:text-base">
              Upload a PDF to estimate skill overlap and gaps. The file is processed for that request only and is not
              stored. Scores are estimates — not a hiring decision.
            </p>
          </div>
          <div className="rounded-2xl bg-white p-6 text-ink sm:p-7">
            <h3 className="text-base font-semibold">Match in one upload</h3>
            <p className="mt-2 text-sm leading-6 text-muted">PDF only, 5 MB maximum. Discarded after the request.</p>
            <Link to="/match" className="btn-primary mt-6 h-12 px-6">
              Match My CV
            </Link>
          </div>
        </div>
      </section>
    </div>
  )
}
