import { useCallback, useEffect, useState, useTransition } from 'react'
import { DEFAULT_PAGE_SIZE } from '../utils/constants'

export function useCrudList(fetcher, initialSortBy) {
  const [query, setQuery] = useState({
    pageNumber: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    searchTerm: '',
    sortBy: initialSortBy,
    descending: false,
  })
  const [data, setData] = useState({ items: [], totalCount: 0, totalPages: 0, pageNumber: 1, pageSize: DEFAULT_PAGE_SIZE })
  const [isLoading, setIsLoading] = useState(true)
  const [isPending, startTransition] = useTransition()

  const load = useCallback(async () => {
    setIsLoading(true)
    try {
      const result = await fetcher(query)
      setData(result)
    } finally {
      setIsLoading(false)
    }
  }, [fetcher, query])

  useEffect(() => {
    load()
  }, [load])

  const updateQuery = (patch) => {
    startTransition(() => {
      setQuery((current) => ({ ...current, ...patch }))
    })
  }

  const setSearchTerm = (searchTerm) => updateQuery({ searchTerm, pageNumber: 1 })
  const setPage = (pageNumber) => updateQuery({ pageNumber })
  const setSort = (sortBy) => {
    updateQuery({
      sortBy,
      descending: query.sortBy === sortBy ? !query.descending : false,
      pageNumber: 1,
    })
  }

  return {
    query,
    data,
    isLoading: isLoading || isPending,
    reload: load,
    setSearchTerm,
    setPage,
    setSort,
  }
}
