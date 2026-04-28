import http from "@/shared/services/http";

export interface CategoriaResponse {
  id: string;
  nome: string;
  tipo: "R" | "D";
  icone: string;
  categoriaPaiId: string | null;
  subcategorias: CategoriaResponse[];
}

export const categoriaService = {
  async getCategorias(): Promise<CategoriaResponse[]> {
    const { data } = await http.get<CategoriaResponse[]>("/Categorias");
    return data;
  },
};
