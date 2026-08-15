import { useId, useState, type ChangeEvent } from 'react'

interface Props {
  onFile: (file: File | null) => void
  disabled?: boolean
}

export function CVUpload({ onFile, disabled = false }: Props) {
  const inputId = useId()
  const [fileName, setFileName] = useState<string | null>(null)
  const [localError, setLocalError] = useState<string | null>(null)

  function handleChange(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0] ?? null
    if (!file) {
      setFileName(null)
      onFile(null)
      return
    }

    if (!file.name.toLowerCase().endsWith('.pdf') || file.size > 5 * 1024 * 1024) {
      setLocalError('Please upload a PDF CV under 5 MB.')
      setFileName(null)
      onFile(null)
      event.target.value = ''
      return
    }

    setLocalError(null)
    setFileName(file.name)
    onFile(file)
  }

  return (
    <div className="grid gap-2">
      <label
        htmlFor={inputId}
        className="flex cursor-pointer flex-col items-center justify-center rounded-2xl border border-dashed border-line bg-white px-4 py-10 text-center"
      >
        <span className="text-sm font-semibold text-ink">Upload a PDF CV</span>
        <span className="mt-1 text-sm text-muted">Maximum 5 MB. Processed temporarily — not stored.</span>
        <span className="mt-4 rounded-lg bg-brand px-4 py-2 text-sm font-semibold text-white">Choose file</span>
        {fileName && <span className="mt-3 text-sm text-slate-600">{fileName}</span>}
      </label>
      <input
        id={inputId}
        type="file"
        accept="application/pdf,.pdf"
        className="sr-only"
        onChange={handleChange}
        disabled={disabled}
      />
      {localError && <p className="text-sm text-red-700">{localError}</p>}
    </div>
  )
}
