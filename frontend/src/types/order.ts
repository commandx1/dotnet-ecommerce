export interface OrderItem {
  productId: string
  quantity: number
  unitPrice: number
}

export interface BuyerOrder {
  id: string
  status: number
  totalAmount: number
  createdAt: string
  items: OrderItem[]
}

export interface VendorOrder {
  id: string
  buyerId: string
  status: number
  createdAt: string
  vendorTotal: number
  items: OrderItem[]
}

export interface CheckoutResult {
  orderId: string
  totalAmount: number
  status: number
}
