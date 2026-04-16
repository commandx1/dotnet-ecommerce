<script setup lang="ts"></script>

<template>
  <div class="min-h-screen bg-background">
    <header class="border-b border-border bg-white">
      <div class="mx-auto flex max-w-6xl items-center justify-between px-4 py-4">
        <h1 class="text-xl font-semibold">Vendor Panel</h1>
        <RouterLink class="rounded border border-border px-3 py-2 text-sm hover:bg-muted" to="/">Marketplace</RouterLink>
      </div>
    </header>
    <main class="mx-auto max-w-6xl px-4 py-8">
      <RouterView />
    </main>
  </div>
</template>
