<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
</script>

<template>
  <section class="grid gap-4 md:grid-cols-2">
    <Card>
      <h2 class="text-lg font-semibold">Vendor Home</h2>
      <p class="mt-2 text-sm text-muted-foreground">Sadece vendor yetkili endpointlere erişim içindir.</p>
    </Card>
    <Card>
      <h2 class="text-lg font-semibold">Hızlı Eylemler</h2>
      <div class="mt-3 flex flex-col gap-2">
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/products">Ürün Yönetimine Git</RouterLink>
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/orders">Sipariş Takibini Aç</RouterLink>
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/reviews">Yorum Yönetimini Aç</RouterLink>
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/questions">Soru-Cevap Yönetimini Aç</RouterLink>
      </div>
    </Card>
  </section>
</template>
