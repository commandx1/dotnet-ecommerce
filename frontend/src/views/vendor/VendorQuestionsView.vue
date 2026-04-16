<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import Card from '@/components/ui/Card.vue'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { answerQuestion, getVendorQuestions } from '@/api/questionsApi'
import type { Question } from '@/types/question'
import { getApiErrorMessage } from '@/lib/api-error'

const loading = ref(false)
const error = ref<string | null>(null)
const questions = ref<Question[]>([])
const drafts = reactive<Record<string, string>>({})

async function loadQuestions() {
  loading.value = true
  error.value = null

  try {
    questions.value = await getVendorQuestions()
  } catch (requestError) {
    error.value = getApiErrorMessage(requestError, 'Unable to load vendor questions.')
  } finally {
    loading.value = false
  }
}

async function sendAnswer(questionId: string) {
  const answerText = drafts[questionId]?.trim()
  if (!answerText) {
    return
  }

  try {
    const updated = await answerQuestion(questionId, answerText)
    const index = questions.value.findIndex((question) => question.id === questionId)
    if (index >= 0) {
      questions.value[index] = updated
    }

    drafts[questionId] = ''
  } catch (requestError) {
    error.value = getApiErrorMessage(requestError, 'Unable to send the answer.')
  }
}

onMounted(async () => {
  await loadQuestions()
})
</script>

<template>
  <section class="space-y-4">
    <h2 class="text-2xl font-semibold">Vendor Q&amp;A Management</h2>

    <Card v-if="loading">Loading...</Card>
    <Card v-else-if="error">{{ error }}</Card>
    <Card v-else-if="questions.length === 0">No questions yet.</Card>

    <Card v-for="question in questions" :key="question.id" class="space-y-4">
      <div class="space-y-1">
        <p class="text-sm">{{ question.questionText }}</p>
      </div>

      <div v-if="question.answerText" class="rounded bg-muted p-3 text-sm">
        <span class="font-medium">Current answer:</span> {{ question.answerText }}
      </div>

      <div class="space-y-2">
        <Input v-model="drafts[question.id]" placeholder="Write an answer to this question" />
        <Button @click="sendAnswer(question.id)">Send Answer</Button>
      </div>
    </Card>
  </section>
</template>
