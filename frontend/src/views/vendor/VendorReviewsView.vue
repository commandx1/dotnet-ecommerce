<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { getVendorReviews, replyToReview } from '@/api/reviewsApi'
import type { Review } from '@/types/review'
import { getApiErrorMessage } from '@/lib/api-error'

const loading = ref(false)
const error = ref<string | null>(null)
const reviews = ref<Review[]>([])
const drafts = reactive<Record<string, string>>({})

async function loadReviews() {
  loading.value = true
  error.value = null

  try {
    reviews.value = await getVendorReviews()
  } catch (requestError) {
    error.value = getApiErrorMessage(requestError, 'Vendor yorumları yüklenemedi.')
  } finally {
    loading.value = false
  }
}

async function sendReply(reviewId: string) {
  const replyText = drafts[reviewId]?.trim()
  if (!replyText) {
    return
  }

  try {
    const updated = await replyToReview(reviewId, replyText)
    const index = reviews.value.findIndex((review) => review.id === reviewId)
    if (index >= 0) {
      reviews.value[index] = updated
    }

    drafts[reviewId] = ''
  } catch (requestError) {
    error.value = getApiErrorMessage(requestError, 'Yorum cevabı gönderilemedi.')
  }
}

onMounted(async () => {
  await loadReviews()
})
</script>

<template>
  <section class="space-y-4">
    <h2 class="text-2xl font-semibold">Vendor Yorum Yönetimi</h2>

    <Card v-if="loading">Yükleniyor...</Card>
    <Card v-else-if="error">{{ error }}</Card>
    <Card v-else-if="reviews.length === 0">Henüz yorum yok.</Card>

    <Card v-for="review in reviews" :key="review.id" class="space-y-4">
      <div class="space-y-1">
        <p class="font-semibold">Puan: {{ review.rating }}/5</p>
        <p class="text-sm">{{ review.comment }}</p>
      </div>

      <div v-if="review.replyText" class="rounded bg-muted p-3 text-sm">
        <span class="font-medium">Mevcut cevap:</span> {{ review.replyText }}
      </div>

      <div class="space-y-2">
        <Input v-model="drafts[review.id]" placeholder="Yoruma cevap yazın" />
        <Button @click="sendReply(review.id)">Cevap Gönder</Button>
      </div>
    </Card>
  </section>
</template>
