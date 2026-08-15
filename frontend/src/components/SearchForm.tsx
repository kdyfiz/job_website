import type { FormEvent } from 'react'
import type { SearchParams } from '../types/job'
import { experienceOptions, workOptions } from '../utils/labels'
import { LocationPicker } from './LocationPicker'

interface Props {
  value: SearchParams
  onChange: (value: SearchParams) => void
  onSubmit: (value: SearchParams) => void
  compact?: boolean
}

export function SearchForm({ value, onChange, onSubmit, compact = false }: Props) {
  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    onSubmit({
      ...value,
      query: value.query.trim(),
      location: value.location.trim(),
    })
  }

  return (
    <form onSubmit={handleSubmit} className="grid gap-4">
      <div className={compact ? 'grid gap-4 md:grid-cols-[1fr_auto] md:items-end' : 'grid gap-4 lg:grid-cols-[1fr_auto] lg:items-end'}>
        <label className="label">
          Job title / keywords
          <input
            required
            minLength={2}
            value={value.query}
            onChange={(e) => onChange({ ...value, query: e.target.value })}
            placeholder="Junior Software Developer"
            className="field h-12 text-[15px]"
          />
        </label>
        {!compact && (
          <button type="submit" className="btn-primary h-12 px-6 lg:min-w-36">
            Find Jobs
          </button>
        )}
        {compact && (
          <button type="submit" className="btn-primary h-12 tracking-wide">
            Find Jobs
          </button>
        )}
      </div>
      <div className="label">
        Location
        <LocationPicker value={value.location} onChange={(location) => onChange({ ...value, location })} />
      </div>
      <div className="grid gap-4 sm:grid-cols-2">
        <label className="label">
          Experience
          <select
            value={value.experienceLevel}
            onChange={(e) =>
              onChange({ ...value, experienceLevel: e.target.value as SearchParams['experienceLevel'] })
            }
            className="field h-12 text-[15px]"
          >
            {experienceOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>
        <label className="label">
          Work arrangement
          <select
            value={value.workArrangement}
            onChange={(e) =>
              onChange({ ...value, workArrangement: e.target.value as SearchParams['workArrangement'] })
            }
            className="field h-12 text-[15px]"
          >
            {workOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>
      </div>
    </form>
  )
}
