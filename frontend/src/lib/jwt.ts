export type UserRole = 'Buyer' | 'Vendor'

interface JwtPayload {
  [key: string]: unknown
}

const ROLE_CLAIM_KEYS = [
  'role',
  'roles',
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
]

const USER_ID_CLAIM_KEYS = ['sub', 'nameid', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']

function parseJwtPayload(token: string): JwtPayload | null {
  try {
    const parts = token.split('.')
    if (parts.length < 2) {
      return null
    }

    const base64Url = parts[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const paddedBase64 = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=')
    const json = atob(paddedBase64)
    return JSON.parse(json) as JwtPayload
  } catch {
    return null
  }
}

function normalizeRole(value: unknown): UserRole | null {
  if (value === 'Buyer' || value === 'Vendor') {
    return value
  }

  return null
}

function extractRole(payload: JwtPayload): UserRole | null {
  for (const key of ROLE_CLAIM_KEYS) {
    const value = payload[key]

    if (Array.isArray(value)) {
      for (const item of value) {
        const role = normalizeRole(item)
        if (role) {
          return role
        }
      }
    }

    const role = normalizeRole(value)
    if (role) {
      return role
    }
  }

  return null
}

function extractUserId(payload: JwtPayload): string | null {
  for (const key of USER_ID_CLAIM_KEYS) {
    const value = payload[key]
    if (typeof value === 'string' && value.length > 0) {
      return value
    }
  }

  return null
}

export function decodeAuthContext(token: string): { role: UserRole | null; userId: string | null } {
  const payload = parseJwtPayload(token)
  if (!payload) {
    return { role: null, userId: null }
  }

  return {
    role: extractRole(payload),
    userId: extractUserId(payload)
  }
}
