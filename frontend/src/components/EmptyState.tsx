interface Props {
  title?: string
  children?: string
}

export function EmptyState({
  title = 'No jobs found.',
  children = 'Try a broader job title, another location, or fewer filters.',
}: Props) {
  return (
    <div className="rounded-2xl border border-dashed border-line bg-white p-8 text-center">
      <h2 className="text-lg font-semibold text-ink">{title}</h2>
      <p className="mt-2 text-sm text-muted">{children}</p>
    </div>
  )
}
