# Notifications API - Fase 4

Worker .NET 8 responsavel por consumir mensagens de notificacao de email no RabbitMQ dentro do ecossistema Fase 4. O projeto roda localmente via .NET ou Docker Compose e possui manifests para Kubernetes local e Amazon EKS.

## Visao geral

A Notifications API e um worker, nao uma Web API HTTP. Sua responsabilidade principal e:

- Consumir mensagens da fila `notification-queue`.
- Ler eventos publicados no exchange RabbitMQ `fiap.events`.
- Processar mensagens no contrato `EmailNotificationEvent`.
- Registrar o envio de email usando a implementacao atual `ConsoleEmailSender`.
- Gravar logs em console e em arquivos rotativos.

## Arquitetura

- .NET 8 Worker Service.
- MassTransit com RabbitMQ.
- Contratos compartilhados em `src/Shared.Contracts`.
- Infraestrutura de consumo e envio em `src/NotificationsAPI.Infrastructure`.
- Testes em `src/NotificationsAPI.Infrastructure.Tests`.
- Dockerfile e Docker Compose em `src`.
- Kubernetes namespace `fase4`.
- Imagem Docker `adinteltidev/fase4-notifications-api:latest`.

## Contrato da mensagem

O worker consome mensagens usando o contrato `EmailNotificationEvent`:

```csharp
public record EmailNotificationEvent
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Recipient { get; init; } = string.Empty;
    public string? Sender { get; init; }
}
```

## Configuracoes principais

Variaveis esperadas pelo worker:

- `RabbitMq__Host`
- `RabbitMq__Port`
- `RabbitMq__Username`
- `RabbitMq__Password`
- `RabbitMq__VirtualHost`
- `RabbitMq__ExchangeName`
- `RabbitMq__NotificationQueueName`
- `MT_LICENSE`

Valores padrao usados no projeto:

- RabbitMQ host local: `localhost`
- RabbitMQ porta AMQP: `5672`
- RabbitMQ Management: `15672`
- Virtual host: `fiap`
- Exchange: `fiap.events`
- Fila/routing key: `notification-queue`

## Execucao local com .NET

```powershell
dotnet restore NotificationsAPI.sln
dotnet run --project src/NotificationsAPI/NotificationsAPI.Worker.csproj
```

Para rodar desse modo, o RabbitMQ precisa estar disponivel e as variaveis `RabbitMq__*` devem apontar para ele.

## Execucao local com Docker Compose

```powershell
cd src
docker compose up --build
```

Servicos locais:

- Notifications worker: container `fiap-notifications-worker`
- RabbitMQ AMQP: `localhost:5672`
- RabbitMQ Management: `http://localhost:15672`

O Compose usa recursos compartilhados:

| Recurso | Nome |
| --- | --- |
| Compose project | `fase4-notificationsapi` |
| Container RabbitMQ | `fiap-rabbitmq` |
| Network | `fiap-ms-network` |
| Volume | `fiap-rabbitmq-data` |
| Virtual host | `fiap` |
| Exchange | `fiap.events` |
| Queue/routing key | `notification-queue` |

## Execucao local com Kubernetes

```powershell
$env:RABBITMQ_USERNAME="admin"
$env:RABBITMQ_PASSWORD="admin123"
$env:RABBITMQ_VHOST="fiap"
.\deployLocal.ps1
```

O script aplica:

- `k8s/namespace.yml`
- `k8s/rabbitmq-service.yml`
- `k8s/rabbitmq-deployment.yml`
- `k8s/notifications-configmap.yml`
- `k8s/notifications-worker-deployment.yml`

Para verificar:

```powershell
kubectl rollout status deployment/notifications-worker -n fase4
kubectl get pods -n fase4
```

## Deploy no EKS

```powershell
$env:RABBITMQ_USERNAME="..."
$env:RABBITMQ_PASSWORD="..."
$env:RABBITMQ_VHOST="fiap"
.\deployEks.ps1 -ClusterName Fcg-Fase4 -Region us-east-1
```

O script conecta no cluster informado, cria/atualiza o secret `rabbitmq-secrets` e aplica os manifests Kubernetes no namespace `fase4`.

## Testes

```powershell
dotnet test NotificationsAPI.sln
```

## Stack

- .NET 8
- Worker Service
- MassTransit
- RabbitMQ
- Serilog
- Docker
- Kubernetes
- Amazon EKS
- xUnit
- Moq
