<script setup lang="ts">
import { computed, reactive, shallowRef } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { useAuthStore } from '@/stores/auth'
import { getApiErrorMessage } from '@/lib/api-error'

const authStore = useAuthStore()
const router = useRouter()
const route = useRoute()

const mode = shallowRef<'login' | 'register'>('login')

const loginForm = reactive({
  email: '',
  password: '',
  rememberMe: false
})

const registerForm = reactive({
  email: '',
  password: '',
  role: 'Buyer' as 'Buyer' | 'Vendor'
})

const error = shallowRef<string | null>(null)
const sessionNotice = computed(() =>
  route.query.reason === 'session-expired'
    ? 'Your session has expired. Please sign in again.'
    : null
)

async function submit() {
  error.value = null
  try {
    if (mode.value === 'login') {
      await authStore.login(loginForm.email, loginForm.password, loginForm.rememberMe)
    } else {
      await authStore.register(registerForm.email, registerForm.password, registerForm.role)
    }

    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : authStore.landingPath
    await router.push(redirect)
  } catch (requestError) {
    error.value = getApiErrorMessage(requestError, 'Authentication failed.')
  }
}
</script>

<template>
  <Card class="mx-auto max-w-md space-y-4">
    <div class="flex items-center justify-between gap-2">
      <h2 class="text-2xl font-semibold">{{ mode === 'login' ? 'Sign In' : 'Create Account' }}</h2>
      <div class="inline-flex rounded-lg border border-border bg-muted/70 p-1 text-sm">
        <button
          class="rounded-md px-3 py-1.5"
          :class="mode === 'login' ? 'bg-white shadow-sm' : 'text-muted-foreground'"
          @click="mode = 'login'"
        >
          Sign In
        </button>
        <button
          class="rounded-md px-3 py-1.5"
          :class="mode === 'register' ? 'bg-white shadow-sm' : 'text-muted-foreground'"
          @click="mode = 'register'"
        >
          Register
        </button>
      </div>
    </div>

    <p v-if="sessionNotice" class="rounded-md border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-700">
      {{ sessionNotice }}
    </p>

    <template v-if="mode === 'login'">
      <Input v-model="loginForm.email" placeholder="Email" />
      <Input v-model="loginForm.password" type="password" placeholder="Password" />
      <label class="flex items-center gap-2 text-sm text-slate-700">
        <input v-model="loginForm.rememberMe" type="checkbox" class="h-4 w-4 rounded border-border" />
        Remember me
      </label>
    </template>

    <template v-else>
      <Input v-model="registerForm.email" placeholder="Email" />
      <Input v-model="registerForm.password" type="password" placeholder="Password" />
      <label class="space-y-2 text-sm">
        <span class="text-muted-foreground">Role</span>
        <select
          v-model="registerForm.role"
          class="h-10 w-full rounded-md border border-border bg-white px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary"
        >
          <option value="Buyer">Buyer</option>
          <option value="Vendor">Vendor</option>
        </select>
      </label>
    </template>

    <Button :class="'w-full'" @click="submit">{{ mode === 'login' ? 'Sign In' : 'Create Account' }}</Button>
    <p v-if="error" class="text-sm text-rose-600">{{ error }}</p>
  </Card>
</template>
