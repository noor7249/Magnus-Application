import { useEffect } from 'react'
import { useUiStore } from '../store/uiStore'

export function Toast() {
  const { toast, clearToast } = useUiStore()

  useEffect(() => {
    if (!toast) return undefined
    const timeoutId = window.setTimeout(clearToast, 3500)
    return () => window.clearTimeout(timeoutId)
  }, [toast, clearToast])

  if (!toast) return null

  const tone = toast.type === 'error' ? 'bg-red-600' : 'bg-slate-950'

  return (
    <div className="fixed bottom-5 right-5 z-50 max-w-sm">
      <div className={`${tone} rounded-2xl px-4 py-3 text-sm font-semibold text-white shadow-2xl`}>
        {toast.message}
      </div>
    </div>
  )
}
