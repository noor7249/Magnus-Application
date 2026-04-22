import { Search } from 'lucide-react'

export function SearchBar({ value, onChange, placeholder = 'Search records...' }) {
  return (
    <div className="relative">
      <Search className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
      <input
        value={value}
        onChange={(event) => onChange(event.target.value)}
        placeholder={placeholder}
        className="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-3 text-sm outline-none transition focus:border-slate-950 focus:ring-4 focus:ring-slate-200 sm:w-80"
      />
    </div>
  )
}
