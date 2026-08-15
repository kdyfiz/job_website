import type { FormEvent } from 'react'
import type { SearchParams } from '../types/job'
import { experienceOptions, workOptions } from '../utils/labels'

interface Props {
  value: SearchParams
  onChange: (value: SearchParams) => void
  onSubmit: () => void
  compact?: boolean
}

export function SearchForm({ value, onChange, onSubmit, compact = false }: Props) {
  function handleSubmit(event: FormEvent) {
    event.preventDefault()
    onSubmit()
  }

  return (
    <form onSubmit={handleSubmit} className="grid gap-3">
      <div className={`grid gap-3 ${compact ? 'md:grid-cols-2' : ''}`}>
        <label className="grid gap-1 text-sm font-medium text-slate-700">
          Job title / keywords
          <input
            required
            minLength={2}
            value={value.query}
            onChange={(e) => onChange({ ...value, query: e.target.value })}
            placeholder="Junior Software Developer"
            className="h-12 rounded-lg border border-line bg-white px-3 text-base font-normal text-ink"
          />
        </label>
        <label className="grid gap-1 text-sm font-medium text-slate-700">
          Location
          <input
            value={value.location}
            onChange={(e) => onChange({ ...value, location: e.target.value })}
            placeholder="Malaysia"
            className="h-12 rounded-lg border border-line bg-white px-3 text-base font-normal text-ink"
          />
        </label>
      </div>
      <div className="grid gap-3 sm:grid-cols-2">
        <label className="grid gap-1 text-sm font-medium text-slate-700">
          Experience level
          <select
            value={value.experienceLevel}
            onChange={(e) =>
              onChange({ ...value, experienceLevel: e.target.value as SearchParams['experienceLevel'] })
            }
            className="h-12 rounded-lg border border-line bg-white px-3 text-base font-normal text-ink"
          >
            {experienceOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>
        <label className="grid gap-1 text-sm font-medium text-slate-700">
          Work arrangement
          <select
            value={value.workArrangement}
            onChange={(e) =>
              onChange({ ...value, workArrangement: e.target.value as SearchParams['workArrangement'] })
            }
            className="h-12 rounded-lg border border-line bg-white px-3 text-base font-normal text-ink"
          >
            {workOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </label>
      </div>
      <button
        type="submit"
        className="h-12 rounded-lg bg-brand px-6 text-sm font-semibold tracking-wide text-white hover:bg-brand-dark"
      >
        Find Jobs
      </button>
    </form>
  )
}
