export type ExperienceLevel =
  | 'Any'
  | 'Internship'
  | 'FreshGraduate'
  | 'EntryLevel'
  | 'OneToTwoYears'
  | 'ThreeToFiveYears'

export type WorkArrangement = 'Any' | 'Remote' | 'Hybrid' | 'OnSite'
export type EmploymentType = 'Any' | 'FullTime' | 'PartTime' | 'Internship' | 'Contract'
export type AvailabilityStatus = 'AppearsActive' | 'RecentlyListed' | 'AvailabilityUnknown'
export type JobSortOption = 'MostRelevant' | 'Newest' | 'HighestMatch'
export type DatePostedFilter = 'Any' | 'Last24Hours' | 'Last7Days' | 'Last30Days'
export type MatchScoreFilter = 'Any' | 'SixtyPlus' | 'EightyPlus'

export interface SalaryInfo {
  min?: number
  max?: number
  currency?: string
  period?: string
  display?: string
}

export interface MatchExplanation {
  estimatedMatchPercent: number
  matchingSkills: string[]
  missingSkills: string[]
  experienceExplanation?: string
  disclaimer: string
}

export interface Job {
  id: string
  title: string
  company: string
  location: string
  description: string
  employmentType?: EmploymentType
  experienceLevel?: ExperienceLevel
  workArrangement?: WorkArrangement
  skills: string[]
  salary?: SalaryInfo
  postedDate?: string
  source: string
  sourceUrl?: string
  availabilityStatus: AvailabilityStatus
  isDemoData: boolean
  estimatedMatchPercent?: number
  match?: MatchExplanation
}

export interface JobSearchResponse {
  total: number
  query: string
  location?: string
  usingDemoData: boolean
  jobs: Job[]
}

export interface CVAnalysis {
  skillCount: number
  skills: string[]
  experienceIndicators: string[]
  warning?: string
}

export interface JobMatchResponse {
  cv: CVAnalysis
  results: JobSearchResponse
}

export interface ApiErrorBody {
  error: {
    code: string
    message: string
  }
}

export interface SearchParams {
  query: string
  location: string
  experienceLevel: ExperienceLevel
  workArrangement: WorkArrangement
  employmentType: EmploymentType
  datePosted: DatePostedFilter
  sort: JobSortOption
  minMatchScore: MatchScoreFilter
}
