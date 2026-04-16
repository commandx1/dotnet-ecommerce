import { apiClient } from './client'

export interface AuthResponse {
  accessToken: string
  refreshToken: string
  refreshTokenExpiresAt: string
}

export async function login(email: string, password: string) {
  const { data } = await apiClient.post<AuthResponse>('/auth/login', { email, password })
  return data
}

export async function register(email: string, password: string, role: 'Buyer' | 'Vendor') {
  const { data } = await apiClient.post<AuthResponse>('/auth/register', { email, password, role })
  return data
}

export async function refresh(refreshToken: string) {
  const { data } = await apiClient.post<AuthResponse>('/auth/refresh', { refreshToken })
  return data
}

export async function logout(refreshToken: string | null, revokeAllSessions = false) {
  await apiClient.post('/auth/logout', { refreshToken, revokeAllSessions }, { skipAuthRefresh: true } as any)
}
