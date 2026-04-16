import { apiClient } from './client'
import type { Question } from '@/types/question'

interface CreateQuestionPayload {
  questionText: string
}

export async function getProductQuestions(productId: string) {
  const { data } = await apiClient.get<Question[]>(`/products/${productId}/questions`)
  return data
}

export async function createProductQuestion(productId: string, payload: CreateQuestionPayload) {
  const { data } = await apiClient.post<Question>(`/products/${productId}/questions`, payload)
  return data
}

export async function getVendorQuestions() {
  const { data } = await apiClient.get<Question[]>('/vendor/questions')
  return data
}

export async function answerQuestion(questionId: string, answerText: string) {
  const { data } = await apiClient.post<Question>(`/vendor/questions/${questionId}/answer`, { answerText })
  return data
}
