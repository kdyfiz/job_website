import type {
  CVAnalysis,
  ExperienceLevel,
  Job,
  JobMatchResponse,
  JobSearchResponse,
  SearchParams,
  WorkArrangement,
} from '../types/job'
import { parseApiError } from '../utils/errors'

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? ''

async function readJson<T>(response: Response): Promise<T> {
  if (!response.ok) {
    throw new Error(await parseApiError(response))
  }
  return (await response.json()) as T
}

export async function searchJobs(params: SearchParams): Promise<JobSearchResponse> {
  const query = new URLSearchParams()
  query.set('query', params.query)
  if (params.location) query.set('location', params.location)
  if (params.experienceLevel !== 'Any') query.set('experienceLevel', params.experienceLevel)
  if (params.workArrangement !== 'Any') query.set('workArrangement', params.workArrangement)
  if (params.employmentType !== 'Any') query.set('employmentType', params.employmentType)
  if (params.datePosted !== 'Any') query.set('datePosted', params.datePosted)
  if (params.sort !== 'MostRelevant') query.set('sort', params.sort)

  const response = await fetch(`${API_BASE}/api/jobs/search?${query.toString()}`)
  return readJson<JobSearchResponse>(response)
}

export async function getJob(id: string): Promise<Job> {
  const response = await fetch(`${API_BASE}/api/jobs/${encodeURIComponent(id)}`)
  return readJson<Job>(response)
}

export async function analyzeCv(file: File): Promise<CVAnalysis> {
  const body = new FormData()
  body.append('file', file)
  const response = await fetch(`${API_BASE}/api/cv/analyze`, { method: 'POST', body })
  return readJson<CVAnalysis>(response)
}

export async function matchJobs(
  file: File,
  params: Pick<SearchParams, 'query' | 'location' | 'experienceLevel' | 'workArrangement' | 'employmentType' | 'datePosted' | 'minMatchScore'>,
): Promise<JobMatchResponse> {
  const body = new FormData()
  body.append('file', file)
  if (params.query) body.append('query', params.query)
  if (params.location) body.append('location', params.location)
  if (params.experienceLevel !== 'Any') body.append('experienceLevel', params.experienceLevel)
  if (params.workArrangement !== 'Any') body.append('workArrangement', params.workArrangement)
  if (params.employmentType !== 'Any') body.append('employmentType', params.employmentType)
  if (params.datePosted !== 'Any') body.append('datePosted', params.datePosted)
  if (params.minMatchScore !== 'Any') body.append('minMatchScore', params.minMatchScore)

  const response = await fetch(`${API_BASE}/api/jobs/match`, { method: 'POST', body })
  return readJson<JobMatchResponse>(response)
}

export function defaultSearchParams(): SearchParams {
  return {
    query: '',
    location: '',
    experienceLevel: 'Any' satisfies ExperienceLevel,
    workArrangement: 'Any' satisfies WorkArrangement,
    employmentType: 'Any',
    datePosted: 'Any',
    sort: 'MostRelevant',
    minMatchScore: 'Any',
  }
}
