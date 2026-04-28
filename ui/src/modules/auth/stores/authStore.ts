import { defineStore } from "pinia";
import { authService } from "../services/authService";
import type { LoginRequest } from "../types";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: localStorage.getItem("token") || "",
    // ⚡ Adicionado: Tenta pegar o nome salvo, ou define como vazio
    userName: localStorage.getItem("userName") || "",
    loading: false,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  actions: {
    async login(credentials: LoginRequest) {
      this.loading = true;
      try {
        const data = await authService.login(credentials);

        this.token = data.token;
        this.userName = data.nome; // ⚡ Salva no estado

        localStorage.setItem("token", data.token);
        localStorage.setItem("userName", data.nome); // ⚡ Persiste o nome no navegador

        return data;
      } catch (error) {
        this.logout();
        throw error;
      } finally {
        this.loading = false;
      }
    },

    logout() {
      this.token = "";
      this.userName = ""; // ⚡ Limpa o nome
      localStorage.removeItem("token");
      localStorage.removeItem("userName"); // ⚡ Remove do navegador
    },
  },
});
