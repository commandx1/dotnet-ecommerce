<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import Button from '@/components/ui/Button.vue'
import { useCartStore } from '@/stores/cart'
import { useCurrency } from '@/composables/useCurrency'

const cartStore = useCartStore()
const { format } = useCurrency()
</script>

<template>
  <section class="space-y-6">
    <h2 class="text-2xl font-semibold">Cart</h2>

    <Card v-if="cartStore.items.length === 0">Your cart is empty.</Card>

    <Card v-for="item in cartStore.items" :key="item.product.id" class="flex items-center justify-between">
      <div>
        <p class="font-medium">{{ item.product.name }}</p>
        <p class="text-sm text-muted-foreground">Quantity: {{ item.quantity }}</p>
      </div>
      <div class="flex items-center gap-3">
        <p class="font-semibold">{{ format(item.product.price * item.quantity) }}</p>
        <Button variant="outline" size="sm" @click="cartStore.removeFromCart(item.product.id)">Remove</Button>
      </div>
    </Card>

    <div class="flex items-center justify-between rounded-md border border-border bg-white p-4">
      <p class="text-sm text-muted-foreground">Total</p>
      <p class="text-lg font-bold">{{ format(cartStore.total) }}</p>
    </div>

    <RouterLink to="/checkout"><Button :class="'w-full'">Checkout</Button></RouterLink>
  </section>
</template>
