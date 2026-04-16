<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { useVendorProductsStore } from '@/stores/vendorProducts'
import { useCurrency } from '@/composables/useCurrency'
import { getApiErrorMessage } from '@/lib/api-error'

const vendorStore = useVendorProductsStore()
const { format } = useCurrency()
const formError = ref<string | null>(null)

const form = reactive({
  name: '',
  description: '',
  price: '0',
  stock: '0'
})

onMounted(async () => {
  await vendorStore.fetchOwnProducts()
})

async function submit() {
  formError.value = null
  try {
    await vendorStore.createProduct({
      name: form.name,
      description: form.description,
      price: Number(form.price),
      stock: Number(form.stock)
    })

    form.name = ''
    form.description = ''
    form.price = '0'
    form.stock = '0'
  } catch (requestError) {
    formError.value = getApiErrorMessage(requestError, 'Unable to save the product.')
  }
}
</script>

<template>
  <section class="space-y-6">
    <Card class="space-y-3">
      <h2 class="text-lg font-semibold">New Product</h2>
      <Input v-model="form.name" placeholder="Product name" />
      <Input v-model="form.description" placeholder="Description" />
      <div class="grid grid-cols-2 gap-2">
        <Input v-model="form.price" type="number" placeholder="Price" />
        <Input v-model="form.stock" type="number" placeholder="Stock" />
      </div>
      <Button @click="submit">Save</Button>
      <p v-if="formError" class="text-sm text-rose-600">{{ formError }}</p>
    </Card>

    <Card>
      <h2 class="mb-4 text-lg font-semibold">My Products</h2>
      <div v-if="vendorStore.loading" class="text-sm text-muted-foreground">Loading...</div>
      <ul v-else class="space-y-3">
        <li v-for="product in vendorStore.products" :key="product.id" class="flex items-center justify-between">
          <div>
            <p class="font-medium">{{ product.name }}</p>
            <p class="text-sm text-muted-foreground">Stock: {{ product.stock }}</p>
          </div>
          <p class="font-semibold">{{ format(product.price) }}</p>
        </li>
      </ul>
    </Card>
  </section>
</template>
