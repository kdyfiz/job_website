export function Logo({ className = 'h-8 w-8' }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 32 32" fill="none" aria-hidden="true">
      <rect width="32" height="32" rx="8" fill="currentColor" />
      <circle cx="16" cy="16" r="8" stroke="#F4F1EA" strokeWidth="1.75" />
      <path d="M16 10v12M10 16h12" stroke="#F4F1EA" strokeWidth="1.75" strokeLinecap="round" />
      <circle cx="16" cy="16" r="1.75" fill="#F4F1EA" />
    </svg>
  )
}
