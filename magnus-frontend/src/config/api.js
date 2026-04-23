const API_HOST_URL = import.meta.env.VITE_API_BASE_URL

if (!API_HOST_URL) {
  throw new Error('VITE_API_BASE_URL is not defined.')
}

const API_BASE_URL = `${API_HOST_URL.replace(/\/+$/, '')}/api`

export { API_HOST_URL, API_BASE_URL }
export default API_BASE_URL
