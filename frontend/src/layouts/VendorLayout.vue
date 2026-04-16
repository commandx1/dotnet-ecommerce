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
    <header class="sticky top-0 z-40 border-b border-border/80 bg-white/75 backdrop-blur-xl">
      <div class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-3 px-4 py-4">
        <h1 class="display-font text-2xl font-semibold text-slate-800">Vendor Panel</h1>
        <nav class="flex flex-wrap items-center gap-2 text-sm">
          <RouterLink class="rounded-lg border border-border/90 bg-white/75 px-3 py-2 hover:bg-muted/85" to="/vendor/products">
            Ürünler
          </RouterLink>
          <RouterLink class="rounded-lg border border-border/90 bg-white/75 px-3 py-2 hover:bg-muted/85" to="/vendor/orders">
            Siparişler
          </RouterLink>
          <RouterLink class="rounded-lg border border-border/90 bg-white/75 px-3 py-2 hover:bg-muted/85" to="/vendor/reviews">
            Yorumlar
          </RouterLink>
          <RouterLink class="rounded-lg border border-border/90 bg-white/75 px-3 py-2 hover:bg-muted/85" to="/vendor/questions">
            Sorular
          </RouterLink>
          <RouterLink class="rounded-lg border border-border/90 bg-white/75 px-3 py-2 hover:bg-muted/85" to="/">Marketplace</RouterLink>
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
