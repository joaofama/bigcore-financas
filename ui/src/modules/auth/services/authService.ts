import http from "@/shared/services/http";
import type { LoginRequest, LoginResponse } from "../types";

export const authService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    // Detalhes da API ficam escondidos aqui
    const { data } = await http.post<LoginResponse>("/Auth/login", credentials);
    return data;
  },
};
