import { useState } from 'react'
import { CVAnalysisPanel } from '../components/CVAnalysis'
import { CVUpload } from '../components/CVUpload'
import { EmptyState } from '../components/EmptyState'
import { ErrorState } from '../components/ErrorState'
import { JobFilters } from '../components/JobFilters'
import { JobList } from '../components/JobList'
import { LoadingState } from '../components/LoadingState'
import { LocationPicker } from '../components/LocationPicker'
import { defaultSearchParams, matchJobs } from '../services/api'
import type { JobMatchResponse, SearchParams } from '../types/job'

export function MatchPage() {
  const [file, setFile] = useState<File | null>(null)
  const [params, setParams] = useState<SearchParams>(defaultSearchParams())
  const [loadingMessage, setLoadingMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [result, setResult] = useState<JobMatchResponse | null>(null)

  async function runMatch(next = params) {
    if (!file) {
      setError('Please upload a PDF CV under 5 MB.')
      return
    }

    setError(null)
    setLoadingMessage('Reading your CV. The first request can take about a minute if the API is waking up.')
    try {
      await new Promise((resolve) => setTimeout(resolve, 250))
      setLoadingMessage('Matching your skills with available jobs...')
      const payload = await matchJobs(file, { ...next, query: '' })
      setResult(payload)
      setParams({ ...next, query: '', sort: 'HighestMatch' })
    } catch (err: unknown) {
      setResult(null)
      setError(err instanceof Error ? err.message : "We couldn't process your CV.")
    } finally {
      setLoadingMessage(null)
    }
  }

  return (
    <div className="page-wrap py-10 sm:py-12">
      <div className="max-w-2xl">
        <p className="eyebrow">Optional</p>
        <h1 className="display mt-3 text-4xl text-ink">Match my CV</h1>
        <p className="mt-3 text-base leading-7 text-muted">
          Upload a PDF. Search defaults to Malaysia; choose up to 3 states to narrow. Skills are matched in memory and the file is discarded.
          Match scores are estimates, not a hiring decision.
        </p>
      </div>

      <div className="mt-8 grid gap-4 lg:grid-cols-[1.1fr_0.9fr]">
        <CVUpload onFile={setFile} disabled={Boolean(loadingMessage)} />
        <div className="card flex flex-col justify-center p-5 sm:p-6">
          <p className="label">Location</p>
          <div className="mt-1.5">
            <LocationPicker
              value={params.location}
              onChange={(location) => setParams({ ...params, location })}
            />
          </div>
          <button
            type="button"
            onClick={() => void runMatch()}
            className="btn-primary mt-4"
            disabled={!file || Boolean(loadingMessage)}
          >
            Search based on my CV
          </button>
          {!file && <p className="mt-3 text-sm text-muted">Upload a PDF first, then search.</p>}
        </div>
      </div>

      {loadingMessage && (
        <div className="mt-8">
          <LoadingState message={loadingMessage} />
        </div>
      )}
      {error && (
        <div className="mt-8">
          <ErrorState message={error} />
        </div>
      )}

      {result && !loadingMessage && (
        <div className="mt-10 grid gap-6 lg:grid-cols-[260px_1fr]">
          <JobFilters value={params} onChange={(next) => void runMatch(next)} showMatchFilter />
          <div className="grid gap-4">
            <CVAnalysisPanel analysis={result.cv} />
            <div className="flex flex-col gap-1 sm:flex-row sm:items-end sm:justify-between">
              <h2 className="text-xl font-semibold tracking-tight text-ink">Recommended jobs</h2>
              <p className="text-sm text-muted">Sorted by estimated match</p>
            </div>
            {result.results.usingDemoData ? (
              <p className="notice">Recommended jobs below are demo data, not live listings.</p>
            ) : (
              <p className="notice">Recommended jobs are remote listings from Himalayas. Apply on the original posting.</p>
            )}
            {result.results.jobs.length === 0 ? (
              <EmptyState title="No recommended jobs.">
                Try fewer filters or another location.
              </EmptyState>
            ) : (
              <JobList jobs={result.results.jobs} showMatch />
            )}
          </div>
        </div>
      )}
    </div>
  )
}
