
---

# 💰 Finanças API & Dashboard Real-time (BigCore)

Este projeto é uma solução de alta performance para gestão financeira, projetada com foco em escalabilidade, testabilidade e manutenibilidade. A arquitetura foi desenvolvida seguindo os rigorosos padrões de **Clean Architecture**, guiada por **Domain-Driven Design (DDD)** e fundamentada nos princípios **SOLID**.

---

## 🚀 Tecnologias e Padrões de Projeto

### **Core & Back-end**
* **.NET 9 (C#)**: Plataforma principal de alto desempenho.
* **SOLID Principles**: Aplicação rigorosa dos cinco princípios para um código limpo e extensível.
* **Domain-Driven Design (DDD)**: Modelação focada no domínio (Entidades, Value Objects e Repositórios).
* **Clean Architecture**: Separação total de interesses entre as camadas de Domínio, Aplicação, Infraestrutura e API.
* **CQRS com MediatR**: Desacoplamento entre comandos (escrita) e consultas (leitura) através de Handlers.
* **SignalR + Redis Backplane**: Mensajeria real-time escalável para atualizações instantâneas de dashboard.
* **MongoDB & Redis**: Persistência NoSQL (JSON/Bson) e Cache de alta performance com limpeza automática.

### **Front-end**
* **Vue.js 3 (TypeScript + Vite)**: Framework moderno e tipado para uma UI reativa.
* **Tailwind CSS**: Estilização utilitária focada em design system escuro (Dark Theme).
* **Lucide Vue Next**: Biblioteca de ícones dinâmica integrada ao mapeamento de categorias.

---

## 🛠️ Como Executar via Docker (Obrigatório)

O projeto é 100% contentorizado e configurável via variáveis de ambiente. Siga os passos abaixo:

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/joaofama/bigcore-financas.git
   cd bigcore-financas
   ```

2. **Configuração do Ambiente:**
   > ⚠️ **IMPORTANTE:** O arquivo `.env` não é versionado por segurança.
   > 
   > **Renomeie o arquivo `.env-example` para `.env` na raiz do projeto.**


3. **Suba os contentores:**
   ```bash
   docker-compose up -d --build
   ```

### ✅ Confirmação de Sucesso
Após o processo de build e inicialização, o terminal deverá exibir o status abaixo em verde (via `diff` highlighting):

```diff
[+] up 48/48d intermediate container 
+ ⠿ Image financas-api                             Built
+ ⠿ Image financas-ui                              Built
+ ⠿ Network financas_default                       Created
+ ⠿ Container financas-redis                       Started
+ ⠿ Container financas-mongodb                     Started
+ ⠿ Container financas-redis-commander             Started
+ ⠿ Container financas-api                         Started
+ ⠿ Container financas-mongo-express               Started
+ ⠿ Container financas-ui                          Started
```

---

## 🔗 Interfaces e Credenciais de Acesso

Após subir os contentores, utilize os links e credenciais abaixo para navegar pelo ecossistema do projeto:

### **1. Aplicação e API**
| Recurso | URL | Objetivo |
| :--- | :--- | :--- |
| **Aplicação Web** | [http://localhost:3000](http://localhost:3000) | Dashboard e Gestão Financeira |
| **Swagger UI** | [http://localhost:5000/swagger](http://localhost:5000/swagger) | Documentação e Testes da API |

> **Credenciais de Teste:**
> * **E-mail:** `teste@teste.com`
> * **Senha:** `teste@123`

### **2. Infraestrutura**
| Recurso | URL | Credenciais |
| :--- | :--- | :--- |
| **Mongo Express** | [http://localhost:8081](http://localhost:8081) | Utilizador: `admin` / Senha: `pass` |
| **Redis Commander** | [http://localhost:8082](http://localhost:8082) | Visualização de Cache Real-time |
| **MongoDB Host** | `localhost:27017` | User: `admin` / Pass: `admin123` |

---

## ⚡ Como Testar o Real-time (SignalR) na Prática

Para visualizar a comunicação via WebSockets a acontecer em tempo real, sugerimos o seguinte teste prático:

1. **Abra duas janelas do navegador lado a lado:**
   * **Janela 1:** Acesse a tela inicial do **Dashboard** (onde estão os gráficos e os saldos totais).
   * **Janela 2:** Acesse a tela de **Lançamentos** (ou Categorias).
2. **Faça uma alteração:** Na Janela 2, adicione, edite ou exclua uma transação qualquer.
3. **Observe a Mágica:** Sem precisar de recarregar a página (F5), repare na Janela 1. Os saldos, receitas e despesas serão recalculados e os gráficos vão animar automaticamente refletindo os novos dados.

---

## 📂 Estrutura do Projeto (Clean Arch & DDD)

O projeto está dividido para garantir que a lógica de negócio nunca dependa de detalhes técnicos:

* **`src/Financas.Domain`**: O "Coração". Entidades, Value Objects e interfaces de contrato. **Zero dependências externas.**
* **`src/Financas.Application`**: Orquestração. Handlers de MediatR, DTOs e validações de casos de uso.
* **`src/Financas.Infrastructure`**: Detalhes técnicos. Implementação de Repositórios MongoDB, Serviços de Cache Redis e SignalR Hubs.
* **`src/Financas.API`**: Porta de entrada. Controllers, Middlewares de exceção e configuração dinâmica via `.env`.
* **`ui/`**: Interface SPA moderna. Comunica-se com a API via Axios e recebe atualizações real-time via WebSockets.

---

## 📝 Notas Técnicas
* **Segurança**: O sistema utiliza autenticação via JWT com suporte a tokens passados via QueryString para ligações SignalR.
* **Performance**: As chaves do Redis são prefixadas dinamicamente (`RedisSettings__InstanceName`) e limpas automaticamente em qualquer operação de escrita (POST/PUT/DELETE).
* **Seed de Dados**: O banco de dados é populado automaticamente no primeiro boot através dos scripts localizados em `/docker/mongo-init/`.

---