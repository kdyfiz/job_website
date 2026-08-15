interface Props {
  message?: string
}

export function LoadingState({ message = 'Searching for jobs...' }: Props) {
  return (
    <div role="status" aria-live="polite" className="grid gap-4">
      <p className="text-sm font-medium text-muted">{message}</p>
      {Array.from({ length: 3 }).map((_, index) => (
        <div key={index} className="card animate-pulse p-6">
          <div className="h-3 w-16 rounded bg-line" />
          <div className="mt-3 h-6 w-2/3 rounded bg-line" />
          <div className="mt-2 h-4 w-1/3 rounded bg-surface" />
          <div className="mt-5 h-4 w-full rounded bg-surface" />
        </div>
      ))}
    </div>
  )
}
