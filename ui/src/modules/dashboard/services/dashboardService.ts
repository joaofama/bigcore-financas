import http from "@/shared/services/http";

export interface DespesaPorCategoria {
  categoria: string;
  valor: number;
}

export interface DashboardResponse {
  saldoInicial: number;
  totalReceitas: number;
  totalDespesas: number;
  saldoAtual: number;
  graficoDespesas: DespesaPorCategoria[];
}

export const dashboardService = {
  async getResumo(mes: number, ano: number): Promise<DashboardResponse> {
    const { data } = await http.get<DashboardResponse>("/dashboard", {
      params: { mes, ano },
    });
    return data;
  },
};
