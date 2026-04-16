<script setup lang="ts">
import Button from '@/components/ui/Button.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
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
        <Button v-else variant="outline" size="sm" @click="authStore.logout">Logout</Button>
      </nav>
    </div>
  </header>
</template>
