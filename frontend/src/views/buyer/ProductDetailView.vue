<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { getProductById } from '@/api/catalogApi'
import { createProductQuestion, getProductQuestions } from '@/api/questionsApi'
import { createProductReview, getProductReviews } from '@/api/reviewsApi'
import type { Product } from '@/types/product'
import type { Question } from '@/types/question'
import type { Review } from '@/types/review'
import Button from '@/components/ui/Button.vue'
import Card from '@/components/ui/Card.vue'
import { useCartStore } from '@/stores/cart'
import { useCurrency } from '@/composables/useCurrency'
import { getApiErrorMessage } from '@/lib/api-error'

const route = useRoute()
const cartStore = useCartStore()
const { format } = useCurrency()

const loading = ref(false)
const product = ref<Product | null>(null)
const reviews = ref<Review[]>([])
const questions = ref<Question[]>([])

const reviewRating = ref('5')
const reviewComment = ref('')
const questionText = ref('')

const reviewSubmitting = ref(false)
const questionSubmitting = ref(false)
const feedback = ref<string | null>(null)
const actionError = ref<string | null>(null)

const productId = computed(() => String(route.params.id || ''))

async function loadProductData() {
  loading.value = true
  actionError.value = null

  try {
    const [productData, reviewData, questionData] = await Promise.all([
      getProductById(productId.value),
      getProductReviews(productId.value),
      getProductQuestions(productId.value)
    ])

    product.value = productData
    reviews.value = reviewData
    questions.value = questionData
  } catch (requestError) {
    actionError.value = getApiErrorMessage(requestError, 'Ürün detayı yüklenemedi.')
  } finally {
    loading.value = false
  }
}

async function submitReview() {
  if (!reviewComment.value.trim()) {
    actionError.value = 'Yorum metni boş olamaz.'
    return
  }

  actionError.value = null
  feedback.value = null
  reviewSubmitting.value = true

  try {
    await createProductReview(productId.value, {
      rating: Number(reviewRating.value),
      comment: reviewComment.value
    })

    reviews.value = await getProductReviews(productId.value)
    reviewComment.value = ''
    reviewRating.value = '5'
    feedback.value = 'Yorum eklendi.'
  } catch (requestError) {
    actionError.value = getApiErrorMessage(requestError, 'Yorum eklenemedi.')
  } finally {
    reviewSubmitting.value = false
  }
}

async function submitQuestion() {
  if (!questionText.value.trim()) {
    actionError.value = 'Soru metni boş olamaz.'
    return
  }

  actionError.value = null
  feedback.value = null
  questionSubmitting.value = true

  try {
    await createProductQuestion(productId.value, {
      questionText: questionText.value
    })

    questions.value = await getProductQuestions(productId.value)
    questionText.value = ''
    feedback.value = 'Soru gönderildi.'
  } catch (requestError) {
    actionError.value = getApiErrorMessage(requestError, 'Soru gönderilemedi.')
  } finally {
    questionSubmitting.value = false
  }
}

onMounted(async () => {
  await loadProductData()
})
</script>

<template>
  <Card v-if="loading">Yükleniyor...</Card>

  <section v-else-if="product" class="space-y-6">
    <Card class="space-y-6">
      <div class="space-y-2">
        <h2 class="text-2xl font-semibold">{{ product.name }}</h2>
        <p class="text-muted-foreground">{{ product.description }}</p>
      </div>
      <div class="flex items-center justify-between">
        <span class="text-xl font-bold">{{ format(product.price) }}</span>
        <Button @click="cartStore.addToCart(product)">Sepete Ekle</Button>
      </div>
    </Card>

    <Card v-if="actionError" class="text-sm text-rose-600">{{ actionError }}</Card>
    <Card v-if="feedback" class="text-sm text-emerald-600">{{ feedback }}</Card>

    <div class="grid gap-6 lg:grid-cols-2">
      <Card class="space-y-4">
        <h3 class="text-lg font-semibold">Yorum Yaz</h3>
        <div class="space-y-2">
          <label class="text-sm text-muted-foreground" for="review-rating">Puan (1-5)</label>
          <input
            id="review-rating"
            v-model="reviewRating"
            min="1"
            max="5"
            type="number"
            class="h-10 w-full rounded-md border border-border px-3"
          />
        </div>
        <div class="space-y-2">
          <label class="text-sm text-muted-foreground" for="review-comment">Yorum</label>
          <textarea
            id="review-comment"
            v-model="reviewComment"
            rows="4"
            class="w-full rounded-md border border-border px-3 py-2"
          />
        </div>
        <Button :disabled="reviewSubmitting" @click="submitReview">
          {{ reviewSubmitting ? 'Gönderiliyor...' : 'Yorum Gönder' }}
        </Button>
      </Card>

      <Card class="space-y-4">
        <h3 class="text-lg font-semibold">Soru Sor</h3>
        <div class="space-y-2">
          <label class="text-sm text-muted-foreground" for="question-text">Soru</label>
          <textarea
            id="question-text"
            v-model="questionText"
            rows="4"
            class="w-full rounded-md border border-border px-3 py-2"
          />
        </div>
        <Button :disabled="questionSubmitting" @click="submitQuestion">
          {{ questionSubmitting ? 'Gönderiliyor...' : 'Soru Gönder' }}
        </Button>
      </Card>
    </div>

    <div class="grid gap-6 lg:grid-cols-2">
      <Card class="space-y-4">
        <h3 class="text-lg font-semibold">Yorumlar</h3>
        <p v-if="reviews.length === 0" class="text-sm text-muted-foreground">Henüz yorum yok.</p>
        <ul v-else class="space-y-3">
          <li v-for="review in reviews" :key="review.id" class="rounded-md border border-border p-3">
            <p class="text-sm font-semibold">Puan: {{ review.rating }}/5</p>
            <p class="mt-1 text-sm">{{ review.comment }}</p>
            <p v-if="review.replyText" class="mt-2 rounded bg-muted p-2 text-sm">
              <span class="font-medium">Vendor cevabı:</span> {{ review.replyText }}
            </p>
          </li>
        </ul>
      </Card>

      <Card class="space-y-4">
        <h3 class="text-lg font-semibold">Sorular</h3>
        <p v-if="questions.length === 0" class="text-sm text-muted-foreground">Henüz soru yok.</p>
        <ul v-else class="space-y-3">
          <li v-for="question in questions" :key="question.id" class="rounded-md border border-border p-3">
            <p class="text-sm">{{ question.questionText }}</p>
            <p v-if="question.answerText" class="mt-2 rounded bg-muted p-2 text-sm">
              <span class="font-medium">Vendor cevabı:</span> {{ question.answerText }}
            </p>
            <p v-else class="mt-2 text-xs text-muted-foreground">Henüz cevaplanmadı.</p>
          </li>
        </ul>
      </Card>
    </div>
  </section>

  <Card v-else>Ürün bulunamadı.</Card>
</template>
