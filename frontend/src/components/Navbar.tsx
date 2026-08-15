import { useState } from 'react'
import { Link, NavLink, useLocation } from 'react-router-dom'
import { Logo } from './Logo'

const links = [
  { to: '/', label: 'Search', match: (path: string) => path === '/' || path.startsWith('/jobs') },
  { to: '/about', label: 'About', match: (path: string) => path === '/about' },
]

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

        <div className="hidden items-center gap-2 md:flex">
          <nav aria-label="Primary" className="flex items-center gap-1 text-sm font-medium">
            {links.map((link) => (
              <NavLink
                key={link.to}
                to={link.to}
                className={() =>
                  `rounded-full px-3.5 py-1.5 no-underline transition ${
                    link.match(pathname) ? 'bg-brand-soft text-brand-dark' : 'text-muted hover:bg-white hover:text-ink'
                  }`
                }
              >
                {link.label}
              </NavLink>
            ))}
          </nav>
          <Link to="/match" className="btn-primary h-10 px-4">
            Match CV
          </Link>
        </div>

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
          {[...links, { to: '/match', label: 'Match CV', match: (path: string) => path === '/match' }].map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              onClick={() => setOpen(false)}
              className={() =>
                `block rounded-lg px-3 py-2.5 text-sm font-medium no-underline ${
                  link.match(pathname) ? 'bg-brand-soft text-brand-dark' : 'text-ink'
                }`
              }
            >
              {link.label}
            </NavLink>
          ))}
        </nav>
      )}
    </header>
  )
}
