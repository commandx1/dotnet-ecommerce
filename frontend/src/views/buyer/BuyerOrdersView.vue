<script setup lang="ts">
import { onMounted, ref } from 'vue'
import Card from '@/components/ui/Card.vue'
import Button from '@/components/ui/Button.vue'
import { getMyOrders } from '@/api/ordersApi'
import type { BuyerOrder } from '@/types/order'
import { useCurrency } from '@/composables/useCurrency'

const loading = ref(false)
const error = ref<string | null>(null)
const orders = ref<BuyerOrder[]>([])

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

async function loadOrders() {
  loading.value = true
  error.value = null

  try {
    orders.value = await getMyOrders()
  } catch {
    error.value = 'Sipariş geçmişi yüklenemedi. Buyer hesabı ile giriş yapın.'
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await loadOrders()
})
</script>

<template>
  <section class="space-y-4">
    <div class="flex items-center justify-between">
      <h2 class="text-2xl font-semibold">Sipariş Geçmişim</h2>
      <Button variant="outline" size="sm" @click="loadOrders">Yenile</Button>
    </div>

    <Card v-if="loading">Yükleniyor...</Card>
    <Card v-else-if="error">{{ error }}</Card>
    <Card v-else-if="orders.length === 0">Henüz siparişiniz yok.</Card>

    <Card v-for="order in orders" :key="order.id" class="space-y-4">
      <div class="flex flex-wrap items-center justify-between gap-3">
        <div class="space-y-1">
          <p class="text-sm text-muted-foreground">Order ID</p>
          <p class="font-mono text-sm">{{ order.id }}</p>
        </div>
        <div class="space-y-1 text-right">
          <p class="text-sm text-muted-foreground">Durum</p>
          <p class="font-semibold">{{ statusLabel(order.status) }}</p>
        </div>
      </div>

      <ul class="space-y-2 border-t border-border pt-3 text-sm">
        <li v-for="(item, index) in order.items" :key="`${order.id}-${item.productId}-${index}`" class="flex justify-between">
          <span>Product: {{ item.productId }} (x{{ item.quantity }})</span>
          <span>{{ format(item.unitPrice * item.quantity) }}</span>
        </li>
      </ul>

      <div class="flex items-center justify-between border-t border-border pt-3 text-sm">
        <span class="text-muted-foreground">Toplam</span>
        <span class="font-semibold">{{ format(order.totalAmount) }}</span>
      </div>
    </Card>
  </section>
</template>
