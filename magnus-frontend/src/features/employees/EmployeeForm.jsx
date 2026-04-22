import { useState } from 'react'
import { Button } from '../../components/Button'
import { Input } from '../../components/Input'
import { Select } from '../../components/Select'

const emptyEmployee = {
  firstName: '',
  lastName: '',
  email: '',
  phoneNumber: '',
  dateOfJoining: '',
  salary: '',
  departmentId: '',
  designationId: '',
  isActive: true,
}

function toDateInput(value) {
  if (!value) return ''
  return new Date(value).toISOString().slice(0, 10)
}

export function EmployeeForm({ initialValues, departments, designations, onSubmit, onCancel, submitting }) {
  const [form, setForm] = useState(() => {
    if (!initialValues) return emptyEmployee
    return {
      ...initialValues,
      dateOfJoining: toDateInput(initialValues.dateOfJoining),
      salary: initialValues.salary ?? '',
      departmentId: initialValues.departmentId ?? '',
      designationId: initialValues.designationId ?? '',
    }
  })
  const [errors, setErrors] = useState({})

  const validate = () => {
    const nextErrors = {}
    if (!form.firstName.trim()) nextErrors.firstName = 'First name is required.'
    if (!form.lastName.trim()) nextErrors.lastName = 'Last name is required.'
    if (!form.email.includes('@')) nextErrors.email = 'Enter a valid email address.'
    if (!form.dateOfJoining) nextErrors.dateOfJoining = 'Date of joining is required.'
    if (!Number(form.salary) || Number(form.salary) <= 0) nextErrors.salary = 'Salary must be greater than zero.'
    if (!form.departmentId) nextErrors.departmentId = 'Select a department.'
    if (!form.designationId) nextErrors.designationId = 'Select a designation.'
    setErrors(nextErrors)
    return Object.keys(nextErrors).length === 0
  }

  const handleSubmit = (event) => {
    event.preventDefault()
    if (!validate()) return

    onSubmit({
      firstName: form.firstName.trim(),
      lastName: form.lastName.trim(),
      email: form.email.trim(),
      phoneNumber: form.phoneNumber || null,
      dateOfJoining: new Date(form.dateOfJoining).toISOString(),
      salary: Number(form.salary),
      departmentId: Number(form.departmentId),
      designationId: Number(form.designationId),
      isActive: Boolean(form.isActive),
    })
  }

  return (
    <form onSubmit={handleSubmit} className="grid gap-4 md:grid-cols-2">
      <Input label="First name" value={form.firstName} error={errors.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} />
      <Input label="Last name" value={form.lastName} error={errors.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} />
      <Input label="Email" type="email" value={form.email} error={errors.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
      <Input label="Phone number" value={form.phoneNumber || ''} onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} />
      <Input label="Date of joining" type="date" value={form.dateOfJoining} error={errors.dateOfJoining} onChange={(e) => setForm({ ...form, dateOfJoining: e.target.value })} />
      <Input label="Salary" type="number" min="1" value={form.salary} error={errors.salary} onChange={(e) => setForm({ ...form, salary: e.target.value })} />
      <Select label="Department" value={form.departmentId} error={errors.departmentId} onChange={(e) => setForm({ ...form, departmentId: e.target.value })}>
        <option value="">Select department</option>
        {departments.map((department) => <option key={department.id} value={department.id}>{department.name}</option>)}
      </Select>
      <Select label="Designation" value={form.designationId} error={errors.designationId} onChange={(e) => setForm({ ...form, designationId: e.target.value })}>
        <option value="">Select designation</option>
        {designations.map((designation) => <option key={designation.id} value={designation.id}>{designation.title}</option>)}
      </Select>
      {initialValues && (
        <Select label="Status" value={String(form.isActive)} onChange={(e) => setForm({ ...form, isActive: e.target.value === 'true' })}>
          <option value="true">Active</option>
          <option value="false">Inactive</option>
        </Select>
      )}
      <div className="flex justify-end gap-2 md:col-span-2">
        <Button variant="secondary" onClick={onCancel}>Cancel</Button>
        <Button type="submit" disabled={submitting}>{submitting ? 'Saving...' : 'Save employee'}</Button>
      </div>
    </form>
  )
}
