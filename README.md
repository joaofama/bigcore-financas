# 💰 Finanças API & Dashboard Real-time

Este projeto é uma solução de alta performance para gestão financeira, projetada com foco em escalabilidade, testabilidade e manutenibilidade. A arquitetura foi desenvolvida seguindo os rigorosos padrões de **Clean Architecture**, guiada por **Domain-Driven Design (DDD)** e fundamentada nos princípios **SOLID**.

---

## 🚀 Tecnologias e Padrões de Projeto

### **Core & Back-end**
* **.NET 9** (C#)
* **SOLID Principles**: Aplicação rigorosa dos cinco princípios para um código limpo e extensível.
* **Domain-Driven Design (DDD)**: Modelagem focada no domínio (Entidades, Value Objects e Repositórios).
* **Clean Architecture**: Separação total de interesses entre as camadas de Domínio, Aplicação, Infraestrutura e API.
* **CQRS com MediatR**: Desacoplamento entre comandos (escrita) e consultas (leitura).
* **SignalR + Redis Backplane**: Mensageria real-time escalável.
* **MongoDB & Redis**: Persistência NoSQL e Cache/Backplane de alta performance.

### **Front-end**
* **Vue.js 3** (TypeScript + Vite)
* **Tailwind CSS** & **SignalR Client**

---

## 🔑 Credenciais de Teste

Para facilitar a avaliação, o sistema já conta com um usuário de teste pré-configurado:

* **E-mail:** `teste@teste.com`
* **Senha:** `teste@123`

---

## 🛠️ Como Executar via Docker (Obrigatório)

Siga os passos abaixo para subir o ecossistema completo:

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/seu-usuario/seu-repositorio.git
    cd BigCore
    ```

2.  **Configuração do Ambiente:**
    > ⚠️ **IMPORTANTE:** O arquivo `.env` não é versionado.
    > 
    > **Renomeie o arquivo `.env-example` para `.env` na raiz do projeto antes de iniciar.**
    > No terminal: `cp .env-example .env` (ou via Windows Explorer).

3.  **Suba os containers:**
    ```bash
    docker-compose up --build
    ```

4.  **Acesse as interfaces:**
    * **Aplicação Web:** [http://localhost:3000](http://localhost:3000)
    * **Swagger (Documentação):** [http://localhost:5000/swagger](http://localhost:5000/swagger)
    * **Mongo Express:** [http://localhost:8081](http://localhost:8081)
    * **Redis Commander:** [http://localhost:8082](http://localhost:8082)

---

## 📂 Estrutura do Projeto (Clean Arch & DDD)

* **`src/Financas.Domain`**: O centro da aplicação. Contém Entidades e Interfaces de Repositório (Aplicação do **D** do SOLID).
* **`src/Financas.Application`**: Casos de uso e Handlers (Aplicação do **S**: Responsabilidade Única).
* **`src/Financas.Infrastructure`**: Implementações técnicas (MongoDB, Redis, SignalR).
* **`src/Financas.API`**: Controllers, Middlewares e Hubs.
* **`ui/`**: Interface SPA em Vue.js.

---

## 🧠 Decisões Arquiteturais e Princípios

O projeto foi construído sobre os pilares do **SOLID**:
* **S (Single Responsibility):** Handlers específicos para cada ação de negócio.
* **O (Open/Closed):** Extensibilidade através de interfaces.
* **L (Liskov Substitution):** Contratos de serviços e repositórios consistentes.
* **I (Interface Segregation):** Interfaces magras e segregadas por propósito.
* **D (Dependency Inversion):** A camada de Domínio nunca depende de detalhes técnicos.




