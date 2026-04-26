import * as signalR from "@microsoft/signalr";

/**
 * Serviço responsável pela comunicação em tempo real com o Back-end via SignalR.
 */
class NotificationService {
  private connection: signalR.HubConnection | null = null;
  private readonly apiUrl: string = import.meta.env.VITE_API_URL;

  /**
   * Inicializa a conexão com o Hub de Notificações.
   * @param token O Token JWT do usuário autenticado.
   * @param onUpdateDashboard Callback disparado quando o back-end solicita atualização do dashboard.
   */
  public setupSignalR(token: string, onUpdateDashboard: () => void): signalR.HubConnection {
    // Evita criar múltiplas conexões se já houver uma ativa
    if (this.connection) {
      return this.connection;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${this.apiUrl}/hubs/notifications`, {
        accessTokenFactory: () => token,
        // O SignalR usará WebSockets por padrão, mas pode fazer fallback se necessário
        skipNegotiation: false, 
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000]) // Tenta reconectar em 0s, 2s, 5s e 10s
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Ouvinte para o evento disparado pelos Handlers no C#
    this.connection.on("AtualizarDashboard", () => {
      console.log("[SignalR] Notificação recebida: Atualizando dados do Dashboard...");
      onUpdateDashboard();
    });

    // Gerenciamento de estado da conexão (Opcional, mas bom para Logs)
    this.connection.onreconnecting((error) => {
      console.warn("[SignalR] Conexão perdida. Tentando reconectar...", error);
    });

    this.connection.onreconnected((connectionId) => {
      console.log("[SignalR] Conexão restabelecida. ID:", connectionId);
    });

    // Inicia a conexão
    this.startConnection();

    return this.connection;
  }

  private async startConnection() {
    try {
      if (this.connection) {
        await this.connection.start();
        console.log("[SignalR] Conectado com sucesso ao Hub.");
      }
    } catch (err) {
      console.error("[SignalR] Erro ao iniciar conexão. Tentando novamente em 5s...", err);
      setTimeout(() => this.startConnection(), 5000);
    }
  }

  /**
   * Encerra a conexão com o Hub (Útil para o Logout).
   */
  public async stopConnection() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      console.log("[SignalR] Conexão encerrada.");
    }
  }
}

// Exporta uma instância única (Singleton)
export const notificationService = new NotificationService();