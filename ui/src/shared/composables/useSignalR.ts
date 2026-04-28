import {
  HubConnectionBuilder,
  HubConnection,
  LogLevel,
} from "@microsoft/signalr";
import { ref } from "vue";

export function useSignalR() {
  const connection = ref<HubConnection | null>(null);

  const startConnection = async (token: string) => {
    const hubUrl = `${import.meta.env.VITE_API_URL}/hubs/notifications`;

    connection.value = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        // Envia o JWT para o backend. O SignalR usa isso para preencher o Context.User
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    try {
      await connection.value.start();
      console.log("SignalR: Conectado com sucesso ao hub de notificações!");
    } catch (err) {
      console.error("SignalR: Erro ao conectar:", err);
    }
  };

  const stopConnection = async () => {
    if (connection.value) {
      await connection.value.stop();
      console.log("⚪ SignalR: Conexão encerrada.");
    }
  };

  return { connection, startConnection, stopConnection };
}
