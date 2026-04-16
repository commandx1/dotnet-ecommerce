<script setup lang="ts">
import { shallowRef } from 'vue'
import Button from '@/components/ui/Button.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const isRevokingAllSessions = shallowRef(false)

async function logoutCurrentSession() {
  await authStore.logout()
}

async function logoutAllSessions() {
  if (isRevokingAllSessions.value) {
    return
  }

  isRevokingAllSessions.value = true
  try {
    await authStore.logout({ revokeAllSessions: true })
  } finally {
    isRevokingAllSessions.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-background">
    <header class="border-b border-border bg-white">
      <div class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-3 px-4 py-4">
        <h1 class="text-xl font-semibold">Vendor Panel</h1>
        <nav class="flex items-center gap-2 text-sm">
          <RouterLink class="rounded border border-border px-3 py-2 hover:bg-muted" to="/vendor/products">
            Ürünler
          </RouterLink>
          <RouterLink class="rounded border border-border px-3 py-2 hover:bg-muted" to="/vendor/orders">
            Siparişler
          </RouterLink>
          <RouterLink class="rounded border border-border px-3 py-2 hover:bg-muted" to="/vendor/reviews">
            Yorumlar
          </RouterLink>
          <RouterLink class="rounded border border-border px-3 py-2 hover:bg-muted" to="/vendor/questions">
            Sorular
          </RouterLink>
          <RouterLink class="rounded border border-border px-3 py-2 hover:bg-muted" to="/">Marketplace</RouterLink>
          <Button variant="outline" size="sm" @click="logoutCurrentSession">Çıkış Yap</Button>
          <Button :disabled="isRevokingAllSessions" variant="outline" size="sm" @click="logoutAllSessions">
            Tüm Cihazlardan Çık
          </Button>
        </nav>
      </div>
    </header>
    <main class="mx-auto max-w-6xl px-4 py-8">
      <RouterView />
    </main>
  </div>
</template>
