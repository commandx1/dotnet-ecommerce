<script setup lang="ts">
import { shallowRef } from 'vue'
import { useRouter } from 'vue-router'
import Button from '@/components/ui/Button.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()
const isRevokingAllSessions = shallowRef(false)

async function logoutCurrentSession() {
  await authStore.logout()
  await router.push({ name: 'login' })
}

async function logoutAllSessions() {
  if (isRevokingAllSessions.value) {
    return
  }

  isRevokingAllSessions.value = true
  try {
    await authStore.logout({ revokeAllSessions: true })
    await router.push({ name: 'login' })
  } finally {
    isRevokingAllSessions.value = false
  }
}
</script>

<template>
  <header class="sticky top-0 z-40 border-b border-border/80 bg-white/75 backdrop-blur-xl">
    <div class="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-2 px-4 py-3">
      <RouterLink class="display-font text-xl font-semibold text-slate-800" to="/">Dotnet Commerce</RouterLink>
      <nav class="flex flex-wrap items-center gap-2 text-sm">
        <RouterLink class="rounded-lg px-3 py-2 hover:bg-muted/85" to="/products">Products</RouterLink>
        <RouterLink class="rounded-lg px-3 py-2 hover:bg-muted/85" to="/cart">Cart</RouterLink>
        <RouterLink v-if="authStore.role === 'Buyer'" class="rounded-lg px-3 py-2 hover:bg-muted/85" to="/orders">
          My Orders
        </RouterLink>
        <RouterLink v-if="authStore.role === 'Vendor'" class="rounded-lg px-3 py-2 hover:bg-muted/85" to="/vendor">
          Vendor
        </RouterLink>
        <RouterLink v-if="!authStore.isAuthenticated" class="rounded-lg px-3 py-2 hover:bg-muted/85" to="/login"
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
          Sign Out Everywhere
        </Button>
      </nav>
    </div>
  </header>
</template>
