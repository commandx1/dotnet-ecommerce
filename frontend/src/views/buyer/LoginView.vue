<script setup lang="ts">
import { reactive, shallowRef } from 'vue'
import { useRouter } from 'vue-router'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()

const form = reactive({
  email: 'vendor@local.dev',
  password: 'Passw0rd!'
})

const error = shallowRef<string | null>(null)

async function submit() {
  error.value = null
  try {
    await authStore.login(form.email, form.password)
    await router.push('/vendor')
  } catch {
    error.value = 'Giriş başarısız.'
  }
}
</script>

<template>
  <Card class="mx-auto max-w-md space-y-4">
    <h2 class="text-2xl font-semibold">Giriş</h2>
    <Input v-model="form.email" placeholder="E-posta" />
    <Input v-model="form.password" type="password" placeholder="Şifre" />
    <Button :class="'w-full'" @click="submit">Giriş Yap</Button>
    <p v-if="error" class="text-sm text-rose-600">{{ error }}</p>
  </Card>
</template>
