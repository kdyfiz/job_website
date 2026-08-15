import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { CompanyMark } from '../components/CompanyMark'
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
      <div className="page-wrap max-w-5xl py-10">
        <LoadingState message="Loading job details..." />
      </div>
    )
  }

  if (error || !job) {
    return (
      <div className="page-wrap max-w-3xl py-10">
        <ErrorState message={error ?? 'We could not find that job listing.'} />
        <Link to="/jobs" className="btn-ghost mt-4">
          Back to results
        </Link>
      </div>
    )
  }

  const originalHref = !job.isDemoData && job.sourceUrl ? job.sourceUrl : null
  const meta = [
    job.location,
    labelWork(job.workArrangement),
    labelEmployment(job.employmentType),
    labelExperience(job.experienceLevel),
  ].filter(Boolean) as string[]

  return (
    <div className="page-wrap max-w-5xl py-10">
      <Link to="/jobs" className="text-sm font-medium text-muted no-underline hover:text-ink">
        ← Back to results
      </Link>

      {job.isDemoData && <p className="notice mt-5">This is demo data, not a live job listing.</p>}

      <div className="mt-8 grid gap-8 lg:grid-cols-[1fr_280px] lg:items-start">
        <div>
          <div className="flex gap-4">
            <CompanyMark name={job.company} className="h-12 w-12 text-base" />
            <div>
              <p className="text-sm font-medium text-muted">{job.company}</p>
              <h1 className="display mt-1 text-3xl text-ink sm:text-4xl">{job.title}</h1>
            </div>
          </div>

          <div className="mt-5 flex flex-wrap gap-1.5">
            {meta.map((item) => (
              <span key={item} className="chip">
                {item}
              </span>
            ))}
          </div>

          <p className="mt-4 text-sm text-muted">
            {formatPostedDate(job.postedDate) ?? 'Posted date unavailable'} · {labelAvailability(job.availabilityStatus)}{' '}
            · Verify on original listing
          </p>

          <section className="mt-10 border-t border-line pt-8">
            <h2 className="text-sm font-semibold uppercase tracking-[0.14em] text-muted">Description</h2>
            <p className="mt-3 whitespace-pre-wrap text-[15px] leading-7 text-ink/90">{job.description}</p>
          </section>

          {job.skills.length > 0 && (
            <section className="mt-10">
              <h2 className="text-sm font-semibold uppercase tracking-[0.14em] text-muted">Required skills</h2>
              <div className="mt-3 flex flex-wrap gap-2">
                {job.skills.map((skill) => (
                  <SkillBadge key={skill} skill={skill} />
                ))}
              </div>
            </section>
          )}
        </div>

        <aside className="card h-fit p-5 lg:sticky lg:top-24">
          {job.estimatedMatchPercent != null && (
            <div className="mb-4">
              <MatchScore percent={job.estimatedMatchPercent} />
            </div>
          )}
          {job.salary?.display && (
            <p className="text-sm font-semibold text-ink">{job.salary.display}</p>
          )}
          <p className="mt-2 text-sm text-muted">Source: {job.source}</p>
          {originalHref && (
            <a href={originalHref} target="_blank" rel="noreferrer" className="btn-primary mt-5 w-full">
              View original job
            </a>
          )}
          <p className="mt-3 text-xs leading-5 text-muted">
            JobScout does not host applications. Always confirm details on the original listing.
          </p>
        </aside>
      </div>
    </div>
  )
}
