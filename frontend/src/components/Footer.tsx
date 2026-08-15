import { Link } from 'react-router-dom'
import { Logo } from './Logo'

export function Footer() {
  return (
    <footer className="mt-auto border-t border-line bg-white">
      <div className="page-wrap grid gap-10 py-12 sm:grid-cols-[1.4fr_1fr]">
        <div>
          <div className="flex items-center gap-2 text-ink">
            <Logo className="h-6 w-6 text-brand" />
            <span className="text-sm font-semibold">JobScout</span>
          </div>
          <p className="mt-3 max-w-sm text-sm leading-6 text-muted">
            A free job discovery tool. Search roles, optionally match a PDF CV, then apply on the original listing.
          </p>
        </div>
        <div className="grid grid-cols-2 gap-8 text-sm sm:justify-self-end sm:gap-16">
          <div className="grid gap-2">
            <p className="text-xs font-semibold uppercase tracking-[0.14em] text-muted">Product</p>
            <Link className="text-ink no-underline hover:text-brand" to="/">
              Search jobs
            </Link>
            <Link className="text-ink no-underline hover:text-brand" to="/match">
              Match CV
            </Link>
          </div>
          <div className="grid gap-2">
            <p className="text-xs font-semibold uppercase tracking-[0.14em] text-muted">More</p>
            <Link className="text-ink no-underline hover:text-brand" to="/about">
              About
            </Link>
            <Link className="text-ink no-underline hover:text-brand" to="/privacy">
              Privacy
            </Link>
          </div>
        </div>
      </div>
      <div className="border-t border-line">
        <div className="page-wrap py-5 text-xs leading-5 text-muted">
          <p>© {new Date().getFullYear()} JobScout. Demo listings are labelled. Always verify the original source.</p>
        </div>
      </div>
    </footer>
  )
}
