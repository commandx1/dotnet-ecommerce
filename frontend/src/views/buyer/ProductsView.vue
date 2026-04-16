<script setup lang="ts">
import { onMounted } from 'vue'
import { useCatalogStore } from '@/stores/catalog'
import { useCartStore } from '@/stores/cart'
import Card from '@/components/ui/Card.vue'
import Button from '@/components/ui/Button.vue'
import { useCurrency } from '@/composables/useCurrency'

const catalogStore = useCatalogStore()
const cartStore = useCartStore()
const { format } = useCurrency()

onMounted(async () => {
  await catalogStore.fetchProducts()
})
</script>

<template>
  <section class="space-y-6">
    <header class="flex items-center justify-between">
      <h2 class="text-2xl font-semibold">Products ({{ catalogStore.productCount }})</h2>
      <p class="text-sm text-muted-foreground">Canlı veri API üzerinden gelir.</p>
    </header>

    <div v-if="catalogStore.loading" class="text-sm text-muted-foreground">Yükleniyor...</div>

    <div v-else class="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
      <Card v-for="product in catalogStore.products" :key="product.id" class="space-y-4">
        <div class="space-y-2">
          <h3 class="text-lg font-semibold">{{ product.name }}</h3>
          <p class="line-clamp-2 text-sm text-muted-foreground">{{ product.description }}</p>
        </div>
        <div class="flex items-center justify-between">
          <span class="text-sm font-semibold">{{ format(product.price) }}</span>
          <div class="flex gap-2">
            <RouterLink :to="`/products/${product.id}`">
              <Button variant="outline" size="sm">Detay</Button>
            </RouterLink>
            <Button size="sm" @click="cartStore.addToCart(product)">Sepete Ekle</Button>
          </div>
        </div>
      </Card>
    </div>
  </section>
</template>
