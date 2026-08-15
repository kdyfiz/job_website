import { useEffect, useMemo, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { EmptyState } from '../components/EmptyState'
import { ErrorState } from '../components/ErrorState'
import { JobFilters } from '../components/JobFilters'
import { JobList } from '../components/JobList'
import { JobSort } from '../components/JobSort'
import { LoadingState } from '../components/LoadingState'
import { SearchForm } from '../components/SearchForm'
import { defaultSearchParams, searchJobs } from '../services/api'
import type { JobSearchResponse, SearchParams } from '../types/job'

export function JobsPage() {
  const [urlParams, setUrlParams] = useSearchParams()
  const [params, setParams] = useState<SearchParams>(paramsFromUrl(urlParams))
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [results, setResults] = useState<JobSearchResponse | null>(null)

  const queryKey = useMemo(() => urlParams.toString(), [urlParams])

  useEffect(() => {
    const next = paramsFromUrl(urlParams)
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
        if (!cancelled) setError(err instanceof Error ? err.message : 'Something went wrong while searching for jobs.')
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [queryKey, urlParams])

  function apply(next: SearchParams) {
    setParams(next)
    setUrlParams(toUrl(next))
  }

  return (
    <div className="mx-auto max-w-6xl px-4 py-8">
      <SearchForm value={params} onChange={setParams} onSubmit={() => apply(params)} compact />

      {results?.usingDemoData && (
        <p className="mt-4 rounded-lg bg-amber-50 px-3 py-2 text-sm text-amber-900">
          These results are labelled demo data. They are not live job listings.
        </p>
      )}

      <div className="mt-6 grid gap-6 lg:grid-cols-[240px_1fr]">
        <JobFilters value={params} onChange={apply} />
        <div>
          <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <h1 className="text-xl font-semibold text-ink">
              {results ? `${results.total} jobs found` : 'Job results'}
            </h1>
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

function paramsFromUrl(urlParams: URLSearchParams): SearchParams {
  const defaults = defaultSearchParams()
  return {
    ...defaults,
    query: urlParams.get('query') ?? '',
    location: urlParams.get('location') ?? '',
    experienceLevel: (urlParams.get('experienceLevel') as SearchParams['experienceLevel']) ?? 'Any',
    workArrangement: (urlParams.get('workArrangement') as SearchParams['workArrangement']) ?? 'Any',
    employmentType: (urlParams.get('employmentType') as SearchParams['employmentType']) ?? 'Any',
    datePosted: (urlParams.get('datePosted') as SearchParams['datePosted']) ?? 'Any',
    sort: (urlParams.get('sort') as SearchParams['sort']) ?? 'MostRelevant',
  }
}

function toUrl(params: SearchParams) {
  const query = new URLSearchParams()
  if (params.query) query.set('query', params.query)
  if (params.location) query.set('location', params.location)
  if (params.experienceLevel !== 'Any') query.set('experienceLevel', params.experienceLevel)
  if (params.workArrangement !== 'Any') query.set('workArrangement', params.workArrangement)
  if (params.employmentType !== 'Any') query.set('employmentType', params.employmentType)
  if (params.datePosted !== 'Any') query.set('datePosted', params.datePosted)
  if (params.sort !== 'MostRelevant') query.set('sort', params.sort)
  return query
}
