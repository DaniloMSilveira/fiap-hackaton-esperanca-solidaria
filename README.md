# EsperancaSolidaria

## Descrição

EsperancaSolidaria é uma solução backend desenvolvida como projeto final do hackathon da pós-graduação em Arquitetura de Sistemas .NET da FIAP. O sistema suporta a ONG Esperança Solidária no gerenciamento de campanhas de arrecadação de fundos, processamento de doações e transparência financeira, implementando uma arquitetura modular com microserviços.

## Arquitetura

O projeto adota um **monolito modular com microserviços**, utilizando padrões de arquitetura limpa, CQRS, Event Sourcing e processamento assíncrono baseado em eventos.

![alt text](image.png)

### Componentes Principais

- **Building Blocks**: Fundamentos compartilhados incluindo CQRS (Commands/Queries), Domain Events, Event Sourcing e Messaging.
- **Core**:
  - **Domain**: Regras de negócio e entidades (Campanha, Doacao, Usuario).
  - **Application**: Handlers CQRS organizados por contexto limitado.
  - **Infrastructure**: Implementações de persistência (EF Core + SQL Server), messaging (RabbitMQ) e event store (MongoDB).
- **Web**: API RESTful com ASP.NET Core, expondo endpoints para autenticação, gerenciamento de campanhas e doações.
- **Workers**: Serviço background para processamento assíncrono de eventos de doação realizada.

### Fluxo de Doação

1. Usuário autenticado cria doação via API.
2. API persiste doação e publica evento `DoacaoRealizadaEvent` no RabbitMQ.
3. Worker consome o evento, atualiza o total da campanha e persiste as mudanças.

## Tecnologias

- **Runtime**: .NET 10.0 (C#)
- **Framework Web**: ASP.NET Core 10.0.5
- **ORM**: Entity Framework Core 10.0.5
- **Bancos de Dados**: SQL Server (dados relacionais), MongoDB (event store)
- **Mensageria**: RabbitMQ
- **Autenticação**: JWT Bearer
- **Documentação**: Swagger/OpenAPI
- **Validação**: FluentValidation
- **Observabilidade**: OpenTelemetry + Prometheus
- **Resiliência**: Polly
- **Containerização**: Docker
- **Orquestração**: Kubernetes (Kind)

## Estrutura do Projeto

```
src/
├── buildingblocks/EsperancaSolidaria.BuildingBlocks/  # CQRS, Events, Messaging
├── core/
│   ├── EsperancaSolidaria.Application/               # Handlers CQRS
│   ├── EsperancaSolidaria.Domain/                    # Entidades, Eventos
│   └── EsperancaSolidaria.Infraestructure/           # Persistência, Messaging
├── web/EsperancaSolidaria.API/                       # API REST
└── workers/EsperancaSolidaria.Worker.DoacaoRealizada/ # Worker de doações

infra-as-code/
├── k8s/                                             # Deployments K8s
└── kind/                                            # Config Kind + Infra

docs/                                                # Documentação técnica
```

## Infraestrutura

### Desenvolvimento Local

Utiliza Docker Compose para subir SQL Server, RabbitMQ e MongoDB.

```bash
docker-compose up -d
```

### Kubernetes Kind

Deploy em cluster Kind com autoscaling (HPA) baseado em CPU/memória.

- **API**: Deployment com health checks e métricas Prometheus.
- **Worker**: Serviço background para processamento de eventos.
- **Infraestrutura**: SQL Server, MongoDB, RabbitMQ, Prometheus + Grafana via Helm.

Para deploy:

```bash
# Criar cluster Kind
kind create cluster --config infra-as-code/kind/cluster-config.yaml

# Aplicar deployments
kubectl apply -f infra-as-code/k8s/
```

## Pipelines CI/CD

### GitHub Actions

- **Pipeline CI** (`pipeline-ci.yaml`): Build, testes, geração de tag semântica e build de imagens Docker.
- **Pipeline CD** (`pipeline-cd.yaml`): Load de imagens no Kind, atualização de manifests e deploy via kubectl.

Executa em runners self-hosted com Kind + kubectl configurado.

## Como Executar

### Local

1. Restaurar dependências: `dotnet restore`
2. Build: `dotnet build`
3. Executar API: `dotnet run --project src/web/EsperancaSolidaria.API`
4. Executar Worker: `dotnet run --project src/workers/EsperancaSolidaria.Worker.DoacaoRealizada`

### Docker

```bash
# API
docker build -f src/web/EsperancaSolidaria.API/Dockerfile -t api .
docker run -p 8080:8080 api

# Worker
docker build -f src/workers/EsperancaSolidaria.Worker.DoacaoRealizada/Dockerfile -t worker .
docker run worker
```

## Segurança e Autorização

- **Autenticação**: JWT tokens.
- **Roles**: GestorONG (admin), Doador (usuário).
- **Validação**: FluentValidation em todos os comandos.
- **Criptografia**: Senhas com BCrypt.

## Observabilidade

- Health checks: `/health/live`, `/health/ready`.
- Métricas Prometheus: Requisições HTTP, tempos de resposta.
- Tracing: OpenTelemetry.

Para mais detalhes, consulte a documentação em `docs/`.