import { useEffect, useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { EmptyState } from '../components/EmptyState'
import { ErrorState } from '../components/ErrorState'
import { JobFilters } from '../components/JobFilters'
import { JobList } from '../components/JobList'
import { JobSort } from '../components/JobSort'
import { LoadingState } from '../components/LoadingState'
import { SearchForm } from '../components/SearchForm'
import { searchJobs } from '../services/api'
import type { JobSearchResponse, SearchParams } from '../types/job'
import { jobsPath, paramsFromUrl } from '../utils/searchParams'

export function JobsPage() {
  const [urlParams] = useSearchParams()
  const navigate = useNavigate()
  const queryKey = urlParams.toString()
  const [params, setParams] = useState<SearchParams>(() => paramsFromUrl(urlParams))
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [results, setResults] = useState<JobSearchResponse | null>(null)

  useEffect(() => {
    const next = paramsFromUrl(new URLSearchParams(queryKey))
    setParams(next)

    if (!next.query.trim()) {
      setResults(null)
      setError('Please enter a job title or keywords.')
      return
    }

    let cancelled = false
    setLoading(true)
    setError(null)

    searchJobs(next)
      .then((payload) => {
        if (!cancelled) setResults(payload)
      })
      .catch((err: unknown) => {
        if (!cancelled) {
          setError(err instanceof Error ? err.message : 'Something went wrong while searching for jobs.')
        }
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [queryKey])

  function apply(next: SearchParams) {
    setParams(next)
    navigate(jobsPath(next))
  }

  const summary = [params.query, params.location].filter(Boolean).join(' · ')

  return (
    <div className="page-wrap py-8 sm:py-10">
      <div className="card p-5 shadow-[0_10px_28px_rgb(22_21_19_/_0.04)] sm:p-6">
        <SearchForm value={params} onChange={setParams} onSubmit={apply} compact />
      </div>

      {results?.usingDemoData && (
        <p className="notice mt-4">
          These results are labelled demo data. They are sample roles for the product, not live vacancies.
        </p>
      )}

      <div className="mt-8 grid gap-6 lg:grid-cols-[260px_1fr]">
        <JobFilters value={params} onChange={apply} />
        <div>
          <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <p className="eyebrow">Results</p>
              <h1 className="display text-3xl text-ink">
                {results ? `${results.total} roles` : 'Job results'}
              </h1>
              {summary && <p className="mt-1 text-sm text-muted">{summary}</p>}
            </div>
            <JobSort value={params.sort} onChange={(sort) => apply({ ...params, sort })} />
          </div>
          {loading && <LoadingState />}
          {!loading && error && <ErrorState message={error} />}
          {!loading && !error && results && results.jobs.length === 0 && <EmptyState />}
          {!loading && !error && results && results.jobs.length > 0 && <JobList jobs={results.jobs} />}
        </div>
      </div>
    </div>
  )
}
