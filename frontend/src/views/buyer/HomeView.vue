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
        Buyer ve Vendor akışını tek omurgada birleştir.
      </h2>
      <p class="text-muted-foreground">
        Dotnet API, PostgreSQL ve Vue arayüzü ile rol bazlı akışı tek komutta ayağa kaldıran e-ticaret başlangıç
        paketi.
      </p>
      <div class="flex flex-wrap gap-2">
        <RouterLink to="/products"><Button>Ürünleri Gör</Button></RouterLink>
        <RouterLink to="/vendor"><Button variant="outline">Vendor Panel</Button></RouterLink>
      </div>
    </div>

    <div class="rounded-2xl border border-border/80 bg-gradient-to-br from-cyan-50 to-sky-100/80 p-6">
      <p class="text-sm font-semibold text-slate-700">Hızlı Başlangıç</p>
      <ul class="mt-4 space-y-2 text-sm text-slate-700/90">
        <li>Yeni kullanıcı oluşturmak için Login ekranından kayıt ol.</li>
        <li>Kayıt sırasında rol seç: Buyer veya Vendor.</li>
      </ul>
    </div>
  </section>
</template>
