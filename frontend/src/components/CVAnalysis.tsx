import type { CVAnalysis } from '../types/job'
import { SkillBadge } from './SkillBadge'

interface Props {
  analysis: CVAnalysis
}

export function CVAnalysisPanel({ analysis }: Props) {
  return (
    <section className="card p-5 sm:p-6">
      <div className="flex items-end justify-between gap-3">
        <h2 className="text-base font-semibold text-ink">Detected from your CV</h2>
        <p className="text-sm text-muted">{analysis.skillCount} skills</p>
      </div>
      <div className="mt-4 flex flex-wrap gap-2">
        {analysis.skills.length > 0 ? (
          analysis.skills.map((skill) => <SkillBadge key={skill} skill={skill} tone="match" />)
        ) : (
          <p className="text-sm text-muted">No known skills were detected.</p>
        )}
      </div>
      {analysis.experienceIndicators.length > 0 && (
        <div className="mt-5 border-t border-line pt-4">
          <h3 className="text-xs font-semibold uppercase tracking-[0.14em] text-muted">Experience indicators</h3>
          <div className="mt-2 flex flex-wrap gap-2">
            {analysis.experienceIndicators.map((item) => (
              <SkillBadge key={item} skill={item} />
            ))}
          </div>
        </div>
      )}
      {analysis.warning && <p className="mt-4 text-sm text-amber">{analysis.warning}</p>}
    </section>
  )
}
