<template>
  <aside
    class="w-64 bg-[#09090b] border-r border-white/5 h-screen sticky top-0 flex flex-col py-6"
  >
    <div class="px-6 mb-10">
      <div class="flex items-center gap-3">
        <div
          class="w-10 h-10 bg-[#6366f1] rounded-xl flex items-center justify-center shadow-lg shadow-indigo-500/20"
        >
          <span class="text-white font-black text-xl">$</span>
        </div>
        <h1 class="text-white font-black tracking-widest text-lg">FINANÇAS</h1>
      </div>
    </div>

    <nav class="flex-1 px-3 space-y-1">
      <router-link to="/dashboard" class="nav-link" active-class="active">
        <span class="icon">🏠</span> Dashboard
      </router-link>
      <router-link to="/transacoes" class="nav-link" active-class="active">
        <span class="icon">📑</span> Lançamentos
      </router-link>
      <router-link to="/categorias" class="nav-link" active-class="active">
        <span class="icon">📂</span> Categorias
      </router-link>
    </nav>

    <div class="px-6 pt-6 border-t border-white/5">
      <button
        @click="handleLogout"
        class="flex items-center gap-3 text-red-500 font-bold hover:opacity-80 transition-all text-sm"
      >
        <span>↪️</span> Sair
      </button>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router";

const router = useRouter();

const handleLogout = () => {
  localStorage.removeItem("token");
  router.push("/login");
};
</script>

<style scoped>
@reference "tailwindcss";

.nav-link {
  @apply flex items-center gap-3 px-4 py-3 rounded-xl text-gray-500 font-bold transition-all hover:text-white;
}

.active {
  @apply bg-[#6366f1]/10 text-[#6366f1] border-l-4 border-[#6366f1] rounded-none -ml-3 pl-6;
}

.icon {
  @apply text-lg;
}
</style>
