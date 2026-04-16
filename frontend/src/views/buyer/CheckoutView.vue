<script setup lang="ts">
import { computed, reactive, shallowRef } from 'vue'
import { checkout } from '@/api/ordersApi'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { useCartStore } from '@/stores/cart'
import { useCurrency } from '@/composables/useCurrency'
import { getApiErrorMessage } from '@/lib/api-error'

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

function isValidCardNumber(value: string) {
  const digits = value.replace(/\D/g, '')
  if (digits.length < 13 || digits.length > 19) {
    return false
  }

  let sum = 0
  let shouldDouble = false

  for (let index = digits.length - 1; index >= 0; index -= 1) {
    let digit = Number(digits[index])
    if (shouldDouble) {
      digit *= 2
      if (digit > 9) {
        digit -= 9
      }
    }

    sum += digit
    shouldDouble = !shouldDouble
  }

  return sum % 10 === 0
}

function isValidExpiry(value: string) {
  const match = value.trim().match(/^(\d{2})\/(\d{2})$/)
  if (!match) {
    return false
  }

  const month = Number(match[1])
  const year = 2000 + Number(match[2])
  if (month < 1 || month > 12) {
    return false
  }

  const now = new Date()
  const currentMonth = now.getMonth() + 1
  const currentYear = now.getFullYear()

  if (year < currentYear) {
    return false
  }

  if (year === currentYear && month < currentMonth) {
    return false
  }

  return true
}

function handleCardNumberInput(value: string) {
  const digits = value.replace(/\D/g, '').slice(0, 19)
  form.cardNumber = digits.replace(/(\d{4})(?=\d)/g, '$1 ').trim()
}

function handleExpiryInput(value: string) {
  const digits = value.replace(/\D/g, '').slice(0, 4)
  if (digits.length < 3) {
    form.expiry = digits
    return
  }

  form.expiry = `${digits.slice(0, 2)}/${digits.slice(2)}`
}

function handleCvcInput(value: string) {
  form.cvc = value.replace(/\D/g, '').slice(0, 4)
}

const fieldErrors = computed(() => ({
  cardHolder:
    form.cardHolder.trim().length >= 3 ? null : 'Cardholder name must be at least 3 characters long.',
  cardNumber: isValidCardNumber(form.cardNumber) ? null : 'Enter a valid card number.',
  expiry: isValidExpiry(form.expiry) ? null : 'Expiry date is invalid or already in the past.',
  cvc: /^\d{3,4}$/.test(form.cvc.trim()) ? null : 'CVC must be 3 or 4 digits.'
}))

const isFormValid = computed(() =>
  Object.values(fieldErrors.value).every((fieldError) => fieldError === null)
)

async function submit() {
  if (!isFormValid.value) {
    error.value = 'Please fix the errors in the payment form.'
    return
  }

  if (cartStore.items.length === 0) {
    error.value = 'Checkout could not be completed because your cart is empty.'
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
  } catch (requestError) {
    error.value = getApiErrorMessage(requestError, 'Checkout failed.')
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <Card class="mx-auto max-w-xl space-y-4">
    <h2 class="text-2xl font-semibold">Checkout (Mock)</h2>
    <p class="text-sm text-muted-foreground">There is no live payment integration yet. This screen is a validated mock checkout form.</p>

    <div class="space-y-3">
      <Input v-model="form.cardHolder" placeholder="Name on Card" />
      <p v-if="fieldErrors.cardHolder" class="text-xs text-rose-600">{{ fieldErrors.cardHolder }}</p>
      <Input
        :model-value="form.cardNumber"
        placeholder="Card Number (16 digits)"
        @update:model-value="handleCardNumberInput"
      />
      <p v-if="fieldErrors.cardNumber" class="text-xs text-rose-600">{{ fieldErrors.cardNumber }}</p>
      <div class="grid grid-cols-2 gap-3">
        <div class="space-y-1">
          <Input :model-value="form.expiry" placeholder="AA/YY" @update:model-value="handleExpiryInput" />
          <p v-if="fieldErrors.expiry" class="text-xs text-rose-600">{{ fieldErrors.expiry }}</p>
        </div>
        <div class="space-y-1">
          <Input :model-value="form.cvc" placeholder="CVC" @update:model-value="handleCvcInput" />
          <p v-if="fieldErrors.cvc" class="text-xs text-rose-600">{{ fieldErrors.cvc }}</p>
        </div>
      </div>
    </div>

    <div class="rounded border border-border bg-muted p-3 text-sm">
      <div class="flex items-center justify-between">
        <span class="text-muted-foreground">Order Total</span>
        <span class="font-semibold">{{ format(cartStore.total) }}</span>
      </div>
    </div>

    <Button :class="'w-full'" :disabled="submitting || !isFormValid" @click="submit">
      {{ submitting ? 'Submitting...' : 'Place Order' }}
    </Button>

    <p v-if="submitted" class="text-sm text-emerald-600">
      Order created successfully. Order ID: <span class="font-mono">{{ orderId }}</span>
    </p>
    <p v-if="error" class="text-sm text-rose-600">{{ error }}</p>
  </Card>
</template>
