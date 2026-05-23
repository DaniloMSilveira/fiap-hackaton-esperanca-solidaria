# 🚀 Guia Completo — Infraestrutura, Observabilidade e Deploy no Kind

Este guia descreve como subir toda a infraestrutura do projeto **EsperancaSolidaria** em um cluster local utilizando **Kind**, incluindo:

- SQL Server
- MongoDB
- RabbitMQ
- Prometheus
- Grafana
- Loki
- Tempo
- API .NET
- Worker de processamento assíncrono

---

### Pré-requisitos

Certifique-se de possuir instalado:

- Docker
- Kind
- kubectl
- Helm
- .NET SDK 10

---

### Estrutura do Ambiente

| Namespace | Responsabilidade |
|---|---|
| `monitoring` | Observabilidade (Prometheus, Grafana, Loki, Tempo) |
| `app` | Aplicação e infraestrutura de negócio |

---

## 🚀 1. Criando o Cluster Kind

Acesse a pasta de infraestrutura:

```bash
cd infra-as-code/kind
```

Crie o cluster:

```bash
kind create cluster --name fiap --config cluster-config.yaml
```

---

## 📊 2. Stack de Observabilidade

A stack de monitoramento deve ser criada antes da aplicação.

---

### Criar Namespace de Monitoramento

```bash
kubectl create namespace monitoring
```

---

### Adicionar Repositórios Helm

```bash
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts

helm repo add grafana https://grafana.github.io/helm-charts

helm repo update
```

---

### Instalar Prometheus + Grafana

```bash
helm install prometheus-grafana-stack prometheus-community/kube-prometheus-stack \
  --version 85.2.0 \
  -f helm-values/values-prometheus.yaml \
  -n monitoring
```

---

### Obter Senha do Grafana

```bash
kubectl get secret prometheus-grafana-stack -n monitoring \
  -o jsonpath="{.data.admin-password}" | base64 --decode
```

---

### Acessar Serviços

| Serviço | URL |
|---|---|
| Grafana | http://localhost:3000 |
| Prometheus | http://localhost:9090 |

---

## 📜 3. Instalar Loki (Logs)

---

### Instalar Loki

```bash
helm install loki grafana/loki \
  --version 6.29.0 \
  -f helm-values/values-loki.yaml \
  -n monitoring
```

---

### Expor Loki via NodePort

```bash
kubectl patch svc loki -n monitoring \
  -p '{"spec":{"type":"NodePort","ports":[{"name":"http-metrics","port":3100,"protocol":"TCP","targetPort":3100,"nodePort":30007}]}}'
```

---

### Validar Loki

```bash
kubectl get svc -n monitoring
```

Loki deve aparecer com:

```text
3100:30007/TCP
```

---

## 🔍 4. Instalar Tempo (Tracing)

---

### Instalar Tempo

```bash
helm install tempo grafana/tempo \
  --version 1.23.0 \
  -f helm-values/values-tempo.yaml \
  -n monitoring
```

---

### Expor Tempo via NodePort

```bash
kubectl patch svc tempo -n monitoring \
  -p '{"spec":{"type":"NodePort","ports":[{"name":"http-tempo","port":3200,"protocol":"TCP","targetPort":3100,"nodePort":30008}]}}'
```

---

### Validar Tempo

```bash
kubectl get svc -n monitoring
```

Tempo deve aparecer com:

```text
3200:30008/TCP
```

---

## 🏗️ 5. Build das Imagens Docker

Volte para a raiz do projeto:

```bash
cd ../../
```

---

### Build da API

```bash
docker build \
  -f ./src/web/EsperancaSolidaria.API/Dockerfile \
  -t esperanca-solidaria-api:1.0 \
  ./src
```

---

### Build do Worker

```bash
docker build \
  -f ./src/workers/EsperancaSolidaria.Worker.DoacaoRealizada/Dockerfile \
  -t esperanca-solidaria-worker-doacao-realizada:1.0 \
  ./src
```

---

### Carregar Imagens no Kind

```bash
kind load docker-image esperanca-solidaria-api:1.0 --name fiap

kind load docker-image esperanca-solidaria-worker-doacao-realizada:1.0 --name fiap
```

---

## 🗄️ 6. Subir Recursos de Infraestrutura

---

### Criar Namespace da Aplicação

```bash
kubectl create namespace app
```

---

### SQL Server

```bash
kubectl apply -f deployment-sqlserver.yaml -n app
```

---

### MongoDB

```bash
kubectl apply -f deployment-mongodb.yaml -n app
```

---

### RabbitMQ

```bash
kubectl apply -f deployment-rabbitmq.yaml -n app
```

---

### Validar Recursos

```bash
kubectl get pods -n app
```

Aguarde todos os pods ficarem com status:

```text
Running
```

---

## 🔐 7. Criar Secrets

> ⚠️ Atualize previamente os valores Base64 dos arquivos YAML.

```bash
kubectl apply -f secret-api.yaml -n app

kubectl apply -f secret-worker.yaml -n app
```

---

## 📡 8. Configurar Monitoramento da API

---

### Registrar ServiceMonitor

```bash
kubectl apply -f service-monitor-prometheus-api.yaml -n monitoring
```

---

### Validar Targets no Prometheus

Acesse:

```text
http://localhost:9090
```

Menu:

```text
Status → Targets
```

Verifique se o target da API está:

```text
UP
```

---

## 🚀 9. Deploy da Aplicação

---

### Subir Worker

```bash
kubectl apply -f deployment-worker.yaml -n app
```

---

### Subir API

```bash
kubectl apply -f deployment-api.yaml -n app
```

---

### Reiniciar API (Opcional)

Caso tenha alterado configurações de observabilidade:

```bash
kubectl rollout restart deployment esperanca-solidaria-api -n app
```

---

## ✅ 10. Validar Aplicação

---

### Verificar Pods

```bash
kubectl get pods -n app
```

---

### Verificar Serviços

```bash
kubectl get svc -n app
```

---

### Swagger

Acesse:

```text
http://localhost:8080/swagger
```

---

## 📊 11. Validar Observabilidade

---

### Prometheus

Consultar métricas:

```text
up
```

```text
http_server_duration_seconds_count
```

```text
aspnetcore_requests_total
```

---

### Loki (Logs)

No Grafana:

```text
Explore → Loki
```

Exemplo de query:

```text
{service_name="esperanca-solidaria-api"}
```

---

### Tempo (Tracing)

No Grafana:

```text
Explore → Tempo
```

As traces da API serão exibidas automaticamente após requisições HTTP.

---

## Fluxo Completo de Observabilidade

```text
API (.NET)
   │
   ├── Métricas → Prometheus
   │
   ├── Logs → Loki
   │
   └── Traces → Tempo
            │
            ▼
         Grafana
```

---

## Arquitetura da Solução

```text
Usuário
   │
   ▼
API REST (.NET)
   │
   ├── SQL Server
   ├── MongoDB
   ├── RabbitMQ
   │
   ├── OpenTelemetry Metrics → Prometheus
   ├── Serilog Logs → Loki
   └── OpenTelemetry Traces → Tempo
                │
                ▼
             Grafana
```
