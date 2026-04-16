import { apiClient } from './client'
import type { BuyerOrder, CheckoutResult, VendorOrder } from '@/types/order'

interface CheckoutItemPayload {
  productId: string
  quantity: number
}

export async function checkout(items: CheckoutItemPayload[]) {
  const { data } = await apiClient.post<CheckoutResult>('/orders/checkout', { items })
  return data
}

export async function getMyOrders() {
  const { data } = await apiClient.get<BuyerOrder[]>('/orders/mine')
  return data
}

export async function getVendorOrders() {
  const { data } = await apiClient.get<VendorOrder[]>('/vendor/orders')
  return data
}
