import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import type { Product } from '@/types/product'
import * as catalogApi from '@/api/catalogApi'

export const useCatalogStore = defineStore('catalog', () => {
  const products = ref<Product[]>([])
  const loading = ref(false)

  const productCount = computed(() => products.value.length)

  async function fetchProducts() {
    loading.value = true
    try {
      products.value = await catalogApi.getProducts()
    } finally {
      loading.value = false
    }
  }

  return {
    products,
    loading,
    productCount,
    fetchProducts
  }
})
