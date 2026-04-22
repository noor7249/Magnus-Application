import { httpClient, unwrapApiResponse } from './httpClient'

export const designationsApi = {
  list: (params) => unwrapApiResponse(httpClient.get('/designations', { params })),
  getById: (id) => unwrapApiResponse(httpClient.get(`/designations/${id}`)),
  create: (payload) => unwrapApiResponse(httpClient.post('/designations', payload)),
  update: (id, payload) => unwrapApiResponse(httpClient.put(`/designations/${id}`, payload)),
  remove: (id) => unwrapApiResponse(httpClient.delete(`/designations/${id}`)),
}
