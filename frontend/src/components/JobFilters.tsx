import type { SearchParams } from '../types/job'
import { dateOptions, employmentOptions, experienceOptions, matchFilterOptions, workOptions } from '../utils/labels'

interface Props {
  value: SearchParams
  onChange: (value: SearchParams) => void
  showMatchFilter?: boolean
}

export function JobFilters({ value, onChange, showMatchFilter = false }: Props) {
  return (
    <aside className="rounded-2xl border border-line bg-white p-4">
      <h2 className="text-sm font-semibold text-ink">Filters</h2>
      <div className="mt-4 grid gap-3">
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
    <label className="grid gap-1 text-sm font-medium text-slate-700">
      {label}
      <select
        value={value}
        onChange={(e) => onChange(e.target.value as T)}
        className="h-10 rounded-lg border border-line bg-white px-3 font-normal text-ink"
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
