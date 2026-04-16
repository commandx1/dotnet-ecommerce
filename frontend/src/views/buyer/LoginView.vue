<script setup lang="ts">
import { computed, reactive, shallowRef } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()

const form = reactive({
  email: 'vendor@local.dev',
  password: 'Passw0rd!',
  rememberMe: false
})

const error = shallowRef<string | null>(null)
const sessionNotice = computed(() =>
  route.query.reason === 'session-expired'
    ? 'Oturumunuz sonlandı. Lütfen tekrar giriş yapın.'
    : null
)

async function submit() {
  error.value = null
  try {
    await authStore.login(form.email, form.password, form.rememberMe)
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : authStore.landingPath
    await router.push(redirect)
  } catch {
    error.value = 'Giriş başarısız.'
  }
}
</script>

<template>
  <Card class="mx-auto max-w-md space-y-4">
    <h2 class="text-2xl font-semibold">Giriş</h2>
    <p v-if="sessionNotice" class="rounded-md border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-700">
      {{ sessionNotice }}
    </p>
    <Input v-model="form.email" placeholder="E-posta" />
    <Input v-model="form.password" type="password" placeholder="Şifre" />
    <label class="flex items-center gap-2 text-sm text-slate-700">
      <input v-model="form.rememberMe" type="checkbox" class="h-4 w-4 rounded border-border" />
      Beni hatırla
    </label>
    <Button :class="'w-full'" @click="submit">Giriş Yap</Button>
    <p v-if="error" class="text-sm text-rose-600">{{ error }}</p>
  </Card>
</template>
