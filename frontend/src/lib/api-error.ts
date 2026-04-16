import axios from 'axios'

export interface ResolvedApiError {
  message: string
  items: string[]
}

function extractMessages(input: unknown): string[] {
  if (!input) {
    return []
  }

  if (typeof input === 'string') {
    const normalized = input.trim()
    return normalized ? [normalized] : []
  }

  if (Array.isArray(input)) {
    return input.flatMap((item) => extractMessages(item))
  }

  if (typeof input === 'object') {
    const candidate = input as Record<string, unknown>
    const preferredKeys = ['description', 'message', 'detail', 'title', 'error']
    const preferredMessages = preferredKeys.flatMap((key) => extractMessages(candidate[key]))

    if (preferredMessages.length > 0) {
      return preferredMessages
    }

    return Object.values(candidate).flatMap((value) => extractMessages(value))
  }

  return []
}

function dedupeMessages(messages: string[]) {
  return [...new Set(messages.map((message) => message.trim()).filter(Boolean))]
}

function getStatusFallback(status: number, fallback: string) {
  if (status === 401) {
    return 'Your session could not be verified. Please sign in again.'
  }

  if (status === 403) {
    return 'You do not have permission to perform this action.'
  }

  if (status === 404) {
    return 'The requested resource could not be found.'
  }

  if (status === 429) {
    return 'Too many requests. Please try again in a moment.'
  }

  return fallback
}

export function resolveApiError(error: unknown, fallback = 'Something went wrong while processing your request.'): ResolvedApiError {
  if (!axios.isAxiosError(error)) {
    if (error instanceof Error && error.message.trim()) {
      return {
        message: error.message,
        items: []
      }
    }

    return {
      message: fallback,
      items: []
    }
  }

  const responseData = error.response?.data as
    | {
        message?: string
        title?: string
        detail?: string
        errors?: unknown
      }
    | string
    | unknown[]
    | undefined

  if (!error.response) {
    return {
      message: 'Unable to reach the server. Please check your network connection.',
      items: []
    }
  }

  if (typeof responseData === 'string' && responseData.trim()) {
    return {
      message: responseData,
      items: []
    }
  }

  if (responseData && typeof responseData === 'object') {
    const payload = responseData as Record<string, unknown>
    const topLevelItems = Array.isArray(responseData) ? extractMessages(responseData) : []
    const nestedItems = extractMessages(payload.errors)
    const items = dedupeMessages([...topLevelItems, ...nestedItems])

    const primaryMessages = dedupeMessages([
      ...extractMessages(payload.message),
      ...extractMessages(payload.detail),
      ...extractMessages(payload.title)
    ])

    if (items.length > 1) {
      return {
        message: primaryMessages[0] ?? 'Please review the following issues:',
        items
      }
    }

    if (items.length === 1) {
      return {
        message: items[0],
        items: []
      }
    }

    if (primaryMessages.length > 0) {
      return {
        message: primaryMessages[0],
        items: []
      }
    }
  }

  return {
    message: getStatusFallback(error.response.status, fallback),
    items: []
  }
}

export function getApiErrorMessage(error: unknown, fallback = 'Something went wrong while processing your request.'): string {
  const resolved = resolveApiError(error, fallback)
  if (resolved.items.length > 0) {
    return resolved.items.join(' ')
  }

  return resolved.message
}
