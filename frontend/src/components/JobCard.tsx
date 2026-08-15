import { Link } from 'react-router-dom'
import type { Job } from '../types/job'
import {
  formatPostedDate,
  labelAvailability,
  labelEmployment,
  labelExperience,
  labelWork,
} from '../utils/labels'
import { MatchScore } from './MatchScore'
import { SkillBadge } from './SkillBadge'

interface Props {
  job: Job
  showMatch?: boolean
}

export function JobCard({ job, showMatch = false }: Props) {
  const originalHref = job.isDemoData || !job.sourceUrl ? '/about#demo-data' : job.sourceUrl
  const originalLabel = job.isDemoData ? 'Demo listing' : 'View Original Job'

  return (
    <article className="rounded-2xl border border-line bg-white p-5">
      <div className="flex flex-col gap-4 sm:flex-row sm:justify-between">
        <div className="min-w-0">
          <div className="mb-2 flex flex-wrap items-center gap-2">
            {job.isDemoData && (
              <span className="rounded-full bg-amber-50 px-2 py-0.5 text-[11px] font-semibold uppercase tracking-wide text-amber-800">
                Demo data
              </span>
            )}
            <span className="text-xs text-muted">{job.source}</span>
          </div>
          <h2 className="text-lg font-semibold text-ink">{job.title}</h2>
          <p className="mt-1 text-sm text-slate-600">{job.company}</p>
          <p className="mt-3 flex flex-wrap gap-x-3 gap-y-1 text-sm text-slate-600">
            <span>{job.location}</span>
            {labelWork(job.workArrangement) && <span>{labelWork(job.workArrangement)}</span>}
            {labelEmployment(job.employmentType) && <span>{labelEmployment(job.employmentType)}</span>}
            {labelExperience(job.experienceLevel) && <span>{labelExperience(job.experienceLevel)}</span>}
          </p>
          <p className="mt-2 text-xs text-muted">
            {formatPostedDate(job.postedDate) ?? 'Posted date unavailable'} · {labelAvailability(job.availabilityStatus)} ·
            Verify on original listing
          </p>
        </div>
        {showMatch && job.estimatedMatchPercent != null && (
          <MatchScore percent={job.estimatedMatchPercent} compact />
        )}
      </div>

      {showMatch && job.match && (
        <div className="mt-4 grid gap-3 text-sm">
          {job.match.matchingSkills.length > 0 && (
            <p>
              <span className="font-medium text-slate-700">Skills: </span>
              {job.match.matchingSkills.map((skill) => (
                <SkillBadge key={skill} skill={skill} tone="match" />
              ))}
            </p>
          )}
          {job.match.missingSkills.length > 0 && (
            <p className="flex flex-wrap items-center gap-1">
              <span className="font-medium text-slate-700">Potential gaps:</span>
              {job.match.missingSkills.map((skill) => (
                <SkillBadge key={skill} skill={skill} tone="gap" />
              ))}
            </p>
          )}
        </div>
      )}

      <div className="mt-5 flex flex-wrap gap-2">
        <Link
          to={`/jobs/${job.id}`}
          className="inline-flex h-10 items-center rounded-lg bg-brand px-4 text-sm font-semibold text-white no-underline hover:bg-brand-dark"
        >
          View Details
        </Link>
        <a
          href={originalHref}
          target={job.isDemoData ? undefined : '_blank'}
          rel={job.isDemoData ? undefined : 'noreferrer'}
          className="inline-flex h-10 items-center rounded-lg border border-line px-4 text-sm font-semibold text-slate-700 no-underline hover:bg-slate-50"
        >
          {originalLabel}
        </a>
      </div>
    </article>
  )
}
