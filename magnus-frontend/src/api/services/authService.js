import axios from 'axios'
import API_BASE_URL from '../../config/api'
import { attachCsrfToken, unwrapApiResponse } from '../httpClient'

const authClient = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
})

authClient.interceptors.request.use(attachCsrfToken)

export const authApi = {
  login: (payload) => unwrapApiResponse(authClient.post('/Auth/login', payload)),
  register: (payload) => unwrapApiResponse(authClient.post('/Auth/register', payload)),
  refreshToken: () => unwrapApiResponse(authClient.post('/Auth/refresh')),
  logout: () => unwrapApiResponse(authClient.post('/Auth/logout')),
}
