
# 🚀 Guia de Instalação: Kubernetes com Kind no Ubuntu

Este guia cobre a instalação dos principais recursos necessários para rodar um cluster Kubernetes local com Kind em uma distro Ubuntu limpa.

## 📦 Pré-requisitos

- Ubuntu atualizado (20.04+ recomendado)
- Usuário com permissão `sudo`
- Acesso à internet

## 🧱 Atualizando o Sistema

```bash
sudo apt update && sudo apt upgrade -y
```

## 🐳 Instalando Docker

### 1. Instalar Dependências

```bash
sudo apt install -y apt-transport-https ca-certificates curl software-properties-common
```

### 2. Adicionar Chave GPG

```bash
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
```

### 3. Adicionar Repositório

```bash
sudo add-apt-repository \
   "deb [arch=amd64] https://download.docker.com/linux/ubuntu \
   $(lsb_release -cs) stable"
```

### 4. Instalar Docker

```bash
sudo apt update
sudo apt install -y docker-ce
```

### 5. Habilitar Docker sem sudo

```bash
sudo usermod -aG docker $USER
newgrp docker
```

### 6. Testar

```bash
docker run hello-world
```

## 🧩 Instalando Docker Compose

```bash
sudo apt install -y docker-compose
```

### Verificar Instalação

```bash
docker-compose --version
```

## ☸️ Instalando kubectl

### 1. Baixar Binário

```bash
curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
```

### 2. Tornar Executável

```bash
sudo chmod +x kubectl
```

### 3. Mover para PATH

```bash
sudo mv kubectl /usr/local/bin/
```

### 4. Verificar

```bash
kubectl version --client
```

## ⎈ Instalando Helm

### 1. Baixar Script Oficial

```bash
curl https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 | bash
```

### 2. Verificar

```bash
helm version
```

## 🧪 Instalando Kind

### 1. Baixar Binário

```bash
curl -Lo ./kind https://kind.sigs.k8s.io/dl/latest/kind-linux-amd64
```

### 2. Tornar Executável

```bash
sudo chmod +x kind
```

### 3. Mover para PATH

```bash
sudo mv kind /usr/local/bin/
```

### 4. Verificar

```bash
kind version
```


## 🚀 Criando um Cluster com Kind

```bash
cd infra-as-code/kind
kind create cluster --name fiap --config cluster-config.yaml
```

### Verificar Cluster

```bash
kubectl cluster-info --context kind-fiap
kubectl get nodes
```

## 🧹 Deletando Cluster

```bash
kind delete cluster --name fiap
```

