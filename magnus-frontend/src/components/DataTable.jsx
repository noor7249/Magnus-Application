import { ArrowDown, ArrowUp, ArrowUpDown } from 'lucide-react'
import { EmptyState } from './EmptyState'

export function DataTable({ columns, rows, sortBy, descending, onSort, getRowKey }) {
  if (!rows?.length) {
    return <EmptyState />
  }

  return (
    <div className="overflow-hidden rounded-2xl border border-slate-200">
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-slate-200">
          <thead className="bg-slate-50">
            <tr>
              {columns.map((column) => {
                const sorted = sortBy === column.sortKey
                return (
                  <th key={column.key} className="px-4 py-3 text-left text-xs font-bold uppercase tracking-wide text-slate-500">
                    {column.sortKey ? (
                      <button
                        type="button"
                        onClick={() => onSort(column.sortKey)}
                        className="inline-flex items-center gap-1.5 hover:text-slate-950"
                      >
                        {column.header}
                        {sorted ? (
                          descending ? <ArrowDown size={14} /> : <ArrowUp size={14} />
                        ) : (
                          <ArrowUpDown size={14} />
                        )}
                      </button>
                    ) : (
                      column.header
                    )}
                  </th>
                )
              })}
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 bg-white">
            {rows.map((row) => (
              <tr key={getRowKey(row)} className="hover:bg-slate-50">
                {columns.map((column) => (
                  <td key={column.key} className="whitespace-nowrap px-4 py-3 text-sm text-slate-700">
                    {column.render ? column.render(row) : row[column.key]}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
