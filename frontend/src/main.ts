import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { AUTH_SESSION_EXPIRED_EVENT } from '@/lib/session'

import App from './App.vue'
import router from './router'
import './assets/main.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)

if (typeof window !== 'undefined') {
  window.addEventListener(AUTH_SESSION_EXPIRED_EVENT, () => {
    const currentRoute = router.currentRoute.value
    if (currentRoute.name === 'login') {
      return
    }

    void router.push({
      name: 'login',
      query: {
        redirect: currentRoute.fullPath,
        reason: 'session-expired'
      }
    })
  })
}

app.mount('#app')
