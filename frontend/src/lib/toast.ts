export const APP_TOAST_EVENT = 'app:toast'

export interface ToastPayload {
  title: string
  message: string
  items?: string[]
  variant?: 'error' | 'success' | 'info'
  durationMs?: number
}

export function showToast(payload: ToastPayload) {
  if (typeof window === 'undefined') {
    return
  }

  window.dispatchEvent(new CustomEvent<ToastPayload>(APP_TOAST_EVENT, { detail: payload }))
}

export function showErrorToast(message: string, title = 'Error', items?: string[]) {
  showToast({ title, message, items, variant: 'error' })
}
