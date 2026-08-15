import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { ErrorState } from '../components/ErrorState'
import { LoadingState } from '../components/LoadingState'
import { MatchScore } from '../components/MatchScore'
import { SkillBadge } from '../components/SkillBadge'
import { getJob } from '../services/api'
import type { Job } from '../types/job'
import {
  formatPostedDate,
  labelAvailability,
  labelEmployment,
  labelExperience,
  labelWork,
} from '../utils/labels'

export function JobDetailsPage() {
  const { id } = useParams()
  const [job, setJob] = useState<Job | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (!id) return
    let cancelled = false
    setLoading(true)
    getJob(id)
      .then((payload) => {
        if (!cancelled) setJob(payload)
      })
      .catch((err: unknown) => {
        if (!cancelled) setError(err instanceof Error ? err.message : 'We could not load this job.')
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [id])

  if (loading) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-8">
        <LoadingState message="Loading job details..." />
      </div>
    )
  }

  if (error || !job) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-8">
        <ErrorState message={error ?? 'We could not find that job listing.'} />
        <Link to="/jobs" className="mt-4 inline-block text-sm font-semibold text-brand">
          Back to results
        </Link>
      </div>
    )
  }

  const originalHref = job.isDemoData || !job.sourceUrl ? '/about#demo-data' : job.sourceUrl

  return (
    <div className="mx-auto max-w-3xl px-4 py-8">
      {job.isDemoData && (
        <p className="mb-4 rounded-lg bg-amber-50 px-3 py-2 text-sm text-amber-900">
          This is demo data, not a live job listing.
        </p>
      )}
      <p className="text-sm text-muted">{job.company}</p>
      <h1 className="mt-1 text-3xl font-semibold tracking-tight text-ink">{job.title}</h1>
      <p className="mt-3 text-slate-600">
        {job.location}
        {labelWork(job.workArrangement) ? ` · ${labelWork(job.workArrangement)}` : ''}
        {labelEmployment(job.employmentType) ? ` · ${labelEmployment(job.employmentType)}` : ''}
        {labelExperience(job.experienceLevel) ? ` · ${labelExperience(job.experienceLevel)}` : ''}
      </p>
      <p className="mt-2 text-sm text-muted">
        {formatPostedDate(job.postedDate) ?? 'Posted date unavailable'} · {labelAvailability(job.availabilityStatus)} ·
        Verify on original listing
      </p>

      {job.estimatedMatchPercent != null && (
        <div className="mt-4 max-w-xs">
          <MatchScore percent={job.estimatedMatchPercent} />
        </div>
      )}

      {job.salary?.display && (
        <p className="mt-4 text-sm font-medium text-ink">Salary: {job.salary.display}</p>
      )}

      <section className="mt-8">
        <h2 className="text-lg font-semibold">Description</h2>
        <p className="mt-2 whitespace-pre-wrap text-slate-700">{job.description}</p>
      </section>

      {job.skills.length > 0 && (
        <section className="mt-8">
          <h2 className="text-lg font-semibold">Required skills</h2>
          <div className="mt-3 flex flex-wrap gap-2">
            {job.skills.map((skill) => (
              <SkillBadge key={skill} skill={skill} />
            ))}
          </div>
        </section>
      )}

      <p className="mt-8 text-sm text-muted">Source: {job.source}</p>

      <a
        href={originalHref}
        target={job.isDemoData ? undefined : '_blank'}
        rel={job.isDemoData ? undefined : 'noreferrer'}
        className="mt-6 inline-flex h-11 items-center rounded-lg bg-brand px-5 text-sm font-semibold text-white no-underline hover:bg-brand-dark"
      >
        {job.isDemoData ? 'About demo listings' : 'View Original Job'}
      </a>
    </div>
  )
}
