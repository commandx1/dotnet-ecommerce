import { apiClient } from './client'
import type { Product } from '@/types/product'

export async function getProducts() {
  const { data } = await apiClient.get<Product[]>('/catalog/products')
  return data
}

export async function getProductById(id: string) {
  const { data } = await apiClient.get<Product>(`/catalog/products/${id}`)
  return data
}
