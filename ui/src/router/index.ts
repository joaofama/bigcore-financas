import { createRouter, createWebHistory } from "vue-router";
import LoginView from "@/modules/auth/views/LoginView.vue";
import DashboardView from "@/modules/dashboard/views/DashboardView.vue";
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
    {
      path: "/",
      component: MainLayout,
      meta: { requiresAuth: true }, // Protege o layout e todas as telas dentro dele
      children: [
        {
          path: "dashboard",
          name: "dashboard",
          component: DashboardView,
        },
        {
          path: "transacoes",
          name: "Lançamentos",
          component: () =>
            import("@/modules/transacoes/views/TransacoesView.vue"),
        },
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
