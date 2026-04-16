import { apiClient } from './client'
import type { Review } from '@/types/review'

interface CreateReviewPayload {
  rating: number
  comment: string
}

export async function getProductReviews(productId: string) {
  const { data } = await apiClient.get<Review[]>(`/products/${productId}/reviews`)
  return data
}

export async function createProductReview(productId: string, payload: CreateReviewPayload) {
  const { data } = await apiClient.post<Review>(`/products/${productId}/reviews`, payload)
  return data
}

export async function getVendorReviews() {
  const { data } = await apiClient.get<Review[]>('/vendor/reviews')
  return data
}

export async function replyToReview(reviewId: string, replyText: string) {
  const { data } = await apiClient.post<Review>(`/vendor/reviews/${reviewId}/reply`, { replyText })
  return data
}
