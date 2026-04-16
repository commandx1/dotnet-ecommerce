export interface Question {
  id: string
  productId: string
  buyerId: string
  questionText: string
  answerText?: string | null
  createdAt: string
  answeredAt?: string | null
}
