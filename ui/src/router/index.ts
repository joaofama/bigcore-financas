import { createRouter, createWebHistory } from "vue-router";
import LoginView from "@/modules/auth/views/LoginView.vue";
import DashboardView from "@/modules/dashboard/views/DashboardView.vue";
// Importe o seu MainLayout (ajuste o caminho se ele estiver em outra pasta)
import MainLayout from "@/components/layout/MainLayout.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      redirect: "/login",
    },
    {
      path: "/login",
      name: "login",
      component: LoginView,
    },
    // A MÁGICA ACONTECE AQUI:
    // Criamos uma rota para o Layout e colocamos o Dashboard dentro dela
    {
      path: "/",
      component: MainLayout,
      meta: { requiresAuth: true }, // Protege o layout e todas as telas dentro dele
      children: [
        {
          // O caminho vazio ou com nome fará o Dashboard carregar dentro do <router-view> do MainLayout
          path: "dashboard",
          name: "dashboard",
          component: DashboardView,
        },
        // No futuro, a rota de 'lancamentos' vai entrar aqui também!
      ],
    },
  ],
});

// Proteção de rotas (Navigation Guard)
router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem("token");

  if (to.meta.requiresAuth && !token) {
    next({ name: "login" });
  } else if (to.name === "login" && token) {
    next({ name: "dashboard" });
  } else {
    next();
  }
});

export default router;
