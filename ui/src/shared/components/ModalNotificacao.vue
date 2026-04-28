<template>
  <transition
    enter-active-class="transition duration-200 ease-out"
    enter-from-class="opacity-0 scale-95"
    enter-to-class="opacity-100 scale-100"
    leave-active-class="transition duration-150 ease-in"
    leave-from-class="opacity-100 scale-100"
    leave-to-class="opacity-0 scale-95"
  >
    <div
      v-if="show"
      class="fixed inset-0 z-9999 flex items-center justify-center p-4"
    >
      <div
        class="absolute inset-0 bg-black/80 backdrop-blur-sm"
        @click="handleCancel"
      ></div>

      <div
        class="relative bg-[#111114] w-full max-w-sm rounded-3xl border border-white/5 shadow-2xl p-8"
      >
        <div class="flex flex-col items-center text-center">
          <div
            :class="[
              'mb-6 p-4 rounded-2xl border',
              type === 'danger'
                ? 'bg-red-500/10 border-red-500/20 text-red-500'
                : type === 'success'
                  ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-400'
                  : 'bg-indigo-500/10 border-indigo-500/20 text-indigo-400',
            ]"
          >
            <AlertTriangle v-if="type === 'danger'" :size="32" />
            <CheckCircle v-else-if="type === 'success'" :size="32" />
            <Info v-else :size="32" />
          </div>

          <h3 class="text-white text-xl font-black tracking-tight mb-2">
            {{ title }}
          </h3>

          <p class="text-gray-500 text-[13px] leading-relaxed mb-8">
            {{ message }}
          </p>

          <div class="flex w-full gap-3">
            <button
              v-if="showCancel"
              @click="handleCancel"
              class="flex-1 py-3.5 text-gray-500 hover:text-white transition-colors text-[12px] font-black uppercase tracking-widest rounded-xl hover:bg-white/5"
            >
              Cancelar
            </button>

            <button
              @click="handleConfirm"
              :class="[
                'flex-1 py-3.5 rounded-xl text-[12px] font-black uppercase tracking-widest text-white transition-all shadow-lg active:scale-95',
                type === 'danger'
                  ? 'bg-red-500 hover:bg-red-600 shadow-red-500/20'
                  : 'bg-[#6366f1] hover:bg-[#4f46e5] shadow-indigo-500/20',
              ]"
            >
              Confirmar
            </button>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { AlertTriangle, CheckCircle, Info } from "lucide-vue-next";

interface Props {
  show: boolean;
  type?: "info" | "danger" | "success";
  title: string;
  message: string;
  showCancel?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  type: "info",
  showCancel: true,
});

const emit = defineEmits(["confirm", "cancel"]);

const handleConfirm = () => emit("confirm");
const handleCancel = () => {
  if (props.showCancel) {
    emit("cancel");
  }
};
</script>

<style scoped>
@reference "tailwindcss";
</style>
