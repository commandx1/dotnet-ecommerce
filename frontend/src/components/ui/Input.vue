<script setup lang="ts">
import { cn } from '@/lib/utils'

interface Props {
  modelValue: string | number
  class?: string
  type?: string
  placeholder?: string
}

defineProps<Props>()
const emit = defineEmits<{ 'update:modelValue': [value: string] }>()

function onInput(event: Event) {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>

<template>
  <input
    :value="modelValue"
    :type="type || 'text'"
    :placeholder="placeholder"
    :class="
      cn(
        'flex h-10 w-full rounded-md border border-border bg-white px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary',
        $props.class
      )
    "
    @input="onInput"
  />
</template>
