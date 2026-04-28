import { reactive } from "vue";

interface NotificationState {
  show: boolean;
  type: "success" | "danger" | "info";
  title: string;
  message: string;
  showCancel: boolean;
  onConfirm?: () => void;
}

const state = reactive<NotificationState>({
  show: false,
  type: "success",
  title: "",
  message: "",
  showCancel: false,
});

export function useNotification() {
  const success = (message: string, title = "Sucesso!") => {
    state.type = "success";
    state.title = title;
    state.message = message;
    state.showCancel = false;
    state.show = true;
  };

  const error = (message: string, title = "Erro!") => {
    state.type = "danger";
    state.title = title;
    state.message = message;
    state.showCancel = false;
    state.show = true;
  };

  const confirm = (
    message: string,
    onConfirm: () => void,
    title = "Confirmação",
  ) => {
    state.type = "danger";
    state.title = title;
    state.message = message;
    state.showCancel = true;
    state.onConfirm = onConfirm;
    state.show = true;
  };

  return { state, success, error, confirm };
}
