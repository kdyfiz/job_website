interface Props {
  percent: number
  compact?: boolean
}

export function MatchScore({ percent, compact = false }: Props) {
  const tone =
    percent >= 80
      ? 'bg-brand-soft text-brand-dark'
      : percent >= 60
        ? 'bg-[#f4ead4] text-[#7a5600]'
        : 'bg-surface text-muted'

  return (
    <div className={`shrink-0 rounded-xl px-3.5 py-2 text-right ${tone}`}>
      <p className="text-[10px] font-semibold uppercase tracking-[0.14em] opacity-80">Est. match</p>
      <p className={`font-semibold tabular-nums tracking-tight ${compact ? 'text-xl' : 'text-3xl'}`}>{percent}%</p>
    </div>
  )
}
