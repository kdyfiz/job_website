import { Link } from 'react-router-dom'
import type { Job } from '../types/job'
import {
  formatPostedDate,
  labelAvailability,
  labelEmployment,
  labelExperience,
  labelWork,
} from '../utils/labels'
import { CompanyMark } from './CompanyMark'
import { MatchScore } from './MatchScore'
import { SkillBadge } from './SkillBadge'

interface Props {
  job: Job
  showMatch?: boolean
}

export function JobCard({ job, showMatch = false }: Props) {
  const originalHref = !job.isDemoData && job.sourceUrl ? job.sourceUrl : null
  const meta = [
    job.location,
    labelWork(job.workArrangement),
    labelEmployment(job.employmentType),
    labelExperience(job.experienceLevel),
  ].filter(Boolean) as string[]

  return (
    <article className="card card-hover p-5 sm:p-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="flex min-w-0 gap-3">
          <CompanyMark name={job.company} />
          <div className="min-w-0">
            <div className="mb-1.5 flex flex-wrap items-center gap-2">
              {job.isDemoData && <span className="chip text-amber">Demo</span>}
              <span className="text-xs text-muted">{job.source}</span>
            </div>
            <h2 className="text-lg font-semibold tracking-tight text-ink">
              <Link to={`/jobs/${job.id}`} className="text-ink no-underline hover:text-brand">
                {job.title}
              </Link>
            </h2>
            <p className="mt-0.5 text-sm text-muted">{job.company}</p>
          </div>
        </div>
        {showMatch && job.estimatedMatchPercent != null && (
          <MatchScore percent={job.estimatedMatchPercent} compact />
        )}
      </div>

      <div className="mt-4 flex flex-wrap gap-1.5">
        {meta.map((item) => (
          <span key={item} className="chip">
            {item}
          </span>
        ))}
      </div>

      {job.salary?.display && <p className="mt-3 text-sm font-medium text-ink">{job.salary.display}</p>}

      <p className="mt-3 text-xs leading-5 text-muted">
        {formatPostedDate(job.postedDate) ?? 'Posted date unavailable'} · {labelAvailability(job.availabilityStatus)} ·
        Verify on original listing
      </p>

      {showMatch && job.match && (
        <div className="mt-4 grid gap-3 border-t border-line pt-4 text-sm">
          {job.match.matchingSkills.length > 0 && (
            <div className="flex flex-wrap items-center gap-1.5">
              <span className="text-xs font-medium text-muted">Matching</span>
              {job.match.matchingSkills.map((skill) => (
                <SkillBadge key={skill} skill={skill} tone="match" />
              ))}
            </div>
          )}
          {job.match.missingSkills.length > 0 && (
            <div className="flex flex-wrap items-center gap-1.5">
              <span className="text-xs font-medium text-muted">Potential gaps</span>
              {job.match.missingSkills.map((skill) => (
                <SkillBadge key={skill} skill={skill} tone="gap" />
              ))}
            </div>
          )}
        </div>
      )}

      <div className="mt-5 flex flex-wrap gap-2">
        <Link to={`/jobs/${job.id}`} className="btn-primary h-10">
          View details
        </Link>
        {originalHref && (
          <a href={originalHref} target="_blank" rel="noreferrer" className="btn-secondary h-10">
            View original
          </a>
        )}
      </div>
    </article>
  )
}
