<template>
  <router-view />

  <ToastList />

  <ModalNotificacao
    :show="state.show"
    :type="state.type"
    :title="state.title"
    :message="state.message"
    :showCancel="state.showCancel"
    @confirm="handleConfirm"
    @cancel="state.show = false"
  />
</template>

<script setup lang="ts">
import ToastList from "@/shared/components/ToastList.vue";
import ModalNotificacao from "@/shared/components/ModalNotificacao.vue";
import { useNotification } from "@/shared/composables/useNotification";

// Trazemos o estado global das notificações
const { state } = useNotification();

// Quando o usuário clica em "Confirmar" no modal
const handleConfirm = () => {
  if (state.onConfirm) {
    state.onConfirm(); // Executa a função de exclusão que veio lá da TransacoesView
  }
  state.show = false; // Fecha o modal
};
</script>
