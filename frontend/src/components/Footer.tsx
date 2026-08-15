import { Link } from 'react-router-dom'

export function Footer() {
  return (
    <footer className="mt-auto border-t border-line bg-white">
      <div className="mx-auto flex max-w-6xl flex-col gap-3 px-4 py-8 text-sm text-muted sm:flex-row sm:items-center sm:justify-between">
        <p>JobScout is a free discovery tool. Always verify listings on the original source.</p>
        <div className="flex gap-4">
          <Link className="text-slate-600 no-underline hover:text-brand" to="/about">
            About
          </Link>
          <Link className="text-slate-600 no-underline hover:text-brand" to="/privacy">
            Privacy
          </Link>
        </div>
      </div>
    </footer>
  )
}
