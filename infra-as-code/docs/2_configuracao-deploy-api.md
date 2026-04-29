# 🚀 Guia Completo — API + SQL Server + Observabilidade no Kind

Este guia descreve como subir uma API .NET com SQL Server em um cluster local usando **Kind**, além de configurar **Prometheus + Grafana** para observabilidade.

---

# 📦 Pré-requisitos

* Docker instalado
* Kind instalado
* kubectl configurado
* Helm instalado

---

# 🧱 Estrutura do Ambiente

* Namespace da aplicação: `app`
* Namespace de monitoramento: `monitoring`

---

## 🚀 Criando um Cluster com Kind

```bash
cd infra-as-code/kind
kind create cluster --name app --config cluster-config.yaml
```

---

# 🚀 1. Build e Deploy da API

## 🔨 1.1 Build da imagem Docker

Na raiz do projeto:

```bash
docker build -f ./src/web/EsperancaSolidaria.API/Dockerfile -t esperanca-solidaria-api:1.0 ./src
docker build -f ./src/workers/EsperancaSolidaria.Worker.DoacaoRealizada/Dockerfile -t esperanca-solidaria-worker-doacao-realizada:1.0 ./src
```

---

## 📦 1.2 Carregar imagem no Kind

```bash
kind load docker-image esperanca-solidaria-api:1.0 --name fiap
kind load docker-image esperanca-solidaria-worker-doacao-realizada:1.0 --name fiap
```

---

## 📁 1.3 Criar namespace da aplicação

```bash
kubectl create namespace app
```

---

## 🗄️ 1.4 Subir SQL Server

```bash
kubectl apply -f deployment-sqlserver.yaml -n app
```

---

## 🗄️ 1.5 Subir RabbitMQ

```bash
kubectl apply -f deployment-rabbitmq.yaml -n app
```

---

## 🔐 1.6 Criar Secrets

> ⚠️ Atualize os valores em Base64 antes de aplicar

```bash
kubectl apply -f secret-api.yaml -n app
kubectl apply -f secret-worker.yaml -n app
```

---

## 🌐 1.7 Subir API e Worker

> Primeiro o worker para inicializar as filas, e depois a API

```bash
kubectl apply -f deployment-worker.yaml -n app
kubectl apply -f deployment-api.yaml -n app
```

---

## ✅ 1.8 Validar aplicação

```bash
kubectl get pods -n app
kubectl get svc -n app
```

Acesse no browser:

```
http://localhost:8080
```

---

# 📊 2. Observabilidade (Prometheus + Grafana)

---

## 📦 2.1 Adicionar repositório Helm

```bash
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
```

---

## 📁 2.2 Criar namespace de monitoramento

```bash
kubectl create namespace monitoring
```

---

## 🚀 2.3 Instalar kube-prometheus-stack

```bash
helm install prometheus-grafana-stack prometheus-community/kube-prometheus-stack \
  -f helm-prometheus-stack/values.yaml \
  -n monitoring
```

---

## 🔐 2.4 Obter senha do Grafana

```bash
kubectl get secret prometheus-grafana-stack -n monitoring \
  -o jsonpath="{.data.admin-password}" | base64 --decode
```

---

## 🔌 2.5 Integrar API com Prometheus

```bash
kubectl apply -f service-monitor-prometheus-api.yaml -n monitoring
```

---

## 🌐 2.6 Acessar serviços

### 📊 Grafana

```
http://localhost:3000
```

### 📈 Prometheus

```
http://localhost:9090
```

---

---

# 🧪 3. Testes

---

## ✅ 3.1 Testar API

```bash
curl http://localhost:8080
```

---

## ✅ 3.2 Testar métricas da API

```bash
curl http://localhost:8081/metrics
```

Deve retornar métricas no formato Prometheus.

---

## ✅ 3.3 Validar no Prometheus

Acesse:

```
http://localhost:9090
```

Vá em:

```
Status → Targets
```

Verifique se:

```
solidarity-connection-api-monitor → UP
```

---

## ✅ 3.4 Testar queries

No Prometheus:

```
up
aspnetcore_requests_total
http_server_duration_seconds_count
```

---

## ✅ 3.5 Validar no Grafana

1. Acesse Grafana
2. Vá em **Dashboards**
3. Utilize dashboards de Kubernetes ou importe customizados
4. Ajuste variáveis como:

   * namespace = `app`

---

# 🧠 4. Arquitetura

```
API (.NET) → expõe /metrics
        ↓
Service (Kubernetes)
        ↓
ServiceMonitor
        ↓
Prometheus
        ↓
Grafana
```
