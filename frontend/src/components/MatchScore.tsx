interface Props {
  percent: number
  compact?: boolean
}

export function MatchScore({ percent, compact = false }: Props) {
  const tone =
    percent >= 80 ? 'text-teal-800 bg-teal-50' : percent >= 60 ? 'text-amber-800 bg-amber-50' : 'text-slate-700 bg-slate-100'

  return (
    <div className={`rounded-lg px-3 py-2 ${tone}`}>
      <p className="text-[11px] font-medium uppercase tracking-wide opacity-80">Estimated match</p>
      <p className={`font-semibold ${compact ? 'text-lg' : 'text-2xl'}`}>{percent}%</p>
    </div>
  )
}
