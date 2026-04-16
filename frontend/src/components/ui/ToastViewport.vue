<script setup lang="ts">
import { onMounted, onUnmounted, shallowRef } from 'vue'
import { APP_TOAST_EVENT, type ToastPayload } from '@/lib/toast'

interface ToastItem {
  id: string
  title: string
  message: string
  variant: 'error' | 'success' | 'info'
}

const toasts = shallowRef<ToastItem[]>([])
const timeoutHandles = new Map<string, number>()

function removeToast(id: string) {
  const timeoutHandle = timeoutHandles.get(id)
  if (timeoutHandle) {
    window.clearTimeout(timeoutHandle)
    timeoutHandles.delete(id)
  }

  toasts.value = toasts.value.filter((toast) => toast.id !== id)
}

function enqueueToast(payload: ToastPayload) {
  const id = `${Date.now()}-${Math.random().toString(16).slice(2)}`
  const toast: ToastItem = {
    id,
    title: payload.title,
    message: payload.message,
    variant: payload.variant ?? 'info'
  }

  toasts.value = [...toasts.value.slice(-3), toast]

  const timeoutHandle = window.setTimeout(() => {
    removeToast(id)
  }, payload.durationMs ?? 4500)

  timeoutHandles.set(id, timeoutHandle)
}

function onToast(event: Event) {
  const customEvent = event as CustomEvent<ToastPayload>
  if (!customEvent.detail?.message) {
    return
  }

  enqueueToast(customEvent.detail)
}

onMounted(() => {
  window.addEventListener(APP_TOAST_EVENT, onToast)
})

onUnmounted(() => {
  window.removeEventListener(APP_TOAST_EVENT, onToast)
  timeoutHandles.forEach((timeoutHandle) => window.clearTimeout(timeoutHandle))
  timeoutHandles.clear()
})
</script>

<template>
  <section class="pointer-events-none fixed right-4 top-4 z-50 flex w-[min(92vw,24rem)] flex-col gap-2">
    <TransitionGroup name="toast">
      <article
        v-for="toast in toasts"
        :key="toast.id"
        class="pointer-events-auto rounded-xl border border-border/80 bg-card/95 p-3 shadow-xl backdrop-blur"
      >
        <div class="flex items-start justify-between gap-2">
          <div class="space-y-1">
            <p
              class="text-sm font-semibold"
              :class="{
                'text-rose-700': toast.variant === 'error',
                'text-emerald-700': toast.variant === 'success',
                'text-slate-800': toast.variant === 'info'
              }"
            >
              {{ toast.title }}
            </p>
            <p class="text-sm text-muted-foreground">{{ toast.message }}</p>
          </div>
          <button class="rounded px-2 py-1 text-xs text-muted-foreground hover:bg-muted" @click="removeToast(toast.id)">
            Close
          </button>
        </div>
      </article>
    </TransitionGroup>
  </section>
</template>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.2s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
