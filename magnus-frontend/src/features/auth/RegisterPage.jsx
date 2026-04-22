import { Link, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { Button } from '../../components/Button'
import { Input } from '../../components/Input'
import { useAuthStore } from '../../store/authStore'
import { useUiStore } from '../../store/uiStore'
import { getApiErrorMessage } from '../../utils/apiError'
import { AuthFormShell } from './AuthFormShell'

export function RegisterPage() {
  const navigate = useNavigate()
  const register = useAuthStore((state) => state.register)
  const { showToast, setGlobalError } = useUiStore()
  const [form, setForm] = useState({
    fullName: '',
    email: '',
    password: '',
    confirmPassword: '',
  })
  const [errors, setErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)

  const validate = () => {
    const nextErrors = {}
    if (form.fullName.trim().length < 2) nextErrors.fullName = 'Full name is required.'
    if (!form.email.includes('@')) nextErrors.email = 'Enter a valid email address.'
    if (form.password.length < 10) nextErrors.password = 'Password must be at least 10 characters.'
    if (form.password !== form.confirmPassword) nextErrors.confirmPassword = 'Passwords must match.'
    setErrors(nextErrors)
    return Object.keys(nextErrors).length === 0
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    if (!validate()) return
    setSubmitting(true)
    try {
      await register(form)
      showToast('Account created successfully.')
      navigate('/dashboard', { replace: true })
    } catch (error) {
      setGlobalError(getApiErrorMessage(error))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthFormShell
      title="Create your account"
      description="Register a user for the Magnus employee management console."
      footer={
        <>
          Already have an account?{' '}
          <Link className="font-bold text-slate-950 underline" to="/login">
            Sign in
          </Link>
        </>
      }
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        <Input label="Full name" value={form.fullName} error={errors.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
        <Input label="Email" type="email" value={form.email} error={errors.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
        <Input label="Password" type="password" value={form.password} error={errors.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
        <Input label="Confirm password" type="password" value={form.confirmPassword} error={errors.confirmPassword} onChange={(e) => setForm({ ...form, confirmPassword: e.target.value })} />
        <Button type="submit" className="w-full" disabled={submitting}>
          {submitting ? 'Creating account...' : 'Create account'}
        </Button>
      </form>
    </AuthFormShell>
  )
}
