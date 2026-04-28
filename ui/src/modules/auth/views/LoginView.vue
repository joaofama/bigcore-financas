<template>
  <div
    class="min-h-screen bg-black flex flex-col items-center justify-center px-4 font-sans"
  >
    <div class="text-center mb-8 max-w-xl">
      <h1 class="text-white text-4xl font-bold mb-4">
        Controle Financeiro Pessoal
      </h1>
      <p class="text-gray-400 text-sm leading-relaxed">
        Para onde está indo meu dinheiro? Para administrar bem suas finanças,
        você precisa saber quanto dinheiro ganha (receitas) e para onde ele vai
        (despesas).
      </p>
    </div>

    <div
      class="bg-card-bg p-10 rounded-2xl border border-gray-800 w-full max-w-110 shadow-2xl"
    >
      <h2 class="text-white text-3xl font-semibold text-center mb-10">Login</h2>

      <form @submit.prevent="handleLogin" class="space-y-6">
        <div>
          <label
            class="block text-gray-400 text-xs font-bold uppercase mb-2 tracking-wider"
            >Email</label
          >
          <input
            v-model="form.email"
            type="email"
            placeholder="seu@email.com"
            required
            class="w-full bg-[#1e1e1e] border border-gray-700 rounded-lg py-3 px-4 text-white placeholder-gray-600 outline-none focus:border-brand-blue transition-all"
          />
        </div>

        <div>
          <label
            class="block text-gray-400 text-xs font-bold uppercase mb-2 tracking-wider"
            >Senha</label
          >
          <input
            v-model="form.password"
            type="password"
            placeholder="********"
            required
            class="w-full bg-[#1e1e1e] border border-gray-700 rounded-lg py-3 px-4 text-white placeholder-gray-600 outline-none focus:border-brand-blue transition-all"
          />
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full bg-brand-blue hover:bg-blue-600 text-white font-bold py-3.5 rounded-lg transition-all transform active:scale-[0.98] shadow-lg shadow-blue-500/10 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <span v-if="loading">Autenticando...</span>
          <span v-else>Entrar</span>
        </button>
      </form>
    </div>

    <footer class="mt-12">
      <p class="text-gray-600 text-xs tracking-tight">
        © 2026 Finanças App. Todos os direitos reservados.
      </p>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../stores/authStore";

const router = useRouter();
const authStore = useAuthStore();

const loading = ref(false);
const form = reactive({
  email: "",
  password: "",
});

const handleLogin = async () => {
  loading.value = true;

  try {
    // Chama a action de login da Store
    await authStore.login({
      email: form.email,
      senha: form.password, // Verifique se o seu backend espera 'senha' ou 'password'
    });

    // Se o login for bem sucedido, redireciona para o Dashboard
    router.push("/dashboard");
  } catch (error: any) {
    console.error("Erro ao fazer login:", error);
    alert("Falha na autenticação. Verifique seu e-mail e senha.");
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
/* Estilos específicos caso queira refinar algo que o Tailwind não cubra */
input:-webkit-autofill,
input:-webkit-autofill:hover,
input:-webkit-autofill:focus {
  -webkit-text-fill-color: white;
  -webkit-box-shadow: 0 0 0px 1000px #1e1e1e inset;
  transition: background-color 5000s ease-in-out 0s;
}
</style>
