<script setup lang="ts">
import Button from '@/components/ui/Button.vue'
</script>

<template>
  <section
    class="grid gap-6 rounded-3xl border border-border/80 bg-white/75 p-8 shadow-[0_22px_60px_-36px_rgba(10,39,76,0.5)] backdrop-blur md:grid-cols-2 md:items-center"
  >
    <div class="space-y-4">
      <p class="inline-flex rounded-full border border-border/90 bg-white/75 px-3 py-1 text-xs font-semibold tracking-[0.12em] text-muted-foreground">
        MVP-2 STOREFRONT
      </p>
      <h2 class="display-font text-4xl leading-tight tracking-tight text-slate-800">
        Unify buyer and vendor workflows on a single commerce backbone.
      </h2>
      <p class="text-muted-foreground">
        A role-aware ecommerce starter built with a .NET API, PostgreSQL, and Vue, designed to boot with a single
        command.
      </p>
      <div class="flex flex-wrap gap-2">
        <RouterLink to="/products"><Button>Browse Products</Button></RouterLink>
        <RouterLink to="/vendor"><Button variant="outline">Vendor Panel</Button></RouterLink>
      </div>
    </div>

    <div class="rounded-2xl border border-border/80 bg-gradient-to-br from-cyan-50 to-sky-100/80 p-6">
      <p class="text-sm font-semibold text-slate-700">Quick Start</p>
      <ul class="mt-4 space-y-2 text-sm text-slate-700/90">
        <li>Create a new account from the login screen.</li>
        <li>Select either the Buyer or Vendor role during registration.</li>
      </ul>
    </div>
  </section>
</template>
