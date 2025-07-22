# 🚀 CS-Chat: Um Chat com Interface de Terminal

Bem-vindo ao CS-Chat! Um chat em tempo real com backend em .NET e frontend em Vue.js, estilizado para parecer um terminal de computador. Autentique-se, crie salas, converse em privado ou em grupo, tudo através de comandos.

![Demo do Terminal Chat](https://i.imgur.com/kWisxZ8.gif)

---

## ✨ Funcionalidades

* **Interface de Terminal:** Uma experiência de usuário única que simula um terminal de linha de comando.
* **Autenticação:** Sistema seguro de registro e login de usuários.
* **Comandos:** Interaja com o chat usando comandos intuitivos, como `/login`, `/join`, `/chat`, etc.
* **Salas de Chat:** Crie salas de chat públicas ou privadas com senha.
* **Conversas Privadas:** Inicie conversas um-a-um com outros usuários.
* **Atualização de Perfil:** Modifique seu nome de usuário e senha com o comando `/profile`.

---

## 🛠️ Tecnologias Utilizadas

Este projeto é dividido em duas partes principais: o backend e o frontend.

### **Backend (.NET)**

* **Framework:** ASP.NET Core 8.0
* **Comunicação em Tempo Real:** SignalR para conexões WebSocket.
* **Banco de Dados:** PostgreSQL com Entity Framework Core, conectado via Supabase.
* **Autenticação:** JWT (JSON Web Tokens).
* **Mapeamento de Objetos:** AutoMapper.
* **Linha de Comando:** System.CommandLine para uma robusta análise de comandos.
* **Containerização:** Suporte para Docker.

### **Frontend (Vue.js)**

* **Framework:** Vue.js 3
* **UI Framework:** Quasar
* **Gerenciamento de Estado:** Pinia
* **Cliente HTTP:** Axios
* **Comunicação em Tempo Real:** Cliente SignalR para JavaScript.
* **Internacionalização:** Vue-i18n.

---

## 🚀 Começando

Siga estas instruções para obter uma cópia do projeto em funcionamento na sua máquina local para desenvolvimento e testes.

### **Pré-requisitos**

* [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior.
* [Node.js](https://nodejs.org/) (que inclui o npm).
* Uma conta no [Supabase](https://supabase.com/) para o banco de dados PostgreSQL.

### **Instalação**

#### **1. Configuração do Banco de Dados (Supabase)**

1.  Crie um novo projeto no [Supabase](https://supabase.com/).
2.  No painel do seu projeto, vá para **Project Settings > Database**.
3.  Encontre sua string de conexão (Connection String) e copie-a.
4.  Na pasta `backend`, cole sua string de conexão no campo `"DefaultConnection"` no arquivo `appsettings.Development.json`.

#### **2. Executando o Backend**

1.  Abra um terminal na pasta `backend`.
2.  Restaure as dependências do .NET:
    ```bash
    dotnet restore
    ```
3.  Aplique as migrações do Entity Framework para criar as tabelas no seu banco de dados Supabase:
    ```bash
    dotnet ef database update
    ```
4.  Inicie o servidor backend:
    ```bash
    dotnet run
    ```
    O servidor estará disponível em `http://localhost:5136`.

#### **3. Executando o Frontend**

1.  Em um novo terminal, navegue até a pasta `frontend`.
2.  Crie um arquivo `.env` a partir do exemplo fornecido:
    ```bash
    cp .env.example .env
    ```
    O arquivo `.env` já está configurado para se conectar ao backend local.

3.  Instale as dependências do npm:
    ```bash
    npm install
    ```
4.  Inicie o servidor de desenvolvimento do frontend:
    ```bash
    npm run serve
    ```
    O frontend estará acessível em `http://localhost:8080`.

---

## 🤖 Comandos Disponíveis

Use os seguintes comandos na interface de terminal para interagir com o chat:

| Comando | Descrição | Uso |
| :--- | :--- | :--- |
| **/help** | Mostra todos os comandos disponíveis. | `/help` |
| **/register** | Registra um novo usuário no sistema. | `/register --username "seu-nome" --password "sua-senha"` |
| **/login** | Faz login no sistema. | `/login --username "seu-nome" --password "sua-senha"` |
| **/logout** | Faz logout do sistema. | `/logout` |
| **/chat** | Inicia uma conversa privada com um usuário. | `/chat "nome-do-usuario"` |
| **/create** | Cria uma nova sala de chat. | `/create "nome-da-sala" --description "descrição" --password "senha"` |
| **/join** | Entra em uma sala de chat existente. | `/join "ID-da-sala" --password "senha-da-sala"` |
| **/list** | Lista todos os chats que você tem acesso. | `/list` |
| **/profile** | Atualiza seu nome de usuário ou senha. | `/profile --username "novo-nome" --password "nova-senha" --confirm-password "nova-senha"` |