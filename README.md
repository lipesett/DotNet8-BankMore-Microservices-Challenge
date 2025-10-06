# Microsserviços BankMore - Portfólio .NET 8

![.NET](https://img.shields.io/badge/.NET-8-blueviolet) ![C#](https://img.shields.io/badge/C%23-11-blue) ![Microservices](https://img.shields.io/badge/Architecture-Microservices-informational) ![CQRS](https://img.shields.io/badge/Pattern-CQRS-orange) ![Dapper](https://img.shields.io/badge/Data-Dapper-brightgreen) ![JWT](https://img.shields.io/badge/Security-JWT-red)

Este repositório é um projeto de portfólio que implementa uma plataforma de banco digital baseada em microsserviços, conforme especificado em um desafio técnico. A solução demonstra uma abordagem profissional para a construção de um sistema distribuído usando .NET 8, com foco em Arquitetura Limpa, Domain-Driven Design (DDD), CQRS e práticas robustas de segurança e gerenciamento de dados.

## 🚀 Principais Conceitos e Tecnologias Aplicadas

Este projeto serve como uma demonstração prática dos seguintes conceitos e tecnologias:

* **Arquitetura de Microsserviços:** O sistema é dividido em serviços independentes e desacoplados (`ContaCorrente.Api`, `Transferencia.Api`).
* **Domain-Driven Design (DDD):** Cada serviço é estruturado com camadas distintas (`Domain`, `Application`, `Infrastructure`) para focar no domínio de negócio.
* **CQRS com MediatR:** O padrão CQRS é utilizado para separar operações de escrita (Commands) de operações de leitura (Queries), orquestrado pela biblioteca MediatR.
* **Comunicação Síncrona entre Serviços:** O `Transferencia.Api` se comunica com o `ContaCorrente.Api` através de requisições HTTP gerenciadas pelo `IHttpClientFactory`.
* **Banco de Dados por Serviço:** Cada microsserviço possui e gerencia seu próprio banco de dados SQLite independente, um princípio central do design de microsserviços.
* **Acesso a Dados com Dapper:** O Dapper é utilizado para persistência e consulta de dados de alta performance, conforme exigido pelo desafio.
* **Autenticação JWT:** O sistema é protegido usando JSON Web Tokens. Um endpoint dedicado fornece os tokens após o login, e os endpoints protegidos validam esses tokens.
* **Idempotência:** Endpoints que alteram o estado (como a movimentação de conta) são projetados para serem idempotentes, prevenindo operações duplicadas de requisições repetidas através de uma tabela de chaves de idempotência dedicada.
* **EF Core para Migrations:** Enquanto o Dapper lida com as operações de dados, as poderosas ferramentas de migração do Entity Framework Core são usadas para gerenciar e versionar o esquema do banco de dados.
* **Documentação com Swagger/OpenAPI:** Cada API é documentada com Swagger, incluindo configurações para suportar autenticação JWT para facilitar os testes.

## 📋 Endpoints da API

### `ContaCorrente.Api`
| Verbo | Rota | Descrição | Segurança |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Contas` | Cadastra uma nova conta corrente. | **Público** |
| `POST` | `/api/Contas/login`| Autentica um usuário e retorna um JWT. | **Público** |
| `DELETE`| `/api/Contas` | Inativa a conta do usuário autenticado. | **Protegido** |
| `POST` | `/api/Contas/movimentacao`| Realiza um crédito ou débito em uma conta. | **Protegido** |
| `GET` | `/api/Contas/saldo` | Retorna o saldo atual do usuário autenticado. | **Protegido** |
| `GET` | `/api/Contas/por-numero/{numeroConta}` | (Interno) Obtém detalhes da conta pelo número. | **Protegido** |

### `Transferencia.Api`
| Verbo | Rota | Descrição | Segurança |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Transferencias` | Efetua uma transferência entre duas contas. | **Protegido** |

## ⚙️ Como Executar o Projeto

Esta é uma solução com múltiplos projetos. Você deve executar as duas APIs simultaneamente.

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
* Git
* A ferramenta global `dotnet-ef`. Se não estiver instalada, execute: `dotnet tool install --global dotnet-ef`

### Passos

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/SEU-USUARIO/.Net8-BankMore-Microservices-Portfolio.git](https://github.com/SEU-USUARIO/.Net8-BankMore-Microservices-Portfolio.git)
    ```
    (Lembre-se de substituir `SEU-USUARIO` pelo seu nome de usuário do GitHub)

2.  **Navegue até a pasta da solução:**
    ```bash
    cd .Net8-BankMore-Microservices-Portfolio
    ```

3.  **Prepare os Bancos de Dados (Apenas na primeira vez):**
    Você precisa criar o banco de dados para cada serviço. Execute estes comandos a partir da **pasta raiz da solução**.
    ```bash
    # Cria o banco de dados para o ContaCorrente.Api
    dotnet ef database update --project ContaCorrente.Api

    # Cria o banco de dados para o Transferencia.Api
    dotnet ef database update --project Transferencia.Api
    ```

4.  **Execute as aplicações:**
    Você pode executar os dois serviços simultaneamente de duas formas:

    * **Via Visual Studio (Recomendado):**
        1.  Clique com o botão direito na Solução > **Set Startup Projects...**.
        2.  Selecione **Multiple startup projects**.
        3.  Para `ContaCorrente.Api` e `Transferencia.Api`, mude a "Action" para **Start**.
        4.  Pressione **F5** para iniciar.

    * **Via .NET CLI:**
        Abra dois terminais separados na pasta raiz da solução.
        * **No Terminal 1:**
            ```bash
            dotnet run --project ContaCorrente.Api
            ```
        * **No Terminal 2:**
            ```bash
            dotnet run --project Transferencia.Api
            ```

5.  **Acesse a documentação do Swagger:**
    As APIs estarão rodando em portas diferentes. Verifique a saída do console para obter as URLs corretas (ex: `https://localhost:7171` e `https://localhost:7010`). Acesse `/swagger` em cada URL para testar a respectiva API.

## 🔜 Próximos Passos (Pendentes)
Este projeto está em desenvolvimento. As próximas etapas planejadas são:
* Implementar a lógica de idempotência no `Transferencia.Api`.
* Implementar a comunicação assíncrona com **Kafka** para a funcionalidade de Tarifas.
* Criar `Dockerfiles` e um arquivo `docker-compose.yaml` para orquestrar a solução.
* Adicionar uma suíte de **testes de integração** para garantir a qualidade e o comportamento esperado.

---
---

# BankMore Microservices - .NET 8 Portfolio (English)

![.NET](https://img.shields.io/badge/.NET-8-blueviolet) ![C#](https://img.shields.io/badge/C%23-11-blue) ![Microservices](https://img.shields.io/badge/Architecture-Microservices-informational) ![CQRS](https://img.shields.io/badge/Pattern-CQRS-orange) ![Dapper](https://img.shields.io/badge/Data-Dapper-brightgreen) ![JWT](https://img.shields.io/badge/Security-JWT-red)

This repository is a portfolio project that implements a microservices-based digital banking platform as specified in a technical challenge. The solution demonstrates a professional approach to building a distributed system using .NET 8, focusing on Clean Architecture, Domain-Driven Design (DDD), CQRS, and robust security and data management practices.

## 🚀 Key Concepts & Technologies Applied

This project serves as a practical demonstration of the following concepts and technologies:

* **Microservice Architecture:** The system is divided into independent, decoupled services (`ContaCorrente.Api`, `Transferencia.Api`).
* **Domain-Driven Design (DDD):** Each service is structured with distinct layers (`Domain`, `Application`, `Infrastructure`) to focus on the business domain.
* **CQRS with MediatR:** The CQRS pattern is used to separate write operations (Commands) from read operations (Queries), orchestrated by the MediatR library.
* **Synchronous Service-to-Service Communication:** The `Transferencia.Api` communicates with `ContaCorrente.Api` via HTTP requests managed by `IHttpClientFactory`.
* **Database per Service:** Each microservice owns and manages its own independent SQLite database, a core tenet of microservice design.
* **Data Access with Dapper:** Dapper is used for high-performance data persistence and querying, as required by the challenge.
* **JWT Authentication:** The system is secured using JSON Web Tokens. A dedicated endpoint provides tokens upon successful login, and protected endpoints validate these tokens.
* **Idempotency:** State-changing endpoints (like account movement) are designed to be idempotent, preventing duplicate operations from repeated requests by using a dedicated idempotency key table.
* **EF Core for Migrations:** While Dapper handles data operations, Entity Framework Core's powerful migration tooling is used to manage and version the database schema.
* **Swagger/OpenAPI Documentation:** Each API is documented with Swagger, including configurations to support JWT authentication for easy testing.

## 📋 API Endpoints

### `ContaCorrente.Api`
| Verb | Route | Description | Security |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Contas` | Registers a new current account. | **Public** |
| `POST` | `/api/Contas/login`| Authenticates a user and returns a JWT. | **Public** |
| `DELETE`| `/api/Contas` | Inactivates the authenticated user's account. | **Protected** |
| `POST` | `/api/Contas/movimentacao`| Performs a credit or debit on an account. | **Protected** |
| `GET` | `/api/Contas/saldo` | Returns the current balance of the authenticated user. | **Protected** |
| `GET` | `/api/Contas/por-numero/{numeroConta}` | (Internal) Gets account details by account number. | **Protected** |

### `Transferencia.Api`
| Verb | Route | Description | Security |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Transferencias` | Performs a transfer between two accounts. | **Protected** |

## ⚙️ How to Run the Project

This is a multi-project solution. You must run both APIs simultaneously.

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Git
* The `dotnet-ef` global tool. If not installed, run: `dotnet tool install --global dotnet-ef`

### Steps

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/SEU-USUARIO/.Net8-BankMore-Microservices-Portfolio.git](https://github.com/SEU-USUARIO/.Net8-BankMore-Microservices-Portfolio.git)
    ```
    (Remember to replace `SEU-USUARIO` with your GitHub username)

2.  **Navigate to the solution folder:**
    ```bash
    cd .Net8-BankMore-Microservices-Portfolio
    ```

3.  **Prepare the Databases (First time only):**
    You need to create the database for each service. Run these commands from the **solution root folder**.
    ```bash
    # Create the database for ContaCorrente.Api
    dotnet ef database update --project ContaCorrente.Api

    # Create the database for Transferencia.Api
    dotnet ef database update --project Transferencia.Api
    ```

4.  **Run the applications:**
    You can run both services simultaneously in two ways:

    * **Via Visual Studio (Recommended):**
        1.  Right-click the Solution > **Set Startup Projects...**.
        2.  Select **Multiple startup projects**.
        3.  For `ContaCorrente.Api` and `Transferencia.Api`, change the "Action" to **Start**.
        4.  Press **F5** to launch.

    * **Via .NET CLI:**
        Open two separate terminals in the solution root folder.
        * **In Terminal 1:**
            ```bash
            dotnet run --project ContaCorrente.Api
            ```
        * **In Terminal 2:**
            ```bash
            dotnet run --project Transferencia.Api
            ```

5.  **Access the Swagger documentation:**
    The APIs will be running on different ports. Check the console output for the correct URLs (e.g., `https://localhost:7171` and `https://localhost:7010`). Access `/swagger` on each URL to test the respective API.

## 🔜 Next Steps (Pending)
This project is under development. The next planned steps are:
* Implement the idempotency logic in the `Transferencia.Api`.
* Implement asynchronous communication with **Kafka** for the Tariffs feature.
* Create `Dockerfiles` and a `docker-compose.yaml` file to orchestrate the solution.
* Add an **integration test** suite to ensure quality and expected behavior.
