<script setup lang="ts">
import { reactive, shallowRef } from 'vue'
import { checkout } from '@/api/ordersApi'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { useCartStore } from '@/stores/cart'
import { useCurrency } from '@/composables/useCurrency'

const cartStore = useCartStore()
const { format } = useCurrency()

const form = reactive({
  cardHolder: '',
  cardNumber: '',
  expiry: '',
  cvc: ''
})

const submitted = shallowRef(false)
const submitting = shallowRef(false)
const orderId = shallowRef<string | null>(null)
const error = shallowRef<string | null>(null)

async function submit() {
  if (cartStore.items.length === 0) {
    error.value = 'Sepet boş olduğu için checkout yapılamadı.'
    return
  }

  error.value = null
  submitting.value = true
  try {
    const result = await checkout(
      cartStore.items.map((item) => ({
        productId: item.product.id,
        quantity: item.quantity
      }))
    )

    orderId.value = result.orderId
    submitted.value = true
    cartStore.clear()
  } catch {
    error.value = 'Checkout başarısız. Buyer hesabı ile tekrar deneyin.'
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <Card class="mx-auto max-w-xl space-y-4">
    <h2 class="text-2xl font-semibold">Checkout (Dummy)</h2>
    <p class="text-sm text-muted-foreground">Gerçek ödeme entegrasyonu yoktur. Bu ekran yalnızca validasyonlu mock formdur.</p>

    <div class="space-y-3">
      <Input v-model="form.cardHolder" placeholder="Kart Üzerindeki İsim" />
      <Input v-model="form.cardNumber" placeholder="Kart Numarası" />
      <div class="grid grid-cols-2 gap-3">
        <Input v-model="form.expiry" placeholder="AA/YY" />
        <Input v-model="form.cvc" placeholder="CVC" />
      </div>
    </div>

    <div class="rounded border border-border bg-muted p-3 text-sm">
      <div class="flex items-center justify-between">
        <span class="text-muted-foreground">Sipariş Toplamı</span>
        <span class="font-semibold">{{ format(cartStore.total) }}</span>
      </div>
    </div>

    <Button :class="'w-full'" :disabled="submitting" @click="submit">
      {{ submitting ? 'Gönderiliyor...' : 'Siparişi Onayla' }}
    </Button>

    <p v-if="submitted" class="text-sm text-emerald-600">
      Sipariş oluşturuldu. Order ID: <span class="font-mono">{{ orderId }}</span>
    </p>
    <p v-if="error" class="text-sm text-rose-600">{{ error }}</p>
  </Card>
</template>
