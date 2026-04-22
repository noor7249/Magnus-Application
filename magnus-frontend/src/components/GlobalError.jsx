import { X } from 'lucide-react'
import { useUiStore } from '../store/uiStore'

export function GlobalError() {
  const { globalError, clearGlobalError } = useUiStore()

  if (!globalError) return null

  return (
    <div className="fixed left-1/2 top-4 z-50 w-[calc(100%-2rem)] max-w-xl -translate-x-1/2 rounded-2xl border border-red-200 bg-red-50 p-4 text-sm font-semibold text-red-700 shadow-xl">
      <div className="flex items-start justify-between gap-3">
        <span>{globalError}</span>
        <button type="button" onClick={clearGlobalError} className="rounded-lg p-1 hover:bg-red-100" aria-label="Dismiss error">
          <X size={16} />
        </button>
      </div>
    </div>
  )
}
