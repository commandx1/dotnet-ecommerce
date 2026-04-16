import { createRouter, createWebHistory } from 'vue-router'
import BuyerLayout from '@/layouts/BuyerLayout.vue'
import VendorLayout from '@/layouts/VendorLayout.vue'
import HomeView from '@/views/buyer/HomeView.vue'
import ProductsView from '@/views/buyer/ProductsView.vue'
import ProductDetailView from '@/views/buyer/ProductDetailView.vue'
import CartView from '@/views/buyer/CartView.vue'
import CheckoutView from '@/views/buyer/CheckoutView.vue'
import LoginView from '@/views/buyer/LoginView.vue'
import BuyerOrdersView from '@/views/buyer/BuyerOrdersView.vue'
import VendorDashboardView from '@/views/vendor/VendorDashboardView.vue'
import VendorOrdersView from '@/views/vendor/VendorOrdersView.vue'
import VendorProductsView from '@/views/vendor/VendorProductsView.vue'
import VendorQuestionsView from '@/views/vendor/VendorQuestionsView.vue'
import VendorReviewsView from '@/views/vendor/VendorReviewsView.vue'
import { validateSession } from '@/api/authApi'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: BuyerLayout,
      children: [
        { path: '', name: 'home', component: HomeView },
        { path: 'products', name: 'products', component: ProductsView },
        { path: 'products/:id', name: 'product-detail', component: ProductDetailView },
        { path: 'cart', name: 'cart', component: CartView },
        { path: 'checkout', name: 'checkout', component: CheckoutView, meta: { requiresAuth: true, roles: ['Buyer'] } },
        { path: 'orders', name: 'buyer-orders', component: BuyerOrdersView, meta: { requiresAuth: true, roles: ['Buyer'] } },
        { path: 'login', name: 'login', component: LoginView, meta: { guestOnly: true } }
      ]
    },
    {
      path: '/vendor',
      component: VendorLayout,
      meta: { requiresAuth: true, roles: ['Vendor'] },
      children: [
        { path: '', name: 'vendor-dashboard', component: VendorDashboardView },
        { path: 'products', name: 'vendor-products', component: VendorProductsView },
        { path: 'orders', name: 'vendor-orders', component: VendorOrdersView },
        { path: 'reviews', name: 'vendor-reviews', component: VendorReviewsView },
        { path: 'questions', name: 'vendor-questions', component: VendorQuestionsView }
      ]
    }
  ]
})

router.beforeEach(async (to) => {
  const authStore = useAuthStore()

  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth)
  if (requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (requiresAuth && authStore.isAuthenticated) {
    try {
      await validateSession()
    } catch {
      authStore.clearSession()
      return {
        name: 'login',
        query: {
          redirect: to.fullPath,
          reason: 'session-expired'
        }
      }
    }
  }

  const isVendorRoute = to.path.startsWith('/vendor')
  if (authStore.isAuthenticated && authStore.role === 'Vendor' && !isVendorRoute && to.name !== 'login') {
    return '/vendor'
  }

  const guestOnly = to.matched.some((record) => record.meta.guestOnly)
  if (guestOnly && authStore.isAuthenticated) {
    return authStore.landingPath
  }

  const allowedRoles = to.matched
    .map((record) => record.meta.roles as string[] | undefined)
    .find((roles) => Array.isArray(roles) && roles.length > 0)

  if (allowedRoles && (!authStore.role || !allowedRoles.includes(authStore.role))) {
    if (!authStore.isAuthenticated) {
      return { name: 'login', query: { redirect: to.fullPath } }
    }

    return authStore.landingPath
  }

  return true
})

export default router
