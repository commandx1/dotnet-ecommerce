<script setup lang="ts">
import { cn } from '@/lib/utils'

interface Props {
  class?: string
}

defineProps<Props>()
</script>

<template>
  <div
    :class="
      cn(
        'rounded-2xl border border-border/85 bg-card/90 p-5 shadow-[0_12px_40px_-24px_rgba(10,39,76,0.45)] backdrop-blur',
        $props.class
      )
    "
  >
    <slot />
  </div>
</template>
