export interface Product {
  id: string
  name: string
  description: string
  price: number
  stock: number
  imageUrl?: string | null
  vendorId: string
  createdAt: string
  updatedAt: string
}
