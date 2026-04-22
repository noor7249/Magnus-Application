import axios from 'axios'
import { useAuthStore } from '../store/authStore'
import { useUiStore } from '../store/uiStore'
import { getApiErrorMessage } from '../utils/apiError'

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5298/api'

export const httpClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

let refreshPromise = null

httpClient.interceptors.request.use((config) => {
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

    const isAuthRequest = originalRequest?.url?.includes('/auth/login') || originalRequest?.url?.includes('/auth/register') || originalRequest?.url?.includes('/auth/refresh-token')

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
        useAuthStore.getState().logout()
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
