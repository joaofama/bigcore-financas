import http from "@/shared/http";
import type { LoginRequest, LoginResponse } from "../types";

export const authService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    // Detalhes da API ficam escondidos aqui
    const { data } = await http.post<LoginResponse>("/Auth/login", credentials);
    return data;
  },

  // Exemplo de outro método que ficaria aqui no futuro
  async register(userData: any) {
    return await http.post("/Auth/register", userData);
  },
};
