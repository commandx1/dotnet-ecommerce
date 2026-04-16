<script setup lang="ts">
import AppHeader from '@/components/layout/AppHeader.vue'
</script>

<template>
  <div class="min-h-screen bg-background">
    <AppHeader />
    <main class="mx-auto max-w-6xl px-4 py-8 md:py-10">
      <RouterView />
    </main>
  </div>
</template>
