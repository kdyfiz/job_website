import type {
  AvailabilityStatus,
  DatePostedFilter,
  EmploymentType,
  ExperienceLevel,
  JobSortOption,
  MatchScoreFilter,
  WorkArrangement,
} from '../types/job'

export const experienceOptions: { value: ExperienceLevel; label: string }[] = [
  { value: 'Any', label: 'Any' },
  { value: 'Internship', label: 'Internship' },
  { value: 'FreshGraduate', label: 'Fresh Graduate' },
  { value: 'EntryLevel', label: 'Entry Level' },
  { value: 'OneToTwoYears', label: '1–2 Years' },
  { value: 'ThreeToFiveYears', label: '3–5 Years' },
]

export const workOptions: { value: WorkArrangement; label: string }[] = [
  { value: 'Any', label: 'Any' },
  { value: 'Remote', label: 'Remote' },
  { value: 'Hybrid', label: 'Hybrid' },
  { value: 'OnSite', label: 'On-site' },
]

export const employmentOptions: { value: EmploymentType; label: string }[] = [
  { value: 'Any', label: 'Any' },
  { value: 'FullTime', label: 'Full-time' },
  { value: 'PartTime', label: 'Part-time' },
  { value: 'Internship', label: 'Internship' },
  { value: 'Contract', label: 'Contract' },
]

export const dateOptions: { value: DatePostedFilter; label: string }[] = [
  { value: 'Any', label: 'Any time' },
  { value: 'Last24Hours', label: 'Last 24 hours' },
  { value: 'Last7Days', label: 'Last 7 days' },
  { value: 'Last30Days', label: 'Last 30 days' },
]

export const sortOptions: { value: JobSortOption; label: string }[] = [
  { value: 'MostRelevant', label: 'Most relevant' },
  { value: 'Newest', label: 'Newest' },
  { value: 'HighestMatch', label: 'Highest match' },
]

export const matchFilterOptions: { value: MatchScoreFilter; label: string }[] = [
  { value: 'Any', label: 'Any match' },
  { value: 'SixtyPlus', label: '60%+' },
  { value: 'EightyPlus', label: '80%+' },
]

export function labelExperience(value?: ExperienceLevel) {
  return experienceOptions.find((o) => o.value === value)?.label ?? null
}

export function labelWork(value?: WorkArrangement) {
  return workOptions.find((o) => o.value === value)?.label ?? null
}

export function labelEmployment(value?: EmploymentType) {
  return employmentOptions.find((o) => o.value === value)?.label ?? null
}

export function labelAvailability(value: AvailabilityStatus) {
  switch (value) {
    case 'AppearsActive':
      return 'Appears active'
    case 'RecentlyListed':
      return 'Recently listed'
    default:
      return 'Availability unknown'
  }
}

export function formatPostedDate(iso?: string) {
  if (!iso) return null
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) return null
  const diff = Date.now() - date.getTime()
  const days = Math.floor(diff / 86_400_000)
  if (days <= 0) return 'Posted today'
  if (days === 1) return 'Posted 1 day ago'
  return `Posted ${days} days ago`
}
