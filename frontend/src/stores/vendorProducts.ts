import { defineStore } from 'pinia'
import { ref } from 'vue'
import * as vendorApi from '@/api/vendorProductsApi'
import type { Product } from '@/types/product'

export const useVendorProductsStore = defineStore('vendor-products', () => {
  const products = ref<Product[]>([])
  const loading = ref(false)

  async function fetchOwnProducts() {
    loading.value = true
    try {
      products.value = await vendorApi.getVendorProducts()
    } finally {
      loading.value = false
    }
  }

  async function createProduct(payload: vendorApi.UpsertVendorProductPayload) {
    const product = await vendorApi.createVendorProduct(payload)
    products.value.unshift(product)
    return product
  }

  return {
    products,
    loading,
    fetchOwnProducts,
    createProduct
  }
})
