interface Props {
  message?: string
}

export function LoadingState({ message = 'Searching for jobs...' }: Props) {
  return (
    <div role="status" aria-live="polite" className="grid gap-4">
      <p className="text-sm font-medium text-slate-600">{message}</p>
      {Array.from({ length: 3 }).map((_, index) => (
        <div key={index} className="animate-pulse rounded-2xl border border-line bg-white p-5">
          <div className="h-4 w-24 rounded bg-slate-200" />
          <div className="mt-3 h-6 w-2/3 rounded bg-slate-200" />
          <div className="mt-2 h-4 w-1/3 rounded bg-slate-200" />
          <div className="mt-4 h-4 w-full rounded bg-slate-100" />
        </div>
      ))}
    </div>
  )
}
