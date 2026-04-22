import { Link } from 'react-router-dom'
import { Button } from '../components/Button'

export function UnauthorizedPage() {
  return (
    <div className="grid min-h-[60vh] place-items-center">
      <div className="max-w-md rounded-3xl bg-white p-8 text-center shadow-sm">
        <p className="text-sm font-black uppercase tracking-[0.3em] text-red-600">403</p>
        <h1 className="mt-3 text-3xl font-black text-slate-950">Access denied</h1>
        <p className="mt-3 text-sm leading-6 text-slate-500">Your account does not have permission to open this area.</p>
        <Link to="/dashboard">
          <Button className="mt-6">Back to dashboard</Button>
        </Link>
      </div>
    </div>
  )
}
