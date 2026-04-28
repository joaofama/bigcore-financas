export interface Categoria {
  id: string;
  nome: string;
  tipo: "R" | "D";
  icone: string;
  categoriaPaiId: string | null;
  subcategorias: Categoria[];
}
