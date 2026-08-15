interface Props {
  skill: string
  tone?: 'default' | 'match' | 'gap'
}

export function SkillBadge({ skill, tone = 'default' }: Props) {
  const styles = {
    default: 'border-line bg-paper text-ink',
    match: 'border-transparent bg-brand-soft text-brand-dark',
    gap: 'border-transparent bg-[#f4ead4] text-[#7a5600]',
  }[tone]

  return (
    <span className={`inline-flex rounded-full border px-2.5 py-1 text-xs font-medium ${styles}`}>
      {skill}
    </span>
  )
}
