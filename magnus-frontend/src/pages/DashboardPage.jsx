import { useEffect, useState } from 'react'
import { BadgeCheck, Building2, Users } from 'lucide-react'
import { departmentsApi } from '../api/departmentsApi'
import { designationsApi } from '../api/designationsApi'
import { employeesApi } from '../api/employeesApi'
import { Card } from '../components/Card'
import { LoadingSpinner } from '../components/LoadingSpinner'
import { PageHeader } from '../components/PageHeader'

const cards = [
  { key: 'employees', label: 'Employees', icon: Users, accent: 'bg-amber-200' },
  { key: 'departments', label: 'Departments', icon: Building2, accent: 'bg-teal-200' },
  { key: 'designations', label: 'Designations', icon: BadgeCheck, accent: 'bg-sky-200' },
]

export function DashboardPage() {
  const [stats, setStats] = useState(null)

  useEffect(() => {
    async function loadStats() {
      const [employees, departments, designations] = await Promise.all([
        employeesApi.list({ pageNumber: 1, pageSize: 1 }),
        departmentsApi.list({ pageNumber: 1, pageSize: 1 }),
        designationsApi.list({ pageNumber: 1, pageSize: 1 }),
      ])
      setStats({
        employees: employees.totalCount,
        departments: departments.totalCount,
        designations: designations.totalCount,
      })
    }
    loadStats()
  }, [])

  return (
    <>
      <PageHeader
        eyebrow="Overview"
        title="Dashboard"
        description="A quick operational snapshot from your Magnus API data."
      />
      {!stats ? (
        <Card><LoadingSpinner label="Loading dashboard" /></Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-3">
          {cards.map((card) => (
            <Card key={card.key}>
              <div className="flex items-start justify-between">
                <div>
                  <p className="text-sm font-semibold text-slate-500">{card.label}</p>
                  <p className="mt-3 text-4xl font-black text-slate-950">{stats[card.key]}</p>
                </div>
                <div className={`grid h-12 w-12 place-items-center rounded-2xl ${card.accent}`}>
                  <card.icon size={22} />
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </>
  )
}
