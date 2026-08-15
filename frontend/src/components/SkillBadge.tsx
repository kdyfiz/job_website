interface Props {
  skill: string
  tone?: 'default' | 'match' | 'gap'
}

export function SkillBadge({ skill, tone = 'default' }: Props) {
  const styles = {
    default: 'bg-slate-100 text-slate-700',
    match: 'bg-teal-50 text-teal-800',
    gap: 'bg-amber-50 text-amber-900',
  }[tone]

  return <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${styles}`}>{skill}</span>
}
