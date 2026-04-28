<template>
  <div
    class="fixed top-6 right-6 z-999 flex flex-col gap-3 w-full max-w-xs pointer-events-none"
  >
    <transition-group
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="transform translate-x-12 opacity-0"
      enter-to-class="transform translate-x-0 opacity-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-for="toast in toasts"
        :key="toast.id"
        class="pointer-events-auto flex items-center gap-3 p-4 rounded-2xl border backdrop-blur-md shadow-2xl transition-all"
        :class="[
          toast.type === 'success'
            ? 'bg-emerald-500/10 border-emerald-500/20 shadow-emerald-500/5'
            : toast.type === 'danger'
              ? 'bg-red-500/10 border-red-500/20 shadow-red-500/5'
              : 'bg-indigo-500/10 border-indigo-500/20 shadow-indigo-500/5',
        ]"
      >
        <div
          :class="[
            'shrink-0',
            toast.type === 'success'
              ? 'text-emerald-400'
              : toast.type === 'danger'
                ? 'text-red-500'
                : 'text-indigo-400',
          ]"
        >
          <CheckCircle v-if="toast.type === 'success'" :size="20" />
          <AlertCircle v-else-if="toast.type === 'danger'" :size="20" />
          <Info v-else :size="20" />
        </div>

        <p
          class="text-white text-[13px] font-bold tracking-tight leading-tight"
        >
          {{ toast.message }}
        </p>
      </div>
    </transition-group>
  </div>
</template>

<script setup lang="ts">
import { CheckCircle, AlertCircle, Info } from "lucide-vue-next";
import { useToast } from "../composables/useToast";

const { toasts } = useToast();
</script>
