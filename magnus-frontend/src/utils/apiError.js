export function getApiErrorMessage(error) {
  const response = error?.response?.data

  if (Array.isArray(response?.errors) && response.errors.length > 0) {
    return response.errors.join(' ')
  }

  return response?.message || error?.message || 'Something went wrong. Please try again.'
}
