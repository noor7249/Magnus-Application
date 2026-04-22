import { useCallback, useState } from 'react'
import { Edit, Plus, Trash2 } from 'lucide-react'
import { designationsApi } from '../../api/designationsApi'
import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import { DataTable } from '../../components/DataTable'
import { LoadingSpinner } from '../../components/LoadingSpinner'
import { PageHeader } from '../../components/PageHeader'
import { Pagination } from '../../components/Pagination'
import { SearchBar } from '../../components/SearchBar'
import { useCrudList } from '../../hooks/useCrudList'
import { useAuthStore } from '../../store/authStore'
import { useUiStore } from '../../store/uiStore'
import { ROLES } from '../../utils/constants'
import { formatDate } from '../../utils/formatters'
import { DesignationForm } from './DesignationForm'

export function DesignationsPage() {
  const fetchDesignations = useCallback((query) => designationsApi.list(query), [])
  const { data, query, isLoading, reload, setPage, setSearchTerm, setSort } = useCrudList(fetchDesignations, 'title')
  const { showToast } = useUiStore()
  const hasAnyRole = useAuthStore((state) => state.hasAnyRole)
  const canCreate = hasAnyRole([ROLES.admin])
  const canUpdate = hasAnyRole([ROLES.admin, ROLES.manager])
  const canDelete = hasAnyRole([ROLES.admin])
  const [editing, setEditing] = useState(null)
  const [showForm, setShowForm] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  const saveDesignation = async (payload) => {
    setSubmitting(true)
    try {
      if (editing) await designationsApi.update(editing.id, payload)
      else await designationsApi.create(payload)
      showToast(editing ? 'Designation updated.' : 'Designation created.')
      setEditing(null)
      setShowForm(false)
      reload()
    } finally {
      setSubmitting(false)
    }
  }

  const deleteDesignation = async (designation) => {
    if (!window.confirm(`Delete designation "${designation.title}"?`)) return
    await designationsApi.remove(designation.id)
    showToast('Designation deleted.')
    reload()
  }

  const columns = [
    { key: 'title', header: 'Title', sortKey: 'title', render: (row) => <span className="font-bold text-slate-950">{row.title}</span> },
    { key: 'description', header: 'Description', render: (row) => row.description || '-' },
    { key: 'createdAt', header: 'Created', sortKey: 'createdat', render: (row) => formatDate(row.createdAt) },
    {
      key: 'actions',
      header: 'Actions',
      render: (row) => (
        <div className="flex gap-2">
          {canUpdate && <Button variant="secondary" className="px-3" onClick={() => { setEditing(row); setShowForm(true) }}><Edit size={15} /></Button>}
          {canDelete && <Button variant="danger" className="px-3" onClick={() => deleteDesignation(row)}><Trash2 size={15} /></Button>}
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        eyebrow="Directory"
        title="Designations"
        description="Manage job titles and designation metadata used across employees."
        actions={canCreate && <Button onClick={() => { setEditing(null); setShowForm(true) }}><Plus size={18} /> Add Designation</Button>}
      />
      {showForm && (
        <Card title={editing ? 'Edit designation' : 'Add designation'} description="Use clear titles that match HR policy.">
          <DesignationForm key={editing?.id || 'new'} initialValues={editing} onSubmit={saveDesignation} onCancel={() => setShowForm(false)} submitting={submitting} />
        </Card>
      )}
      <div className="mt-5">
        <Card actions={<SearchBar value={query.searchTerm} onChange={setSearchTerm} placeholder="Search designations..." />}>
          {isLoading ? <LoadingSpinner /> : (
            <>
              <DataTable columns={columns} rows={data.items} sortBy={query.sortBy} descending={query.descending} onSort={setSort} getRowKey={(row) => row.id} />
              <Pagination {...data} onPageChange={setPage} />
            </>
          )}
        </Card>
      </div>
    </>
  )
}
