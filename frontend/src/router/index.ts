import { createRouter, createWebHistory } from 'vue-router'
import BuyerLayout from '@/layouts/BuyerLayout.vue'
import VendorLayout from '@/layouts/VendorLayout.vue'
import HomeView from '@/views/buyer/HomeView.vue'
import ProductsView from '@/views/buyer/ProductsView.vue'
import ProductDetailView from '@/views/buyer/ProductDetailView.vue'
import CartView from '@/views/buyer/CartView.vue'
import CheckoutView from '@/views/buyer/CheckoutView.vue'
import LoginView from '@/views/buyer/LoginView.vue'
import VendorDashboardView from '@/views/vendor/VendorDashboardView.vue'
import VendorProductsView from '@/views/vendor/VendorProductsView.vue'

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
        { path: 'checkout', name: 'checkout', component: CheckoutView },
        { path: 'login', name: 'login', component: LoginView }
      ]
    },
    {
      path: '/vendor',
      component: VendorLayout,
      children: [
        { path: '', name: 'vendor-dashboard', component: VendorDashboardView },
        { path: 'products', name: 'vendor-products', component: VendorProductsView }
      ]
    }
  ]
})

export default router
