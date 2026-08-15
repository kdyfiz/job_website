interface Props {
  message: string
}

export function ErrorState({ message }: Props) {
  return (
    <div role="alert" className="rounded-xl border border-[#ead7d2] bg-[#fbf4f2] px-4 py-3 text-sm text-[#8a3a2b]">
      {message}
    </div>
  )
}
