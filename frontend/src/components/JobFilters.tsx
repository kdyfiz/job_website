import type { SearchParams } from '../types/job'
import { dateOptions, employmentOptions, experienceOptions, matchFilterOptions, workOptions } from '../utils/labels'

interface Props {
  value: SearchParams
  onChange: (value: SearchParams) => void
  showMatchFilter?: boolean
}

export function JobFilters({ value, onChange, showMatchFilter = false }: Props) {
  return (
    <aside className="card h-fit p-5 lg:sticky lg:top-24">
      <h2 className="text-xs font-semibold uppercase tracking-[0.14em] text-muted">Filters</h2>
      <div className="mt-4 grid gap-4">
        <FilterSelect
          label="Experience"
          value={value.experienceLevel}
          options={experienceOptions}
          onChange={(experienceLevel) => onChange({ ...value, experienceLevel })}
        />
        <FilterSelect
          label="Work arrangement"
          value={value.workArrangement}
          options={workOptions}
          onChange={(workArrangement) => onChange({ ...value, workArrangement })}
        />
        <FilterSelect
          label="Employment type"
          value={value.employmentType}
          options={employmentOptions}
          onChange={(employmentType) => onChange({ ...value, employmentType })}
        />
        <FilterSelect
          label="Date posted"
          value={value.datePosted}
          options={dateOptions}
          onChange={(datePosted) => onChange({ ...value, datePosted })}
        />
        {showMatchFilter && (
          <FilterSelect
            label="Match score"
            value={value.minMatchScore}
            options={matchFilterOptions}
            onChange={(minMatchScore) => onChange({ ...value, minMatchScore })}
          />
        )}
      </div>
    </aside>
  )
}

function FilterSelect<T extends string>({
  label,
  value,
  options,
  onChange,
}: {
  label: string
  value: T
  options: { value: T; label: string }[]
  onChange: (value: T) => void
}) {
  return (
    <label className="label">
      {label}
      <select value={value} onChange={(e) => onChange(e.target.value as T)} className="field">
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </label>
  )
}
