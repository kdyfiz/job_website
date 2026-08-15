import { DEFAULT_LOCATION, joinLocations, malaysiaStates, MAX_LOCATION_SELECTIONS, parseLocations } from '../utils/locations'

interface Props {
  value: string
  onChange: (value: string) => void
}

export function LocationPicker({ value, onChange }: Props) {
  const selected = parseLocations(value).filter((item) => item !== DEFAULT_LOCATION)
  const atLimit = selected.length >= MAX_LOCATION_SELECTIONS

  function toggle(state: string) {
    if (selected.includes(state)) {
      onChange(joinLocations(selected.filter((item) => item !== state)))
      return
    }

    if (atLimit) return
    onChange(joinLocations([...selected, state]))
  }

  return (
    <div>
      <div className="flex flex-wrap gap-1.5">
        <span className="rounded-full border border-brand bg-brand-soft px-2.5 py-1 text-xs font-medium text-brand-dark">
          {DEFAULT_LOCATION}
        </span>
        {malaysiaStates.map((state) => {
          const isSelected = selected.includes(state)
          const disabled = atLimit && !isSelected
          return (
            <button
              key={state}
              type="button"
              onClick={() => toggle(state)}
              disabled={disabled}
              aria-pressed={isSelected}
              className={`rounded-full border px-2.5 py-1 text-xs font-medium transition ${
                isSelected
                  ? 'border-brand bg-brand-soft text-brand-dark'
                  : disabled
                    ? 'cursor-not-allowed border-line bg-paper text-muted/50'
                    : 'border-line bg-white text-muted hover:border-[#d4cec2] hover:text-ink'
              }`}
            >
              {state}
            </button>
          )
        })}
      </div>
      <p className="mt-2 text-xs text-muted">
        Always searches Malaysia. Choose up to {MAX_LOCATION_SELECTIONS} states to narrow.
      </p>
    </div>
  )
}
