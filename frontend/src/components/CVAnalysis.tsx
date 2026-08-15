import type { CVAnalysis } from '../types/job'
import { SkillBadge } from './SkillBadge'

interface Props {
  analysis: CVAnalysis
}

export function CVAnalysisPanel({ analysis }: Props) {
  return (
    <section className="rounded-2xl border border-line bg-white p-5">
      <h2 className="text-lg font-semibold text-ink">Your CV</h2>
      <p className="mt-1 text-sm text-muted">{analysis.skillCount} skills detected</p>
      <div className="mt-4 flex flex-wrap gap-2">
        {analysis.skills.length > 0 ? (
          analysis.skills.map((skill) => <SkillBadge key={skill} skill={skill} tone="match" />)
        ) : (
          <p className="text-sm text-muted">No known skills were detected.</p>
        )}
      </div>
      {analysis.experienceIndicators.length > 0 && (
        <div className="mt-4">
          <h3 className="text-sm font-semibold text-slate-700">Experience indicators</h3>
          <div className="mt-2 flex flex-wrap gap-2">
            {analysis.experienceIndicators.map((item) => (
              <SkillBadge key={item} skill={item} />
            ))}
          </div>
        </div>
      )}
      {analysis.warning && <p className="mt-4 text-sm text-amber-800">{analysis.warning}</p>}
    </section>
  )
}
