import { useCallback, useEffect, useState } from 'react'
import { Edit, Plus, Trash2 } from 'lucide-react'
import { departmentsApi } from '../../api/departmentsApi'
import { designationsApi } from '../../api/designationsApi'
import { employeesApi } from '../../api/employeesApi'
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
import { formatCurrency, formatDate } from '../../utils/formatters'
import { EmployeeForm } from './EmployeeForm'

export function EmployeesPage() {
  const fetchEmployees = useCallback((query) => employeesApi.list(query), [])
  const { data, query, isLoading, reload, setPage, setSearchTerm, setSort } = useCrudList(fetchEmployees, 'createdat')
  const { showToast } = useUiStore()
  const hasAnyRole = useAuthStore((state) => state.hasAnyRole)
  const canDelete = hasAnyRole([ROLES.admin])
  const [departments, setDepartments] = useState([])
  const [designations, setDesignations] = useState([])
  const [editing, setEditing] = useState(null)
  const [showForm, setShowForm] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    async function loadLookups() {
      const [departmentResult, designationResult] = await Promise.all([
        departmentsApi.list({ pageNumber: 1, pageSize: 100, sortBy: 'name' }),
        designationsApi.list({ pageNumber: 1, pageSize: 100, sortBy: 'title' }),
      ])
      setDepartments(departmentResult.items)
      setDesignations(designationResult.items)
    }
    loadLookups()
  }, [])

  const saveEmployee = async (payload) => {
    setSubmitting(true)
    try {
      if (editing) await employeesApi.update(editing.id, payload)
      else await employeesApi.create(payload)
      showToast(editing ? 'Employee updated.' : 'Employee created.')
      setEditing(null)
      setShowForm(false)
      reload()
    } finally {
      setSubmitting(false)
    }
  }

  const deleteEmployee = async (employee) => {
    if (!window.confirm(`Delete employee "${employee.firstName} ${employee.lastName}"?`)) return
    await employeesApi.remove(employee.id)
    showToast('Employee deleted.')
    reload()
  }

  const columns = [
    { key: 'name', header: 'Employee', render: (row) => <div><p className="font-bold text-slate-950">{row.firstName} {row.lastName}</p><p className="text-xs text-slate-500">{row.email}</p></div> },
    { key: 'departmentName', header: 'Department' },
    { key: 'designationTitle', header: 'Designation' },
    { key: 'salary', header: 'Salary', render: (row) => formatCurrency(row.salary) },
    { key: 'dateOfJoining', header: 'Joined', sortKey: 'dateofjoining', render: (row) => formatDate(row.dateOfJoining) },
    { key: 'isActive', header: 'Status', render: (row) => <span className={`rounded-full px-2.5 py-1 text-xs font-bold ${row.isActive ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-100 text-slate-500'}`}>{row.isActive ? 'Active' : 'Inactive'}</span> },
    {
      key: 'actions',
      header: 'Actions',
      render: (row) => (
        <div className="flex gap-2">
          <Button variant="secondary" className="px-3" onClick={() => { setEditing(row); setShowForm(true) }}><Edit size={15} /></Button>
          {canDelete && <Button variant="danger" className="px-3" onClick={() => deleteEmployee(row)}><Trash2 size={15} /></Button>}
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        eyebrow="People"
        title="Employees"
        description="Manage employee profiles with department, designation, salary, and joining details."
        actions={<Button onClick={() => { setEditing(null); setShowForm(true) }}><Plus size={18} /> Add Employee</Button>}
      />
      {showForm && (
        <Card title={editing ? 'Edit employee' : 'Add employee'} description="Employees must reference an existing department and designation.">
          <EmployeeForm
            key={editing?.id || 'new'}
            initialValues={editing}
            departments={departments}
            designations={designations}
            onSubmit={saveEmployee}
            onCancel={() => setShowForm(false)}
            submitting={submitting}
          />
        </Card>
      )}
      <div className="mt-5">
        <Card actions={<SearchBar value={query.searchTerm} onChange={setSearchTerm} placeholder="Search employees..." />}>
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
