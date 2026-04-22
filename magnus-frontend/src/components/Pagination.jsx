import { Button } from './Button'

export function Pagination({ pageNumber = 1, totalPages = 1, totalCount = 0, onPageChange }) {
  return (
    <div className="flex flex-col gap-3 pt-4 text-sm text-slate-500 sm:flex-row sm:items-center sm:justify-between">
      <span>
        Page <strong className="text-slate-900">{pageNumber}</strong> of{' '}
        <strong className="text-slate-900">{Math.max(totalPages, 1)}</strong> · {totalCount} records
      </span>
      <div className="flex gap-2">
        <Button variant="secondary" disabled={pageNumber <= 1} onClick={() => onPageChange(pageNumber - 1)}>
          Previous
        </Button>
        <Button variant="secondary" disabled={pageNumber >= totalPages} onClick={() => onPageChange(pageNumber + 1)}>
          Next
        </Button>
      </div>
    </div>
  )
}
