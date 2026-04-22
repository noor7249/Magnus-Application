import { Link, useLocation, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import { Button } from '../../components/Button'
import { Input } from '../../components/Input'
import { useAuthStore } from '../../store/authStore'
import { useUiStore } from '../../store/uiStore'
import { getApiErrorMessage } from '../../utils/apiError'
import { AuthFormShell } from './AuthFormShell'

export function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const login = useAuthStore((state) => state.login)
  const { showToast, setGlobalError } = useUiStore()
  const [form, setForm] = useState({ email: '', password: '' })
  const [errors, setErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)

  const validate = () => {
    const nextErrors = {}
    if (!form.email.includes('@')) nextErrors.email = 'Enter a valid email address.'
    if (!form.password) nextErrors.password = 'Password is required.'
    setErrors(nextErrors)
    return Object.keys(nextErrors).length === 0
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    if (!validate()) return
    setSubmitting(true)
    try {
      await login(form)
      showToast('Signed in successfully.')
      navigate(location.state?.from?.pathname || '/dashboard', { replace: true })
    } catch (error) {
      setGlobalError(getApiErrorMessage(error))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthFormShell
      title="Sign in to your workspace"
      description="Use your Magnus account to manage employees, departments, and designations."
      footer={
        <>
          New here?{' '}
          <Link className="font-bold text-slate-950 underline" to="/register">
            Create an account
          </Link>
        </>
      }
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        <Input label="Email" type="email" value={form.email} error={errors.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
        <Input label="Password" type="password" value={form.password} error={errors.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
        <Button type="submit" className="w-full" disabled={submitting}>
          {submitting ? 'Signing in...' : 'Sign in'}
        </Button>
      </form>
    </AuthFormShell>
  )
}
