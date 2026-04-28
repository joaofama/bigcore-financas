<template>
  <div class="flex min-h-screen bg-black text-gray-400 font-sans">
    <AppSidebar />

    <div class="flex-1 flex flex-col">
      <header
        class="h-16 flex items-center justify-between px-8 bg-[#09090b] border-b border-white/5"
      >
        <div class="flex items-center gap-4">
          <span class="text-white text-xl cursor-pointer">☰</span>
          <h2
            class="text-white font-black text-[10px] tracking-widest uppercase"
          >
            {{ $route.name }}
          </h2>
        </div>

        <div class="flex items-center gap-3">
          <div class="text-right">
            <p
              class="text-[9px] text-gray-500 font-bold uppercase leading-none"
            >
              USUÁRIO
            </p>
            <p class="text-white text-sm font-bold">
              {{ authStore.userName || "Convidado" }}
            </p>
          </div>
          <div
            class="w-10 h-10 rounded-full bg-[#18181b] border border-white/10 flex items-center justify-center text-xs font-bold text-gray-500"
          >
            {{ userInitials }}
          </div>
        </div>
      </header>

      <main class="p-6 flex-1 flex flex-col">
        <router-view />
      </main>

      <footer
        class="p-6 text-center text-[10px] text-gray-600 uppercase tracking-widest"
      >
        &copy; 2026 BigCore
      </footer>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import AppSidebar from "@/components/layout/AppSidebar.vue";
import { useAuthStore } from "@/modules/auth/stores/authStore"; // Ajuste o caminho se necessário

const authStore = useAuthStore();

// ⚡ Lógica para extrair as iniciais (ex: "Usuário de Teste" -> "UT")
const userInitials = computed(() => {
  const name = authStore.userName;
  if (!name) return "??";

  const parts = name.split(" ").filter((p) => p.length > 0);
  if (parts.length === 0) return "??";
  if (parts.length === 1) return parts[0].substring(0, 2).toUpperCase();

  return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
});
</script>
