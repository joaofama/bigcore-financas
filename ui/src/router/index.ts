import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/modules/auth/stores/authStore';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('@/modules/auth/views/LoginView.vue'),
      name: 'login',
      meta: { requiresAuth: false }
    },
    {
      path: '/dashboard',
      component: () => import('@/modules/dashboard/views/DashboardView.vue'),
      name: 'dashboard',
      meta: { requiresAuth: true } // Esta rota exige login
    }
  ]
});


router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const isAuthenticated = !!authStore.token; // Verifica se existe token

  if (to.meta.requiresAuth && !isAuthenticated) {
    next({ name: 'login' });
  } 
  else if (to.name === 'login' && isAuthenticated) {
    next({ name: 'dashboard' });
  } 
  else {
    next();
  }
});

export default router;