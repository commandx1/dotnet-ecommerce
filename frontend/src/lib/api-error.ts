import axios from 'axios'

function flattenErrors(input: unknown): string[] {
  if (!input) {
    return []
  }

  if (Array.isArray(input)) {
    return input.filter((item): item is string => typeof item === 'string' && item.trim().length > 0)
  }

  if (typeof input === 'object') {
    return Object.values(input).flatMap((value) => flattenErrors(value))
  }

  return []
}

export function getApiErrorMessage(error: unknown, fallback = 'Something went wrong while processing your request.'): string {
  if (!axios.isAxiosError(error)) {
    if (error instanceof Error && error.message.trim()) {
      return error.message
    }

    return fallback
  }

  const responseData = error.response?.data as
    | {
        message?: string
        title?: string
        detail?: string
        errors?: unknown
      }
    | string
    | undefined

  if (!error.response) {
    return 'Unable to reach the server. Please check your network connection.'
  }

  if (typeof responseData === 'string' && responseData.trim()) {
    return responseData
  }

  if (responseData && typeof responseData === 'object') {
    const validationErrors = flattenErrors(responseData.errors)
    if (validationErrors.length > 0) {
      return validationErrors.join(' ')
    }

    const primary = responseData.message ?? responseData.detail ?? responseData.title
    if (typeof primary === 'string' && primary.trim()) {
      return primary
    }
  }

  if (error.response.status === 401) {
    return 'Your session could not be verified. Please sign in again.'
  }

  if (error.response.status === 403) {
    return 'You do not have permission to perform this action.'
  }

  if (error.response.status === 404) {
    return 'The requested resource could not be found.'
  }

  if (error.response.status === 429) {
    return 'Too many requests. Please try again in a moment.'
  }

  return fallback
}
