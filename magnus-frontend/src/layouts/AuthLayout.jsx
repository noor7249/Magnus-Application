import { Outlet } from 'react-router-dom'

export function AuthLayout() {
  return (
    <main className="min-h-screen bg-[#f5f1e8] p-4 text-slate-950">
      <div className="mx-auto grid min-h-[calc(100vh-2rem)] max-w-6xl overflow-hidden rounded-[2rem] border border-slate-200 bg-white shadow-2xl lg:grid-cols-[1.05fr_0.95fr]">
        <section className="relative hidden bg-slate-950 p-10 text-white lg:block">
          <div className="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,rgba(251,191,36,0.24),transparent_28%),radial-gradient(circle_at_80%_10%,rgba(20,184,166,0.22),transparent_26%),linear-gradient(140deg,#020617,#111827)]" />
          <div className="relative flex h-full flex-col justify-between">
            <div>
              <p className="text-sm font-bold uppercase tracking-[0.35em] text-amber-200">Magnus</p>
              <h1 className="mt-8 max-w-lg text-5xl font-black leading-tight tracking-tight">
                Workforce operations with calm, secure control.
              </h1>
            </div>
            <div className="grid grid-cols-3 gap-3 text-sm">
              {['JWT Auth', 'RBAC', 'CRUD APIs'].map((item) => (
                <div key={item} className="rounded-2xl border border-white/15 bg-white/10 p-4 backdrop-blur">
                  {item}
                </div>
              ))}
            </div>
          </div>
        </section>
        <section className="flex items-center justify-center p-6 sm:p-10">
          <Outlet />
        </section>
      </div>
    </main>
  )
}
