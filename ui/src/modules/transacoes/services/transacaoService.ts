import http from "@/shared/services/http";

export interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  data: string;
  tipo: "R" | "D";
  categoriaId: string;
  categoriaNome: string;
  categoriaIcone: string;
  categoriaPaiId: string;
  categoriaPaiNome: string;
  categoriaPaiIcone: string;
}

export interface TransacoesMesResponse {
  saldoInicial: number;
  totalReceitas: number;
  totalDespesas: number;
  saldoAtual: number;
  transacoes: Transacao[];
}

export const transacaoService = {
  /**
   * Busca o resumo financeiro (saldos) e a lista de transações do mês/ano.
   */
  async getPorMes(mes: number, ano: number): Promise<TransacoesMesResponse> {
    const { data } = await http.get<TransacoesMesResponse>(
      `/Transacoes/mes/${mes}/ano/${ano}`,
    );
    return data;
  },

  /**
   * Obtém os detalhes de uma única transação pelo ID.
   */
  async getById(id: string): Promise<Transacao> {
    const { data } = await http.get<Transacao>(`/Transacoes/${id}`);
    return data;
  },

  /**
   * Cria uma nova transação e retorna o objeto completo com snapshots.
   */
  async criar(payload: Partial<Transacao>): Promise<Transacao> {
    const { data } = await http.post<Transacao>("/Transacoes", payload);
    return data;
  },

  /**
   * Atualiza os dados de uma transação existente.
   */
  async atualizar(id: string, payload: Partial<Transacao>): Promise<void> {
    await http.put(`/Transacoes/${id}`, payload);
  },

  /**
   * Remove uma transação.
   */
  async remover(id: string): Promise<void> {
    await http.delete(`/Transacoes/${id}`);
  },
};
