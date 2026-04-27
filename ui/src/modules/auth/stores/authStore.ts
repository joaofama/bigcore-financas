import { defineStore } from "pinia";
import { authService } from "../services/authService";
import type { LoginRequest } from "../types"; // Criaremos este arquivo de tipos logo abaixo

export const useAuthStore = defineStore("auth", {
  // 1. Estado: Onde os dados "vivem"
  state: () => ({
    token: localStorage.getItem("token") || "",
    user: null as any, // Você pode tipar como 'Usuario | null' depois
    loading: false,
  }),

  // 2. Getters: Como se fossem "propriedades computadas" da Store
  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  // 3. Actions: Métodos que alteram o estado (podem ser assíncronos)
  actions: {
    async login(credentials: LoginRequest) {
      this.loading = true;
      try {
        // Chamada ao Service (Garçom)
        const data = await authService.login(credentials);

        // Atualiza o estado da Store
        this.token = data.token;
        // Se sua API retornar dados do usuário no login, salve aqui:
        // this.user = data.user;

        // Persistência simples para não deslogar ao dar F5
        localStorage.setItem("token", data.token);

        return data;
      } catch (error) {
        this.logout(); // Limpa tudo se der erro
        throw error;
      } finally {
        this.loading = false;
      }
    },

    logout() {
      this.token = "";
      this.user = null;
      localStorage.removeItem("token");
    },
  },
});
