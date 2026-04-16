export const ACCESS_TOKEN_KEY = 'accessToken'
export const REFRESH_TOKEN_KEY = 'refreshToken'

export const AUTH_SESSION_UPDATED_EVENT = 'auth:session-updated'
export const AUTH_SESSION_CLEARED_EVENT = 'auth:session-cleared'

export interface StoredSession {
  accessToken: string
  refreshToken: string
}

function emitEvent(eventName: string) {
  if (typeof window === 'undefined') {
    return
  }

  window.dispatchEvent(new Event(eventName))
}

export function readSessionFromStorage(): StoredSession | null {
  const accessToken = localStorage.getItem(ACCESS_TOKEN_KEY)
  const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY)

  if (!accessToken || !refreshToken) {
    return null
  }

  return { accessToken, refreshToken }
}

export function writeSessionToStorage(accessToken: string, refreshToken: string) {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken)
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken)
  emitEvent(AUTH_SESSION_UPDATED_EVENT)
}

export function clearSessionFromStorage() {
  localStorage.removeItem(ACCESS_TOKEN_KEY)
  localStorage.removeItem(REFRESH_TOKEN_KEY)
  emitEvent(AUTH_SESSION_CLEARED_EVENT)
}
