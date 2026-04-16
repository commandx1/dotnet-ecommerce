import { defineStore } from 'pinia'
import { computed, shallowRef } from 'vue'
import * as authApi from '@/api/authApi'
import { decodeAuthContext, type UserRole } from '@/lib/jwt'
import {
  AUTH_SESSION_CLEARED_EVENT,
  AUTH_SESSION_UPDATED_EVENT,
  clearSessionFromStorage,
  readSessionFromStorage,
  writeSessionToStorage
} from '@/lib/session'

type NullableRole = UserRole | null

export const useAuthStore = defineStore('auth', () => {
  const accessToken = shallowRef<string | null>(null)
  const refreshToken = shallowRef<string | null>(null)
  const role = shallowRef<NullableRole>(null)
  const userId = shallowRef<string | null>(null)

  const isAuthenticated = computed(() => Boolean(accessToken.value))
  const landingPath = computed(() => (role.value === 'Vendor' ? '/vendor' : '/orders'))

  function applySession(newAccessToken: string, newRefreshToken: string) {
    writeSessionToStorage(newAccessToken, newRefreshToken)
    syncFromStorage()
  }

  function syncFromStorage() {
    const session = readSessionFromStorage()
    if (!session) {
      accessToken.value = null
      refreshToken.value = null
      role.value = null
      userId.value = null
      return
    }

    accessToken.value = session.accessToken
    refreshToken.value = session.refreshToken

    const context = decodeAuthContext(session.accessToken)
    role.value = context.role
    userId.value = context.userId
  }

  async function login(email: string, password: string) {
    const response = await authApi.login(email, password)
    applySession(response.accessToken, response.refreshToken)
  }

  async function register(email: string, password: string, selectedRole: 'Buyer' | 'Vendor') {
    const response = await authApi.register(email, password, selectedRole)
    applySession(response.accessToken, response.refreshToken)
    role.value = selectedRole
  }

  function clearSession() {
    clearSessionFromStorage()
    syncFromStorage()
  }

  async function logout(options?: { skipServerLogout?: boolean; revokeAllSessions?: boolean }) {
    if (!options?.skipServerLogout && accessToken.value) {
      try {
        await authApi.logout(refreshToken.value, options?.revokeAllSessions ?? false)
      } catch {
        // Client-side session must still be cleared even if API revoke fails.
      }
    }

    clearSession()
  }

  syncFromStorage()

  if (typeof window !== 'undefined') {
    window.addEventListener(AUTH_SESSION_UPDATED_EVENT, syncFromStorage)
    window.addEventListener(AUTH_SESSION_CLEARED_EVENT, syncFromStorage)
  }

  return {
    accessToken,
    refreshToken,
    role,
    userId,
    isAuthenticated,
    landingPath,
    login,
    register,
    clearSession,
    logout,
    applySession
  }
})
