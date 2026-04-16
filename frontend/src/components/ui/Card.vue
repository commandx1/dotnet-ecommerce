<script setup lang="ts">
import { cn } from '@/lib/utils'

interface Props {
  class?: string
}

defineProps<Props>()
</script>

<template>
  <div :class="cn('rounded-lg border border-border bg-card p-5 shadow-sm', $props.class)">
    <slot />
  </div>
</template>
