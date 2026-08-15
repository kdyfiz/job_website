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
        className={`card flex cursor-pointer flex-col items-center justify-center border-dashed px-4 py-12 text-center transition ${
          fileName ? 'border-brand bg-brand-soft/40' : 'hover:border-[#d4cec2]'
        } ${disabled ? 'pointer-events-none opacity-60' : ''}`}
      >
        <span className="inline-flex h-10 w-10 items-center justify-center rounded-full bg-brand-soft text-brand">
          <svg viewBox="0 0 24 24" className="h-5 w-5" fill="none" aria-hidden="true">
            <path
              d="M12 16V8m0 0l-3 3m3-3l3 3M6 20h12"
              stroke="currentColor"
              strokeWidth="1.75"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
        </span>
        <span className="mt-4 text-sm font-semibold text-ink">{fileName ? 'CV selected' : 'Upload a PDF CV'}</span>
        <span className="mt-1 max-w-xs text-sm leading-6 text-muted">
          {fileName ?? 'Maximum 5 MB. Processed for this request only — not stored.'}
        </span>
        <span className="btn-primary mt-5 h-10">{fileName ? 'Replace file' : 'Choose file'}</span>
      </label>
      <input
        id={inputId}
        type="file"
        accept="application/pdf,.pdf"
        className="sr-only"
        onChange={handleChange}
        disabled={disabled}
      />
      {localError && <p className="text-sm text-[#8a3a2b]">{localError}</p>}
    </div>
  )
}
