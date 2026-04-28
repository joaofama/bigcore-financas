import * as LucideIcons from "lucide-vue-next";
import type { Component } from "vue";

export const getLucideIcon = (
  iconName: string | null | undefined,
): Component => {
  if (!iconName) return LucideIcons.Package;

  const pascalName = iconName
    .trim()
    .split(/[- ]+/) // Divide por traço ou espaço
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1).toLowerCase())
    .join("");

  // Busca no objeto de ícones
  const icon = (LucideIcons as any)[pascalName];

  // Se o usuário digitou algo que ainda não existe, retorna o Package (ou outro padrão)
  return icon || LucideIcons.Package;
};
