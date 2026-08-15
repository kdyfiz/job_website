import type { SearchParams } from '../types/job'
import { defaultSearchParams } from '../services/api'

export function paramsFromUrl(urlParams: URLSearchParams): SearchParams {
  const defaults = defaultSearchParams()
  return {
    ...defaults,
    query: urlParams.get('query') ?? '',
    location: urlParams.get('location') ?? '',
    experienceLevel: (urlParams.get('experienceLevel') as SearchParams['experienceLevel']) ?? 'Any',
    workArrangement: (urlParams.get('workArrangement') as SearchParams['workArrangement']) ?? 'Any',
    employmentType: (urlParams.get('employmentType') as SearchParams['employmentType']) ?? 'Any',
    datePosted: (urlParams.get('datePosted') as SearchParams['datePosted']) ?? 'Any',
    sort: (urlParams.get('sort') as SearchParams['sort']) ?? 'MostRelevant',
  }
}

export function toSearchQuery(params: SearchParams) {
  const query = new URLSearchParams()
  const title = params.query.trim()
  const location = params.location.trim()

  if (title) query.set('query', title)
  if (location) query.set('location', location)
  if (params.experienceLevel !== 'Any') query.set('experienceLevel', params.experienceLevel)
  if (params.workArrangement !== 'Any') query.set('workArrangement', params.workArrangement)
  if (params.employmentType !== 'Any') query.set('employmentType', params.employmentType)
  if (params.datePosted !== 'Any') query.set('datePosted', params.datePosted)
  if (params.sort !== 'MostRelevant') query.set('sort', params.sort)
  return query
}

export function jobsPath(params: SearchParams) {
  const query = toSearchQuery(params).toString()
  return query ? `/jobs?${query}` : '/jobs'
}
