export const ACCESS_TOKEN_KEY = 'accessToken'
export const REFRESH_TOKEN_KEY = 'refreshToken'

export const AUTH_SESSION_UPDATED_EVENT = 'auth:session-updated'
export const AUTH_SESSION_CLEARED_EVENT = 'auth:session-cleared'
export const AUTH_SESSION_EXPIRED_EVENT = 'auth:session-expired'

export type SessionPersistence = 'persistent' | 'session'

export interface StoredSession {
  accessToken: string
  refreshToken: string
  persistence: SessionPersistence
}

function emitEvent(eventName: string) {
  if (typeof window === 'undefined') {
    return
  }

  window.dispatchEvent(new Event(eventName))
}

function readSessionFrom(storage: Storage, persistence: SessionPersistence): StoredSession | null {
  const accessToken = storage.getItem(ACCESS_TOKEN_KEY)
  const refreshToken = storage.getItem(REFRESH_TOKEN_KEY)

  if (!accessToken || !refreshToken) {
    return null
  }

  return { accessToken, refreshToken, persistence }
}

export function readSessionFromStorage(): StoredSession | null {
  const persistentSession = readSessionFrom(localStorage, 'persistent')
  if (persistentSession) {
    return persistentSession
  }

  return readSessionFrom(sessionStorage, 'session')
}

export function writeSessionToStorage(
  accessToken: string,
  refreshToken: string,
  persistence: SessionPersistence = 'persistent'
) {
  localStorage.removeItem(ACCESS_TOKEN_KEY)
  localStorage.removeItem(REFRESH_TOKEN_KEY)
  sessionStorage.removeItem(ACCESS_TOKEN_KEY)
  sessionStorage.removeItem(REFRESH_TOKEN_KEY)

  const storage = persistence === 'session' ? sessionStorage : localStorage
  storage.setItem(ACCESS_TOKEN_KEY, accessToken)
  storage.setItem(REFRESH_TOKEN_KEY, refreshToken)

  emitEvent(AUTH_SESSION_UPDATED_EVENT)
}

export function clearSessionFromStorage() {
  localStorage.removeItem(ACCESS_TOKEN_KEY)
  localStorage.removeItem(REFRESH_TOKEN_KEY)
  sessionStorage.removeItem(ACCESS_TOKEN_KEY)
  sessionStorage.removeItem(REFRESH_TOKEN_KEY)
  emitEvent(AUTH_SESSION_CLEARED_EVENT)
}

export function emitSessionExpiredEvent() {
  emitEvent(AUTH_SESSION_EXPIRED_EVENT)
}
