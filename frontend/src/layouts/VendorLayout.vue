<script setup lang="ts"></script>

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
        </nav>
      </div>
    </header>
    <main class="mx-auto max-w-6xl px-4 py-8">
      <RouterView />
    </main>
  </div>
</template>
