import type { JobSortOption } from '../types/job'
import { sortOptions } from '../utils/labels'

interface Props {
  value: JobSortOption
  onChange: (value: JobSortOption) => void
  showHighestMatch?: boolean
}

export function JobSort({ value, onChange, showHighestMatch = false }: Props) {
  const options = showHighestMatch ? sortOptions : sortOptions.filter((o) => o.value !== 'HighestMatch')

  return (
    <label className="flex items-center gap-2 text-xs font-medium text-muted">
      Sort
      <select
        value={value}
        onChange={(e) => onChange(e.target.value as JobSortOption)}
        className="field h-10 w-auto min-w-40 text-sm text-ink"
      >
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </label>
  )
}
