export interface Review {
  id: string
  productId: string
  buyerId: string
  rating: number
  comment: string
  replyText?: string | null
  createdAt: string
}
