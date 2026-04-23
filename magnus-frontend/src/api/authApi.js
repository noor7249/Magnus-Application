import axios from 'axios'
import API_BASE_URL from '../config/api'
import { unwrapApiResponse } from './httpClient'

const authClient = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
})

export const authApi = {
  login: (payload) => unwrapApiResponse(authClient.post('/auth/login', payload)),
  register: (payload) => unwrapApiResponse(authClient.post('/auth/register', payload)),
  refreshToken: (payload) => unwrapApiResponse(authClient.post('/auth/refresh-token', payload)),
}
