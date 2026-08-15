export const MAX_LOCATION_SELECTIONS = 3

export const malaysiaStates = [
  'Kuala Lumpur',
  'Selangor',
  'Penang',
  'Johor',
  'Putrajaya',
  'Negeri Sembilan',
  'Perak',
  'Malacca',
  'Kedah',
  'Pahang',
  'Kelantan',
  'Terengganu',
  'Perlis',
  'Sabah',
  'Sarawak',
  'Labuan',
] as const

export function parseLocations(value: string): string[] {
  return value
    .split(',')
    .map((item) => item.trim())
    .filter(Boolean)
}

export function joinLocations(states: string[]): string {
  return states.join(', ')
}
