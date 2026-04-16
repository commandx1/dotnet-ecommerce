import axios, { AxiosError, AxiosHeaders, type InternalAxiosRequestConfig } from 'axios'
import {
  clearSessionFromStorage,
  emitSessionExpiredEvent,
  readSessionFromStorage,
  writeSessionToStorage
} from '@/lib/session'
import type { AuthResponse } from './authApi'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? '/api'

type RetryableRequestConfig = InternalAxiosRequestConfig & {
  _retry?: boolean
  skipAuthRefresh?: boolean
}

const refreshClient = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json'
  }
})

export const apiClient = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json'
  }
})

let refreshPromise: Promise<AuthResponse | null> | null = null

function isAuthEndpoint(url: string | undefined) {
  if (!url) {
    return false
  }

  return ['/auth/login', '/auth/register', '/auth/refresh', '/auth/logout'].some((authPath) =>
    url.includes(authPath)
  )
}

async function refreshSession(): Promise<AuthResponse | null> {
  const currentSession = readSessionFromStorage()
  if (!currentSession?.refreshToken) {
    clearSessionFromStorage()
    return null
  }

  try {
    const { data } = await refreshClient.post<AuthResponse>('/auth/refresh', {
      refreshToken: currentSession.refreshToken
    })

    writeSessionToStorage(data.accessToken, data.refreshToken, currentSession.persistence)
    return data
  } catch {
    clearSessionFromStorage()
    emitSessionExpiredEvent()
    return null
  }
}

apiClient.interceptors.request.use((config) => {
  const session = readSessionFromStorage()

  if (!config.headers) {
    config.headers = new AxiosHeaders()
  }

  if (session?.accessToken) {
    config.headers.Authorization = `Bearer ${session.accessToken}`
  }

  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableRequestConfig | undefined
    const status = error.response?.status

    if (!originalRequest || status !== 401 || originalRequest.skipAuthRefresh || originalRequest._retry) {
      return Promise.reject(error)
    }

    if (isAuthEndpoint(originalRequest.url)) {
      return Promise.reject(error)
    }

    originalRequest._retry = true

    if (!refreshPromise) {
      refreshPromise = refreshSession().finally(() => {
        refreshPromise = null
      })
    }

    const refreshedTokens = await refreshPromise
    if (!refreshedTokens) {
      return Promise.reject(error)
    }

    if (!originalRequest.headers) {
      originalRequest.headers = new AxiosHeaders()
    }

    originalRequest.headers.Authorization = `Bearer ${refreshedTokens.accessToken}`

    return apiClient(originalRequest)
  }
)
