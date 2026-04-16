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

export function getApiErrorMessage(error: unknown, fallback = 'İşlem sırasında bir hata oluştu.'): string {
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
    return 'Sunucuya ulaşılamadı. Ağ bağlantınızı kontrol edin.'
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
    return 'Oturum doğrulanamadı. Lütfen tekrar giriş yapın.'
  }

  if (error.response.status === 403) {
    return 'Bu işlem için yetkiniz yok.'
  }

  if (error.response.status === 404) {
    return 'İstenen kaynak bulunamadı.'
  }

  if (error.response.status === 429) {
    return 'Çok fazla istek gönderdiniz. Lütfen biraz sonra tekrar deneyin.'
  }

  return fallback
}
