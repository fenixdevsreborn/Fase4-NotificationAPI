# NotificationsAPI

API de notificação responsável por consumir eventos de outros microsserviços (como pagamentos aprovados e criação de usuário) e disparar notificações aos usuários conforme regras de negócio definidas na arquitetura de microsserviços. ([github.com](https://github.com/thefenixdevs/Fase2-NotificationsAPI/tree/Development))

---

## Índice

1. Visão Geral
2. Responsabilidades no Sistema
3. Arquitetura e Tecnologias
4. Fluxos de Eventos
5. Endpoints da API
6. Variáveis de Ambiente
7. Execução

   * Local
   * Docker
   * Docker Compose
   * Kubernetes
8. Observações de Qualidade para Avaliação

---

## 1. Visão Geral

O **NotificationsAPI** é um microsserviço voltado ao processamento de eventos de domínio relacionados a notificações que impactam o usuário final, tais como:

* Envio de e-mails ou mensagens quando um **pagamento é aprovado**.
* Notificações de **boas-vindas** após criação de usuário.
* Outros eventos relevantes à experiência de usuário definidos na arquitetura.

Esse serviço é essencial para garantir que os eventos publicados por outros microsserviços (como **PaymentsAPI** e **UsersAPI**) resultem em ações concretas de notificação. A integração é feita por meio de mensageria assíncrona (RabbitMQ/MassTransit). ([github.com](https://github.com/thefenixdevs/Fase2-NotificationsAPI/tree/Development))

---

## 2. Responsabilidades no Sistema

| Serviço                   | Responsabilidade                                                                                  |
| ------------------------- | ------------------------------------------------------------------------------------------------- |
| **NotificationsAPI**      | Consumir eventos relacionados a notificações e disparar notificações ao usuário (e-mail/SMS/etc). |
| **PaymentsAPI**           | Publicar eventos de pagamento que podem gerar notificações (ex.: pagamento aprovado).             |
| **UsersAPI**              | Publicar eventos de criação de usuário para notificações de boas-vindas.                          |
| **CatalogAPI**            | Publicar eventos que podem requerer notificações (quando aplicável).                              |
| **AuthService (externo)** | Fornecer contexto de autenticação/autorização se endpoints diretos forem expostos.                |

---

## 3. Arquitetura e Tecnologias

**Plataforma e linguagem de desenvolvimento:**

* .NET 10 (ou versão mínima compatível com o restante do projeto)
* C#

**Principais ferramentas e padrões:**

* **MassTransit** para abstração de mensageria
* **RabbitMQ** como broker de mensagens
* **Docker** e **Kubernetes** para containerização e orquestração
* Health checks para monitoramento de dependências
* (Opcional) integração com provedores de e-mail/SMS (via provedores ou mocks)

**Estrutura típica do repositório:**

```
Fase2-NotificationsAPI
├── src
│   ├── NotificationsApi
│   │   ├── Controllers
│   │   ├── Application
│   │   ├── Domain
│   │   ├── Infrastructure
│   │   └── Program.cs
├── Dockerfile
├── docker-compose.yml
├── k8s
│   ├── deployment.yaml
│   ├── service.yaml
│   ├── configmap.yaml
│   └── secret.yaml
└── README.md
```

---

## 4. Fluxos de Eventos

A comunicação assíncrona é o ponto central da NotificationsAPI. Os fluxos principais são:

### 4.1. Usuário Criado

1. **Evento publicado:** outro serviço (UsersAPI) publica o evento `UserCreatedEvent`.
2. **Consumo:** NotificationsAPI consome o evento via fila de mensagens.
3. **Ação:** dispara notificação de boas-vindas por e-mail/SMS ao usuário.

### 4.2. Pagamento Aprovado

1. **Evento publicado:** PaymentsAPI publica o evento `PaymentProcessedEvent` com status aprovado.
2. **Consumo:** NotificationsAPI consome o evento.
3. **Ação:** dispara notificação confirmando a compra.

### 4.3. Outros Eventos

O padrão de design permite consumir outros eventos que demandem ações de notificação. A configuração de consumers e filas deve refletir essa extensibilidade.

---

## 5. Endpoints da API

Dependendo da necessidade de expor uma API REST, os endpoints podem incluir:

| Verbo | Endpoint                  | Autenticação | Descrição                                                  |
| ----- | ------------------------- | ------------ | ---------------------------------------------------------- |
| GET   | `/health`                 | Não          | Health check do serviço (broker, dependências).            |
| GET   | `/api/notifications`      | Sim*         | Consultar histórico de notificações (quando implementado). |
| POST  | `/api/notifications/send` | Sim*         | Acionar envio manual de notificação (se aplicável).        |

* Ajustar conforme os controllers efetivamente no código do projeto (Commons/Controllers). Se a NotificationsAPI for puramente orientada a eventos, os endpoints REST podem ser mínimos.

---

## 6. Variáveis de Ambiente

Configure as variáveis de ambiente dos componentes a seguir para rodar em qualquer ambiente (desenvolvimento, staging ou produção). Use **ConfigMap** para valores não sensíveis e **Secrets** para sensíveis.

### ConfigMap (Não sensíveis)

| Variável                           | Descrição                               |
| ---------------------------------- | --------------------------------------- |
| `RabbitMq__Host`                    | Host do broker RabbitMQ                 |
| `RABBITMQ_EXCHANGE_NOTIFICATIONS`  | Exchange para eventos de notificação    |
| `RABBITMQ_RabbitMq__UserCreatedQueueName`      | Fila de eventos de usuário criado       |
| `RABBITMQ_QUEUE_PAYMENT_PROCESSED` | Fila de eventos de pagamento processado |
| `ASPNETCORE_ENVIRONMENT`           | Ambiente (.NET)                         |

---

### Secrets (Sensíveis)

| Variável                | Descrição                                             |
| ----------------------- | ----------------------------------------------------- |
| `RabbitMq__Username`     | Usuário para autenticação no RabbitMQ                 |
| `RabbitMq__Password`     | Senha para autenticação no RabbitMQ                   |
| `EMAIL_SERVICE_API_KEY` | Chave de API do provedor de e-mail (quando aplicável) |
| `SMS_SERVICE_API_KEY`   | Chave de API do provedor de SMS (quando aplicável)    |

> Em produção, utilize soluções de gerenciamento de secrets (ex.: Vault, Kubernetes Secrets).

---

## 7. Execução

### 7.1 Local (Desenvolvimento)

1. Clone o repositório:

   ```bash
   git clone https://github.com/thefenixdevs/Fase2-NotificationsAPI.git
   ```
2. Selecione a branch `Development` e ajuste as variáveis de ambiente localmente.
3. Inicie dependências (RabbitMQ, brokers de notificação, serviços de e-mail/SMS simulados).
4. Compile e execute com .NET CLI:

   ```bash
   dotnet restore
   dotnet build
   dotnet run --project src/NotificationsApi/NotificationsApi.csproj
   ```

---

### 7.2 Docker

1. Construa a imagem:

   ```bash
   docker build -t notifications-api .
   ```
2. Execute com variáveis de ambiente:

   ```bash
   docker run -e RabbitMq__Host=... -e RabbitMq__Username=... -e RabbitMq__Password=... notifications-api
   ```

---

### 7.3 Docker Compose

Caso exista `docker-compose.yml`:

```bash
docker compose up --build
```

O compose deve orquestrar tanto a NotificationsAPI quanto o RabbitMQ para desenvolvimento local.

---

### 7.4 Kubernetes

Aplique os manifests localizados em `k8s/`:

```bash
kubectl apply -f k8s/
```

Lembre-se de criar **ConfigMaps** e **Secrets** antes de aplicar os deployments.

---

## 8. Observações de Qualidade para Avaliação Acadêmica

Para garantir a documentação atende aos critérios da **Fase 2 da avaliação**, confira:

* Consolidação clara de **eventos consumidos e publicados**.
* Separação entre **ConfigMaps e Secrets** no uso de variáveis de ambiente.
* **Fluxos de eventos** bem definidos com relação ao consumidor.
* **Execução em múltiplos ambientes** (Local, Docker, Kubernetes).
* **Health checks** para mensageria e serviços relacionados.

---