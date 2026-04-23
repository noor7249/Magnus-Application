import axios from 'axios'
import API_BASE_URL from '../config/api'
import { useAuthStore } from '../store/authStore'
import { useUiStore } from '../store/uiStore'
import { getApiErrorMessage } from '../utils/apiError'

export const httpClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
})

let refreshPromise = null
const CSRF_COOKIE_NAME = 'XSRF-TOKEN'
const CSRF_HEADER_NAME = 'X-XSRF-TOKEN'

export function getCookie(name) {
  return document.cookie
    .split('; ')
    .find((row) => row.startsWith(`${name}=`))
    ?.split('=')[1]
}

export function attachCsrfToken(config) {
  const method = config.method?.toUpperCase()
  if (method && !['GET', 'HEAD', 'OPTIONS'].includes(method)) {
    const csrfToken = getCookie(CSRF_COOKIE_NAME)
    if (csrfToken) {
      config.headers[CSRF_HEADER_NAME] = decodeURIComponent(csrfToken)
    }
  }

  return config
}

httpClient.interceptors.request.use((config) => {
  attachCsrfToken(config)
  const token = useAuthStore.getState().token
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

httpClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    const normalizedUrl = originalRequest?.url?.toLowerCase()
    const isAuthRequest = normalizedUrl?.includes('/auth/login') || normalizedUrl?.includes('/auth/register') || normalizedUrl?.includes('/auth/refresh') || normalizedUrl?.includes('/auth/refresh-token')

    if (error.response?.status === 401 && originalRequest && !originalRequest._retry && !isAuthRequest) {
      originalRequest._retry = true

      try {
        refreshPromise ||= useAuthStore.getState().refreshToken()
        const session = await refreshPromise
        refreshPromise = null

        if (!session?.accessToken) {
          throw new Error('Session refresh failed.')
        }

        originalRequest.headers.Authorization = `Bearer ${session.accessToken}`
        return httpClient(originalRequest)
      } catch (refreshError) {
        refreshPromise = null
        useAuthStore.getState().clearSession()
        // Centralized 401 handling avoids leaving stale sessions active in the UI.
        useUiStore.getState().setGlobalError('Your session expired. Please sign in again.')
        return Promise.reject(refreshError)
      }
    }

    const message = getApiErrorMessage(error)
    useUiStore.getState().setGlobalError(message)
    return Promise.reject(error)
  },
)

export async function unwrapApiResponse(request) {
  const { data } = await request
  if (data?.success === false) {
    throw new Error(data.message || 'Request failed.')
  }
  return data?.data ?? data
}
