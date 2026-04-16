<script setup lang="ts">
import Button from '@/components/ui/Button.vue'
</script>

<template>
  <section class="grid gap-6 rounded-lg border border-border bg-white p-8 md:grid-cols-2 md:items-center">
    <div class="space-y-4">
      <p class="inline-flex rounded-full bg-muted px-3 py-1 text-sm text-muted-foreground">MVP-2 Storefront</p>
      <h2 class="text-3xl font-bold tracking-tight">Buyer + Vendor workflow tek omurgada</h2>
      <p class="text-muted-foreground">
        Dotnet API, PostgreSQL ve Vue arayüzü ile rol bazlı akışı tek komutta ayağa kaldıran e-ticaret başlangıç
        paketi.
      </p>
      <div class="flex gap-2">
        <RouterLink to="/products"><Button>Ürünleri Gör</Button></RouterLink>
        <RouterLink to="/vendor"><Button variant="outline">Vendor Panel</Button></RouterLink>
      </div>
    </div>
    <div class="rounded-lg border border-border bg-gradient-to-br from-white to-slate-100 p-6">
      <p class="text-sm font-medium">Seed kullanıcılar</p>
      <ul class="mt-4 space-y-2 text-sm text-muted-foreground">
        <li>`buyer@local.dev / Passw0rd!`</li>
        <li>`vendor@local.dev / Passw0rd!`</li>
      </ul>
    </div>
  </section>
</template>
