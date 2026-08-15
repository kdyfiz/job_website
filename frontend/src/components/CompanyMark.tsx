interface Props {
  name: string
  className?: string
}

export function CompanyMark({ name, className = 'h-11 w-11 text-sm' }: Props) {
  const letter = (name.trim()[0] ?? '?').toUpperCase()

  return (
    <span
      aria-hidden="true"
      className={`inline-flex shrink-0 items-center justify-center rounded-xl bg-brand-soft font-semibold text-brand-dark ${className}`}
    >
      {letter}
    </span>
  )
}
