<script setup lang="ts">
import { onMounted, ref } from 'vue'
import Card from '@/components/ui/Card.vue'
import { getVendorOrders } from '@/api/ordersApi'
import type { VendorOrder } from '@/types/order'
import { useCurrency } from '@/composables/useCurrency'
import { getApiErrorMessage } from '@/lib/api-error'

const loading = ref(false)
const error = ref<string | null>(null)
const orders = ref<VendorOrder[]>([])

const { format } = useCurrency()

function statusLabel(status: number) {
  const labels: Record<number, string> = {
    1: 'Pending',
    2: 'Paid',
    3: 'Shipped',
    4: 'Completed',
    5: 'Cancelled'
  }

  return labels[status] ?? `Unknown (${status})`
}

onMounted(async () => {
  loading.value = true
  error.value = null
  try {
    orders.value = await getVendorOrders()
  } catch (requestError) {
    error.value = getApiErrorMessage(requestError, 'Unable to load vendor orders.')
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <section class="space-y-4">
    <h2 class="text-2xl font-semibold">Vendor Order Tracking</h2>

    <Card v-if="loading">Loading...</Card>
    <Card v-else-if="error">{{ error }}</Card>
    <Card v-else-if="orders.length === 0">No orders yet.</Card>

    <Card v-for="order in orders" :key="order.id" class="space-y-4">
      <div class="flex flex-wrap items-center justify-between gap-3">
        <div class="space-y-1">
          <p class="text-sm text-muted-foreground">Order ID</p>
          <p class="font-mono text-sm">{{ order.id }}</p>
        </div>
        <div class="space-y-1 text-right">
          <p class="text-sm text-muted-foreground">Status</p>
          <p class="font-semibold">{{ statusLabel(order.status) }}</p>
        </div>
      </div>

      <div class="space-y-2 border-t border-border pt-3">
        <p class="text-sm text-muted-foreground">Vendor items</p>
        <ul class="space-y-2 text-sm">
          <li v-for="(item, index) in order.items" :key="`${order.id}-${item.productId}-${index}`" class="flex justify-between">
            <span>Product: {{ item.productId }} (x{{ item.quantity }})</span>
            <span>{{ format(item.unitPrice * item.quantity) }}</span>
          </li>
        </ul>
      </div>

      <div class="flex items-center justify-between border-t border-border pt-3 text-sm">
        <span class="text-muted-foreground">Vendor total</span>
        <span class="font-semibold">{{ format(order.vendorTotal) }}</span>
      </div>
    </Card>
  </section>
</template>
