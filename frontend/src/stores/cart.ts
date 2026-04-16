import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import type { Product } from '@/types/product'

interface CartItem {
  product: Product
  quantity: number
}

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>([])

  const total = computed(() =>
    items.value.reduce((accumulator, item) => accumulator + item.product.price * item.quantity, 0)
  )

  function addToCart(product: Product) {
    const existing = items.value.find((item) => item.product.id === product.id)
    if (existing) {
      existing.quantity += 1
      return
    }

    items.value.push({ product, quantity: 1 })
  }

  function removeFromCart(productId: string) {
    items.value = items.value.filter((item) => item.product.id !== productId)
  }

  function clear() {
    items.value = []
  }

  return {
    items,
    total,
    addToCart,
    removeFromCart,
    clear
  }
})
