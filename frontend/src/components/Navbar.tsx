import { Link, NavLink } from 'react-router-dom'

const links = [
  { to: '/', label: 'Search' },
  { to: '/match', label: 'Match my CV' },
  { to: '/about', label: 'About' },
]

export function Navbar() {
  return (
    <header className="border-b border-line bg-white">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-4">
        <Link to="/" className="flex items-center gap-2 no-underline">
          <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-brand text-sm font-bold text-white">
            JS
          </span>
          <span>
            <span className="block text-base font-semibold tracking-tight text-ink">JobScout</span>
            <span className="block text-xs text-muted">Find jobs. Match your skills.</span>
          </span>
        </Link>
        <nav aria-label="Primary" className="flex items-center gap-1 text-sm font-medium">
          {links.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              className={({ isActive }) =>
                `rounded-md px-3 py-2 no-underline ${
                  isActive ? 'bg-brand-soft text-brand-dark' : 'text-slate-600 hover:bg-slate-100'
                }`
              }
            >
              {link.label}
            </NavLink>
          ))}
        </nav>
      </div>
    </header>
  )
}
