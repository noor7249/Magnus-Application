import { httpClient, unwrapApiResponse } from './httpClient'

export const employeesApi = {
  list: (params) => unwrapApiResponse(httpClient.get('/employees', { params })),
  getById: (id) => unwrapApiResponse(httpClient.get(`/employees/${id}`)),
  create: (payload) => unwrapApiResponse(httpClient.post('/employees', payload)),
  update: (id, payload) => unwrapApiResponse(httpClient.put(`/employees/${id}`, payload)),
  remove: (id) => unwrapApiResponse(httpClient.delete(`/employees/${id}`)),
}
