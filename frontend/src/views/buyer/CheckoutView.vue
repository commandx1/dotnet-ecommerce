<script setup lang="ts">
import { reactive, shallowRef } from 'vue'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'

const form = reactive({
  cardHolder: '',
  cardNumber: '',
  expiry: '',
  cvc: ''
})

const submitted = shallowRef(false)

function submit() {
  submitted.value = true
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

    <Button :class="'w-full'" @click="submit">Siparişi Onayla</Button>

    <p v-if="submitted" class="text-sm text-emerald-600">Demo sipariş başarıyla alındı.</p>
  </Card>
</template>
