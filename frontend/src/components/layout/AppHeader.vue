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
  <header class="border-b border-border bg-white/90 backdrop-blur">
    <div class="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
      <RouterLink class="text-lg font-semibold" to="/">Dotnet Ecommerce</RouterLink>
      <nav class="flex items-center gap-2 text-sm">
        <RouterLink class="rounded px-3 py-2 hover:bg-muted" to="/products">Products</RouterLink>
        <RouterLink class="rounded px-3 py-2 hover:bg-muted" to="/cart">Cart</RouterLink>
        <RouterLink v-if="authStore.role === 'Buyer'" class="rounded px-3 py-2 hover:bg-muted" to="/orders">
          My Orders
        </RouterLink>
        <RouterLink v-if="authStore.role === 'Vendor'" class="rounded px-3 py-2 hover:bg-muted" to="/vendor">
          Vendor
        </RouterLink>
        <RouterLink v-if="!authStore.isAuthenticated" class="rounded px-3 py-2 hover:bg-muted" to="/login"
          >Login</RouterLink
        >
        <Button v-else variant="outline" size="sm" @click="logoutCurrentSession">Logout</Button>
        <Button
          v-if="authStore.isAuthenticated"
          :disabled="isRevokingAllSessions"
          variant="outline"
          size="sm"
          @click="logoutAllSessions"
        >
          Tüm Cihazlardan Çık
        </Button>
      </nav>
    </div>
  </header>
</template>
