interface Props {
  title?: string
  children?: string
}

export function EmptyState({
  title = 'No jobs found.',
  children = 'Try a broader job title, another location, or fewer filters.',
}: Props) {
  return (
    <div className="card border-dashed px-6 py-16 text-center">
      <p className="mx-auto mb-4 inline-flex h-10 w-10 items-center justify-center rounded-full bg-surface text-sm font-semibold text-muted">
        0
      </p>
      <h2 className="text-lg font-semibold tracking-tight text-ink">{title}</h2>
      <p className="mx-auto mt-2 max-w-md text-sm leading-6 text-muted">{children}</p>
    </div>
  )
}
