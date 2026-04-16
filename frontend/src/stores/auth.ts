import { defineStore } from 'pinia'
import { computed, shallowRef } from 'vue'
import * as authApi from '@/api/authApi'

type UserRole = 'Buyer' | 'Vendor' | null

export const useAuthStore = defineStore('auth', () => {
  const accessToken = shallowRef<string | null>(localStorage.getItem('accessToken'))
  const refreshToken = shallowRef<string | null>(localStorage.getItem('refreshToken'))
  const role = shallowRef<UserRole>(null)

  const isAuthenticated = computed(() => Boolean(accessToken.value))

  async function login(email: string, password: string) {
    const response = await authApi.login(email, password)
    accessToken.value = response.accessToken
    refreshToken.value = response.refreshToken
    localStorage.setItem('accessToken', response.accessToken)
    localStorage.setItem('refreshToken', response.refreshToken)
  }

  async function register(email: string, password: string, selectedRole: 'Buyer' | 'Vendor') {
    const response = await authApi.register(email, password, selectedRole)
    accessToken.value = response.accessToken
    refreshToken.value = response.refreshToken
    role.value = selectedRole
    localStorage.setItem('accessToken', response.accessToken)
    localStorage.setItem('refreshToken', response.refreshToken)
  }

  function logout() {
    accessToken.value = null
    refreshToken.value = null
    role.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
  }

  return {
    accessToken,
    refreshToken,
    role,
    isAuthenticated,
    login,
    register,
    logout
  }
})
