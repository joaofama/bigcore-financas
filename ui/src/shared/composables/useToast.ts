import { ref } from "vue";

interface Toast {
  id: number;
  message: string;
  type: "success" | "danger" | "info";
}

const toasts = ref<Toast[]>([]);
let nextId = 0;

export function useToast() {
  const add = (
    message: string,
    type: "success" | "danger" | "info" = "success",
  ) => {
    const id = nextId++;
    toasts.value.push({ id, message, type });

    // Remove automaticamente após 3 segundos
    setTimeout(() => {
      toasts.value = toasts.value.filter((t) => t.id !== id);
    }, 3000);
  };

  return {
    toasts,
    success: (msg: string) => add(msg, "success"),
    error: (msg: string) => add(msg, "danger"),
    info: (msg: string) => add(msg, "info"),
  };
}
