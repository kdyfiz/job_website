import { useState } from 'react'
import { Link, NavLink, useLocation } from 'react-router-dom'
import { Logo } from './Logo'

const links = [
  { to: '/', label: 'Search', match: (path: string) => path === '/' || path.startsWith('/jobs') },
  { to: '/about', label: 'About', match: (path: string) => path === '/about' },
  { to: '/match', label: 'Match CV', match: (path: string) => path === '/match' },
]

function linkClass(active: boolean, mobile = false) {
  if (mobile) {
    return active ? 'bg-brand text-white' : 'text-ink'
  }
  return active
    ? 'btn-primary h-10 px-4'
    : 'rounded-full px-3.5 py-1.5 text-muted no-underline transition hover:bg-white hover:text-ink'
}

export function Navbar() {
  const [open, setOpen] = useState(false)
  const { pathname } = useLocation()

  return (
    <header className="sticky top-0 z-40 border-b border-line bg-paper/90 backdrop-blur-md">
      <a
        href="#main"
        className="sr-only focus:not-sr-only focus:absolute focus:left-4 focus:top-3 focus:z-50 focus:rounded-md focus:bg-white focus:px-3 focus:py-2"
      >
        Skip to content
      </a>
      <div className="page-wrap flex h-16 items-center justify-between gap-4">
        <Link to="/" className="flex items-center gap-2.5 text-ink no-underline" onClick={() => setOpen(false)}>
          <Logo className="h-8 w-8 text-brand" />
          <span className="leading-tight">
            <span className="block text-[15px] font-semibold tracking-tight">JobScout</span>
            <span className="hidden text-[11px] text-muted sm:block">Find jobs. Match your skills.</span>
          </span>
        </Link>

        <nav aria-label="Primary" className="hidden items-center gap-1 text-sm font-medium md:flex">
          {links.map((link) => (
            <NavLink key={link.to} to={link.to} className={linkClass(link.match(pathname))}>
              {link.label}
            </NavLink>
          ))}
        </nav>

        <button
          type="button"
          className="inline-flex h-10 w-10 items-center justify-center rounded-lg border border-line bg-white md:hidden"
          aria-expanded={open}
          aria-controls="mobile-nav"
          onClick={() => setOpen((value) => !value)}
        >
          <span className="sr-only">Menu</span>
          <span className="flex flex-col gap-1.5">
            <span className="block h-px w-4 bg-ink" />
            <span className="block h-px w-4 bg-ink" />
            <span className="block h-px w-4 bg-ink" />
          </span>
        </button>
      </div>

      {open && (
        <nav id="mobile-nav" className="border-t border-line bg-paper px-4 py-3 md:hidden">
          {links.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              onClick={() => setOpen(false)}
              className={`block rounded-lg px-3 py-2.5 text-sm font-medium no-underline ${linkClass(link.match(pathname), true)}`}
            >
              {link.label}
            </NavLink>
          ))}
        </nav>
      )}
    </header>
  )
}
