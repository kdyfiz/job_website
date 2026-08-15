import { useState } from 'react'
import { CVAnalysisPanel } from '../components/CVAnalysis'
import { CVUpload } from '../components/CVUpload'
import { EmptyState } from '../components/EmptyState'
import { ErrorState } from '../components/ErrorState'
import { JobFilters } from '../components/JobFilters'
import { JobList } from '../components/JobList'
import { LoadingState } from '../components/LoadingState'
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
    setLoadingMessage('Reading your CV...')
    try {
      await new Promise((resolve) => setTimeout(resolve, 250))
      setLoadingMessage('Matching your skills with available jobs...')
      const payload = await matchJobs(file, next)
      setResult(payload)
      setParams({ ...next, sort: 'HighestMatch' })
    } catch (err: unknown) {
      setResult(null)
      setError(err instanceof Error ? err.message : "We couldn't process your CV.")
    } finally {
      setLoadingMessage(null)
    }
  }

  return (
    <div className="mx-auto max-w-6xl px-4 py-8">
      <h1 className="text-3xl font-semibold tracking-tight text-ink">Match my CV</h1>
      <p className="mt-2 max-w-2xl text-slate-600">
        Upload a PDF. JobScout extracts skills locally in the API process, scores demo jobs, then discards the file.
      </p>

      <div className="mt-6 grid gap-4 lg:grid-cols-[1fr_1fr]">
        <CVUpload onFile={setFile} disabled={Boolean(loadingMessage)} />
        <div className="rounded-2xl border border-line bg-white p-5">
          <h2 className="text-sm font-semibold text-ink">Optional search hints</h2>
          <div className="mt-3 grid gap-3">
            <input
              value={params.query}
              onChange={(e) => setParams({ ...params, query: e.target.value })}
              placeholder="Job title / keywords (optional)"
              className="h-11 rounded-lg border border-line px-3"
            />
            <input
              value={params.location}
              onChange={(e) => setParams({ ...params, location: e.target.value })}
              placeholder="Location (optional)"
              className="h-11 rounded-lg border border-line px-3"
            />
            <button
              type="button"
              onClick={() => void runMatch()}
              className="h-11 rounded-lg bg-brand text-sm font-semibold text-white hover:bg-brand-dark"
            >
              Match My CV
            </button>
          </div>
        </div>
      </div>

      <p className="mt-4 text-sm text-muted">
        Match scores are estimates based on information detected from your CV and the job listing. They are not a
        guarantee of suitability or employment.
      </p>

      {loadingMessage && (
        <div className="mt-6">
          <LoadingState message={loadingMessage} />
        </div>
      )}
      {error && (
        <div className="mt-6">
          <ErrorState message={error} />
        </div>
      )}

      {result && !loadingMessage && (
        <div className="mt-8 grid gap-6 lg:grid-cols-[240px_1fr]">
          <JobFilters value={params} onChange={(next) => void runMatch(next)} showMatchFilter />
          <div className="grid gap-4">
            <CVAnalysisPanel analysis={result.cv} />
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <h2 className="text-xl font-semibold text-ink">Recommended jobs</h2>
              <p className="text-sm text-muted">Sorted by estimated match</p>
            </div>
            {result.results.usingDemoData && (
              <p className="rounded-lg bg-amber-50 px-3 py-2 text-sm text-amber-900">
                Recommended jobs below are demo data, not live listings.
              </p>
            )}
            {result.results.jobs.length === 0 ? (
              <EmptyState title="No recommended jobs.">
                Try fewer filters or a broader keyword.
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
