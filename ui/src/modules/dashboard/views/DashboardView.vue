<template>
  <div class="min-h-screen bg-black text-white p-8 font-sans">
    <div class="max-w-4xl mx-auto">
      <header class="flex justify-between items-center mb-12 border-b border-gray-800 pb-6">
        <h1 class="text-3xl font-bold text-brand-blue">Finanças</h1>
        <button 
          @click="handleLogout" 
          class="text-gray-400 hover:text-red-500 transition-colors text-sm font-medium"
        >
          Sair da conta
        </button>
      </header>

      <main>
        <div class="bg-[#121212] border border-gray-800 rounded-2xl p-8 shadow-xl text-center">
          <h2 class="text-xl mb-4">Bem-vindo ao seu Dashboard!</h2>
          <p class="text-gray-400">
            Se você está vendo esta tela, significa que o **Login funcionou**, o **Token foi salvo** e a comunicação com o **Docker** está ativa. 🚀
          </p>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/modules/auth/stores/authStore';

const router = useRouter();
const authStore = useAuthStore();

const handleLogout = () => {
  authStore.logout();
  router.push('/');
};
</script>