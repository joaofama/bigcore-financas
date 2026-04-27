import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import tailwindcss from "@tailwindcss/vite";
import path from "path";

export default defineConfig({
  plugins: [vue(), tailwindcss()],

  envDir: "../",

  resolve: {
    alias: {
      // Este mapeamento transforma "@" no caminho absoluto da pasta "src"
      "@": path.resolve(__dirname, "./src"),
    },
  },

  server: {
    port: 5173,
    // Abre o navegador automaticamente ao rodar npm run dev
    open: true,
  },
});
