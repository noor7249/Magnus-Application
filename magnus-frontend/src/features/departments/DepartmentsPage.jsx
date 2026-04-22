import { useCallback, useState } from 'react'
import { Edit, Plus, Trash2 } from 'lucide-react'
import { departmentsApi } from '../../api/departmentsApi'
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
import { DepartmentForm } from './DepartmentForm'

export function DepartmentsPage() {
  const fetchDepartments = useCallback((query) => departmentsApi.list(query), [])
  const { data, query, isLoading, reload, setPage, setSearchTerm, setSort } = useCrudList(fetchDepartments, 'name')
  const { showToast } = useUiStore()
  const hasAnyRole = useAuthStore((state) => state.hasAnyRole)
  const canCreate = hasAnyRole([ROLES.admin])
  const canUpdate = hasAnyRole([ROLES.admin, ROLES.manager])
  const canDelete = hasAnyRole([ROLES.admin])
  const [editing, setEditing] = useState(null)
  const [showForm, setShowForm] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  const saveDepartment = async (payload) => {
    setSubmitting(true)
    try {
      if (editing) await departmentsApi.update(editing.id, payload)
      else await departmentsApi.create(payload)
      showToast(editing ? 'Department updated.' : 'Department created.')
      setEditing(null)
      setShowForm(false)
      reload()
    } finally {
      setSubmitting(false)
    }
  }

  const deleteDepartment = async (department) => {
    if (!window.confirm(`Delete department "${department.name}"?`)) return
    await departmentsApi.remove(department.id)
    showToast('Department deleted.')
    reload()
  }

  const columns = [
    { key: 'name', header: 'Name', sortKey: 'name', render: (row) => <span className="font-bold text-slate-950">{row.name}</span> },
    { key: 'description', header: 'Description', render: (row) => row.description || '-' },
    { key: 'createdAt', header: 'Created', sortKey: 'createdat', render: (row) => formatDate(row.createdAt) },
    {
      key: 'actions',
      header: 'Actions',
      render: (row) => (
        <div className="flex gap-2">
          {canUpdate && <Button variant="secondary" className="px-3" onClick={() => { setEditing(row); setShowForm(true) }}><Edit size={15} /></Button>}
          {canDelete && <Button variant="danger" className="px-3" onClick={() => deleteDepartment(row)}><Trash2 size={15} /></Button>}
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        eyebrow="Directory"
        title="Departments"
        description="Create and maintain departments used by employee records."
        actions={canCreate && <Button onClick={() => { setEditing(null); setShowForm(true) }}><Plus size={18} /> Add Department</Button>}
      />
      {showForm && (
        <Card title={editing ? 'Edit department' : 'Add department'} description="Keep names concise and business-friendly.">
          <DepartmentForm key={editing?.id || 'new'} initialValues={editing} onSubmit={saveDepartment} onCancel={() => setShowForm(false)} submitting={submitting} />
        </Card>
      )}
      <div className="mt-5">
        <Card actions={<SearchBar value={query.searchTerm} onChange={setSearchTerm} placeholder="Search departments..." />}>
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
