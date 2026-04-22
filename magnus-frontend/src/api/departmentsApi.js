import { httpClient, unwrapApiResponse } from './httpClient'

export const departmentsApi = {
  list: (params) => unwrapApiResponse(httpClient.get('/departments', { params })),
  getById: (id) => unwrapApiResponse(httpClient.get(`/departments/${id}`)),
  create: (payload) => unwrapApiResponse(httpClient.post('/departments', payload)),
  update: (id, payload) => unwrapApiResponse(httpClient.put(`/departments/${id}`, payload)),
  remove: (id) => unwrapApiResponse(httpClient.delete(`/departments/${id}`)),
}
