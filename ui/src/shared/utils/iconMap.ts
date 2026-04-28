import * as LucideIcons from "lucide-vue-next";
import type { Component } from "vue";

/**
 * Busca o componente Lucide diretamente pelo nome exato vindo do banco.
 */
export const getLucideIcon = (
  iconName: string | null | undefined,
): Component => {
  if (!iconName) return LucideIcons.Package;

  // Busca direta: se o banco diz "Pizza", pegamos LucideIcons["Pizza"]
  const icon = (LucideIcons as any)[iconName];

  // Se o ícone não existir na biblioteca (erro de digitação no banco, por exemplo),
  // ele retorna o 'Package' como padrão.
  return icon || LucideIcons.Package;
};
