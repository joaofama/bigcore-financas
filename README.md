# 💰 Finanças API & Dashboard Real-time

Este projeto é uma solução de alta performance para gestão financeira, projetada com foco em escalabilidade, testabilidade e manutenibilidade. A arquitetura foi desenvolvida seguindo os rigorosos padrões de **Clean Architecture**, guiada por **Domain-Driven Design (DDD)** e fundamentada nos princípios **SOLID**.

---

## 🚀 Tecnologias e Padrões de Projeto

### **Core & Back-end**
* **.NET 9** (C#)
* **SOLID Principles**: Aplicação rigorosa dos cinco princípios para um código limpo e extensível.
* **Domain-Driven Design (DDD)**: Modelagem focada no domínio (Entidades, Value Objects e Repositórios).
* **Clean Architecture**: Separação total de interesses entre as camadas de Domínio, Aplicação, Infraestrutura e API.
* **CQRS com MediatR**: Desacoplamento entre comandos (escrita) e consultas (leitura) através de Handlers.
* **SignalR + Redis Backplane**: Mensageria real-time escalável para atualizações de dashboard.
* **MongoDB & Redis**: Persistência NoSQL (JSON/Bson) e Cache de alta performance.

### **Front-end**
* **Vue.js 3** (TypeScript + Vite)
* **Tailwind CSS** & **SignalR Client**

---

## 🔗 Interfaces e Credenciais de Acesso

Após subir os containers, utilize os links e credenciais abaixo para navegar pelo ecossistema do projeto:

### **1. Aplicação e API**
Para testar as funcionalidades do sistema e visualizar a documentação:
* **Aplicação Web (Dashboard):** http://localhost:3000
* **Swagger (Documentação da API):** http://localhost:5000/swagger
* **Credenciais do Usuário de Teste:**
  * **E-mail:** `teste@teste.com`
  * **Senha:** `teste@123`

### **2. Infraestrutura (Bancos de Dados e Cache)**
As credenciais abaixo são baseadas no arquivo `.env` padrão configurado no Docker:

* **Mongo Express (Interface Web do MongoDB)**
  * **URL:** http://localhost:8081
  * **Usuário:** `admin`
  * **Senha:** `pass`

* **MongoDB (Conexão direta via Compass ou Shell)**
  * **Host:** `localhost:27017`
  * **Usuário:** `admin`
  * **Senha:** `admin123`
  * **Database:** `FinancasDb`

* **Redis Commander (Interface Web do Redis)**
  * **URL:** http://localhost:8082

---

## 🛠️ Como Executar via Docker (Obrigatório)

Siga os passos abaixo para subir o ecossistema completo:

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/seu-usuario/seu-repositorio.git
   cd BigCore
   ```

2. **Configuração do Ambiente:**
   > ⚠️ **IMPORTANTE:** O arquivo `.env` não é versionado por segurança.
   > 
   > **Renomeie o arquivo `.env-example` para `.env` na raiz do projeto antes de iniciar.**
   > No terminal: `cp .env-example .env` (ou via Windows Explorer).

3. **Suba os containers:**
   ```bash
   docker-compose up --build
   ```
   *(Após o build, consulte a seção de Interfaces e Credenciais acima para acessar o sistema).*

---

## 📂 Estrutura do Projeto (Clean Arch & DDD)

O projeto está dividido em camadas para garantir que a lógica de negócio seja independente de frameworks externos:

* **`src/Financas.Domain`**: O coração da aplicação. Contém as Entidades (como `Usuario`), interfaces de repositório e regras de negócio puras.
* **`src/Financas.Application`**: Contém os Commands, Queries, Handlers e Requests. É onde os casos de uso são orquestrados via MediatR.
* **`src/Financas.Infrastructure`**: Implementações técnicas. Configurações do MongoDB (Contexto e Mapeamentos), Repositórios e integração com Redis.
* **`src/Financas.API`**: Camada de entrada. Controllers que recebem `Requests` e despacham `Commands`, além da configuração do Token JWT e SignalR.
* **`ui/`**: Interface Single Page Application (SPA) moderna em Vue.js.

---

## 📝 Notas de Versão
* O banco de dados é populado automaticamente via scripts de seed no Docker (`01-mongodb-init-schema.js` e `02-mongodb-init-data.js`).
* Os IDs são armazenados nativamente como `BinData` (UUID) no MongoDB para máxima performance.