import { Link, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { SearchForm } from '../components/SearchForm'
import { defaultSearchParams } from '../services/api'
import type { SearchParams } from '../types/job'

export function HomePage() {
  const navigate = useNavigate()
  const [params, setParams] = useState<SearchParams>(defaultSearchParams())

  function search() {
    const query = new URLSearchParams()
    query.set('query', params.query.trim())
    if (params.location.trim()) query.set('location', params.location.trim())
    if (params.experienceLevel !== 'Any') query.set('experienceLevel', params.experienceLevel)
    if (params.workArrangement !== 'Any') query.set('workArrangement', params.workArrangement)
    navigate(`/jobs?${query.toString()}`)
  }

  return (
    <div>
      <section className="bg-white">
        <div className="mx-auto grid max-w-6xl gap-10 px-4 py-14 lg:grid-cols-[1.1fr_0.9fr] lg:items-center">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.18em] text-brand">JobScout</p>
            <h1 className="mt-3 max-w-xl text-4xl font-semibold tracking-tight text-ink sm:text-5xl">
              Find your next job.
            </h1>
            <p className="mt-4 max-w-xl text-lg text-slate-600">
              Search job opportunities and discover roles that match your skills — no account required.
            </p>
          </div>
          <div className="rounded-2xl border border-line bg-surface p-5 shadow-sm">
            <SearchForm value={params} onChange={setParams} onSubmit={search} />
          </div>
        </div>
      </section>

      <section className="border-t border-line">
        <div className="mx-auto max-w-6xl px-4 py-12">
          <div className="rounded-2xl bg-brand px-6 py-10 text-white sm:px-10">
            <h2 className="text-2xl font-semibold">Already have a CV?</h2>
            <p className="mt-2 max-w-2xl text-teal-50">
              Upload your CV and discover jobs that match your skills.
            </p>
            <ul className="mt-4 grid gap-1 text-sm text-teal-100 sm:grid-cols-2">
              <li>No account required.</li>
              <li>CV is processed temporarily.</li>
              <li>CV is not intended to be permanently stored.</li>
              <li>Job matching is an estimate. Always verify the original listing.</li>
            </ul>
            <Link
              to="/match"
              className="mt-6 inline-flex h-11 items-center rounded-lg bg-white px-5 text-sm font-semibold text-brand-dark no-underline"
            >
              Match My CV
            </Link>
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-6xl px-4 py-12">
        <h2 className="text-2xl font-semibold text-ink">How it works</h2>
        <div className="mt-6 grid gap-4 md:grid-cols-3">
          {[
            { step: '1. Search', body: 'Enter a job title, location, and experience level.' },
            { step: '2. Match', body: 'Optionally upload a CV to see estimated skill matches and gaps.' },
            { step: '3. Apply', body: 'Open the original listing. JobScout does not host applications.' },
          ].map((item) => (
            <div key={item.step} className="rounded-2xl border border-line bg-white p-5">
              <h3 className="font-semibold text-ink">{item.step}</h3>
              <p className="mt-2 text-sm text-slate-600">{item.body}</p>
            </div>
          ))}
        </div>
        <div className="mt-8 flex flex-wrap gap-6 text-sm font-medium text-slate-600">
          <span>Free</span>
          <span>No account</span>
          <span>Temporary CV processing</span>
        </div>
      </section>
    </div>
  )
}
