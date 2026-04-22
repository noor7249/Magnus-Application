import { Link } from 'react-router-dom'

export function AuthFormShell({ title, description, footer, children }) {
  return (
    <div className="w-full max-w-md">
      <Link to="/login" className="text-sm font-black uppercase tracking-[0.3em] text-slate-950">
        Magnus
      </Link>
      <h1 className="mt-8 text-3xl font-black tracking-tight text-slate-950">{title}</h1>
      <p className="mt-2 text-sm leading-6 text-slate-500">{description}</p>
      <div className="mt-8">{children}</div>
      {footer && <p className="mt-6 text-center text-sm text-slate-500">{footer}</p>}
    </div>
  )
}
