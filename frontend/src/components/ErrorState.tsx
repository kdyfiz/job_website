interface Props {
  message: string
}

export function ErrorState({ message }: Props) {
  return (
    <div role="alert" className="rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-800">
      {message}
    </div>
  )
}
