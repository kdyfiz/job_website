import type { Job } from '../types/job'
import { JobCard } from './JobCard'

interface Props {
  jobs: Job[]
  showMatch?: boolean
}

export function JobList({ jobs, showMatch = false }: Props) {
  return (
    <div className="grid gap-4">
      {jobs.map((job) => (
        <JobCard key={job.id} job={job} showMatch={showMatch} />
      ))}
    </div>
  )
}
