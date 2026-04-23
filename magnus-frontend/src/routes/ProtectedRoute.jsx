import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useEffect } from 'react'
import { useAuthStore } from '../store/authStore'

export function ProtectedRoute({ roles = [] }) {
  const location = useLocation()
  const { isAuthenticated, isRestoringSession, restoreSession, hasAnyRole } = useAuthStore()

  useEffect(() => {
    if (!isAuthenticated) {
      restoreSession()
    }
  }, [isAuthenticated, restoreSession])

  if (isRestoringSession) {
    return null
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  if (!hasAnyRole(roles)) {
    return <Navigate to="/unauthorized" replace />
  }

  return <Outlet />
}
