import { defineStore } from 'pinia'
import { computed, shallowRef } from 'vue'
import * as authApi from '@/api/authApi'
import { decodeAuthContext, type UserRole } from '@/lib/jwt'

type NullableRole = UserRole | null

export const useAuthStore = defineStore('auth', () => {
  const accessToken = shallowRef<string | null>(localStorage.getItem('accessToken'))
  const refreshToken = shallowRef<string | null>(localStorage.getItem('refreshToken'))
  const role = shallowRef<NullableRole>(null)
  const userId = shallowRef<string | null>(null)

  const isAuthenticated = computed(() => Boolean(accessToken.value))
  const landingPath = computed(() => (role.value === 'Vendor' ? '/vendor' : '/orders'))

  function applySession(newAccessToken: string, newRefreshToken: string) {
    accessToken.value = newAccessToken
    refreshToken.value = newRefreshToken

    const context = decodeAuthContext(newAccessToken)
    role.value = context.role
    userId.value = context.userId

    localStorage.setItem('accessToken', newAccessToken)
    localStorage.setItem('refreshToken', newRefreshToken)
  }

  function restoreSessionFromStorage() {
    if (!accessToken.value) {
      return
    }

    const context = decodeAuthContext(accessToken.value)
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

  function logout() {
    accessToken.value = null
    refreshToken.value = null
    role.value = null
    userId.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
  }

  restoreSessionFromStorage()

  return {
    accessToken,
    refreshToken,
    role,
    userId,
    isAuthenticated,
    landingPath,
    login,
    register,
    logout,
    applySession
  }
})
