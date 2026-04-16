<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
</script>

<template>
  <section class="grid gap-4 md:grid-cols-2">
    <Card>
      <h2 class="text-lg font-semibold">Vendor Home</h2>
      <p class="mt-2 text-sm text-muted-foreground">This area is reserved for vendor-only workflows and protected endpoints.</p>
    </Card>
    <Card>
      <h2 class="text-lg font-semibold">Quick Actions</h2>
      <div class="mt-3 flex flex-col gap-2">
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/products">Open Product Management</RouterLink>
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/orders">Open Order Tracking</RouterLink>
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/reviews">Open Review Management</RouterLink>
        <RouterLink class="text-sm font-medium text-primary underline" to="/vendor/questions">Open Q&amp;A Management</RouterLink>
      </div>
    </Card>
  </section>
</template>
