<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getProductById } from '@/api/catalogApi'
import type { Product } from '@/types/product'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'
import { useCartStore } from '@/stores/cart'
import { useCurrency } from '@/composables/useCurrency'

const route = useRoute()
const cartStore = useCartStore()
const { format } = useCurrency()

const loading = ref(false)
const product = ref<Product | null>(null)

const productId = computed(() => String(route.params.id || ''))

onMounted(async () => {
  loading.value = true
  try {
    product.value = await getProductById(productId.value)
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <Card v-if="loading">Yükleniyor...</Card>
  <Card v-else-if="product" class="space-y-6">
    <div class="space-y-2">
      <h2 class="text-2xl font-semibold">{{ product.name }}</h2>
      <p class="text-muted-foreground">{{ product.description }}</p>
    </div>
    <div class="flex items-center justify-between">
      <span class="text-xl font-bold">{{ format(product.price) }}</span>
      <Button @click="cartStore.addToCart(product)">Sepete Ekle</Button>
    </div>
  </Card>
  <Card v-else>Ürün bulunamadı.</Card>
</template>
