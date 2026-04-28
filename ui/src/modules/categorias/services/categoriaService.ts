import http from "@/shared/services/http";
import type { Categoria } from "../types";

export const categoriaService = {
  /**
   * Busca todas as categorias do usuário em formato de árvore.
   * Substitui o antigo 'getAll'.
   */
  async getCategorias(): Promise<Categoria[]> {
    const { data } = await http.get<Categoria[]>("/Categorias");
    return data;
  },

  /**
   * Busca os detalhes de uma única categoria pelo ID.
   */
  async getById(id: string): Promise<Categoria> {
    const { data } = await http.get<Categoria>(`/Categorias/${id}`);
    return data;
  },

  /**
   * Cadastra uma nova categoria ou subcategoria.
   */
  async create(payload: {
    nome: string;
    icone: string;
    tipo: string;
    categoriaPaiId: string | null;
  }): Promise<Categoria> {
    const { data } = await http.post<Categoria>("/Categorias", payload);
    return data;
  },

  /**
   * Atualiza os dados de uma categoria existente.
   */
  async update(
    id: string,
    payload: {
      nome: string;
      icone: string;
      tipo: string;
      categoriaPaiId: string | null;
    },
  ): Promise<void> {
    await http.put(`/Categorias/${id}`, payload);
  },

  /**
   * Remove uma categoria e limpa o cache no back-end.
   */
  async delete(id: string): Promise<void> {
    await http.delete(`/Categorias/${id}`);
  },
};
