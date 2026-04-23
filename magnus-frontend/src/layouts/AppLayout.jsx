import { LogOut, Menu } from 'lucide-react'
import { useState } from 'react'
import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { Button } from '../components/Button'
import { Toast } from '../components/Toast'
import { useAuthStore } from '../store/authStore'
import { initials } from '../utils/formatters'
import { navigationItems } from '../routes/navigation'

export function AppLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const { user, logout, hasAnyRole } = useAuthStore()
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    navigate('/login', { replace: true })
  }

  return (
    <div className="min-h-screen bg-[#f5f1e8] text-slate-950">
      <aside className={`fixed inset-y-0 left-0 z-40 w-72 border-r border-slate-200 bg-slate-950 p-5 text-white transition-transform lg:translate-x-0 ${sidebarOpen ? 'translate-x-0' : '-translate-x-full'}`}>
        <div className="flex items-center gap-3">
          <div className="grid h-11 w-11 place-items-center rounded-2xl bg-amber-300 text-lg font-black text-slate-950">M</div>
          <div>
            <p className="text-lg font-black">Magnus</p>
            <p className="text-xs text-slate-400">Employee Console</p>
          </div>
        </div>
        <nav className="mt-10 space-y-2">
          {navigationItems.filter((item) => hasAnyRole(item.roles || [])).map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              onClick={() => setSidebarOpen(false)}
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-2xl px-4 py-3 text-sm font-semibold transition ${
                  isActive ? 'bg-white text-slate-950' : 'text-slate-300 hover:bg-white/10 hover:text-white'
                }`
              }
            >
              <item.icon size={18} />
              {item.label}
            </NavLink>
          ))}
        </nav>
      </aside>

      {sidebarOpen && <button aria-label="Close menu" className="fixed inset-0 z-30 bg-slate-950/40 lg:hidden" onClick={() => setSidebarOpen(false)} />}

      <div className="lg:pl-72">
        <header className="sticky top-0 z-20 border-b border-slate-200 bg-[#f5f1e8]/90 px-4 py-4 backdrop-blur sm:px-6 lg:px-8">
          <div className="flex items-center justify-between gap-4">
            <Button variant="secondary" className="lg:hidden" onClick={() => setSidebarOpen(true)}>
              <Menu size={18} />
            </Button>
            <div>
              <p className="text-sm font-semibold text-slate-500">Welcome back</p>
              <h1 className="text-xl font-black text-slate-950 sm:text-2xl">{user?.fullName || user?.email}</h1>
            </div>
            <div className="flex items-center gap-3">
              <div className="hidden items-center gap-3 rounded-2xl bg-white px-3 py-2 shadow-sm sm:flex">
                <div className="grid h-9 w-9 place-items-center rounded-xl bg-amber-200 text-sm font-black">
                  {initials(user?.fullName || user?.email)}
                </div>
                <div className="text-sm">
                  <p className="font-bold">{user?.email}</p>
                  <p className="text-xs text-slate-500">{user?.roles?.join(', ')}</p>
                </div>
              </div>
              <Button variant="ghost" onClick={handleLogout}>
                <LogOut size={18} />
                <span className="hidden sm:inline">Logout</span>
              </Button>
            </div>
          </div>
        </header>
        <main className="px-4 py-6 sm:px-6 lg:px-8">
          <Outlet />
        </main>
      </div>
      <Toast />
    </div>
  )
}
