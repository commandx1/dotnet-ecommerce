import { apiClient } from './client'
import type { Product } from '@/types/product'

export interface UpsertVendorProductPayload {
  name: string
  description: string
  price: number
  stock: number
  image?: File | null
}

function toFormData(payload: UpsertVendorProductPayload) {
  const formData = new FormData()
  formData.append('name', payload.name)
  formData.append('description', payload.description)
  formData.append('price', String(payload.price))
  formData.append('stock', String(payload.stock))
  if (payload.image) {
    formData.append('image', payload.image)
  }

  return formData
}

export async function getVendorProducts() {
  const { data } = await apiClient.get<Product[]>('/vendor/products')
  return data
}

export async function createVendorProduct(payload: UpsertVendorProductPayload) {
  const { data } = await apiClient.post<Product>('/vendor/products', toFormData(payload), {
    headers: { 'Content-Type': 'multipart/form-data' }
  })

  return data
}
