import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'

export function ProtectedRoute({ roles = [] }) {
  const location = useLocation()
  const { isAuthenticated, hasAnyRole } = useAuthStore()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  if (!hasAnyRole(roles)) {
    return <Navigate to="/unauthorized" replace />
  }

  return <Outlet />
}
