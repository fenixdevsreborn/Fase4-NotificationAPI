param(
  [string]$Namespace = "fase4",
  [string]$Region = "us-east-1",
  [string]$ClusterName = "Fcg-Fase4"
)

$ErrorActionPreference = "Stop"

function Require-Env([string]$Name) {
  $value = [Environment]::GetEnvironmentVariable($Name)
  if ([string]::IsNullOrWhiteSpace($value)) {
    throw "Environment variable '$Name' is required to create Kubernetes secrets."
  }
  return $value
}

function Apply-Secret([string]$Name, [string[]]$Literals) {
  kubectl create secret generic $Name -n $Namespace @Literals --dry-run=client -o yaml | kubectl apply -f -
}

aws eks update-kubeconfig --name $ClusterName --region $Region

Write-Host "Applying shared namespace..." -ForegroundColor Green
kubectl apply -f k8s/namespace.yml

Write-Host "Creating shared RabbitMQ secret from environment variables..." -ForegroundColor Green
Apply-Secret "rabbitmq-secrets" @(
  "--from-literal=rabbitmq-user=$(Require-Env 'RABBITMQ_USERNAME')",
  "--from-literal=rabbitmq-pass=$(Require-Env 'RABBITMQ_PASSWORD')",
  "--from-literal=rabbitmq-vhost=$(Require-Env 'RABBITMQ_VHOST')"
)

Write-Host "Applying shared RabbitMQ and NotificationsAPI worker..." -ForegroundColor Green
kubectl apply -f k8s/rabbitmq-service.yml
kubectl apply -f k8s/rabbitmq-deployment.yml
kubectl apply -f k8s/notifications-configmap.yml
kubectl apply -f k8s/notifications-worker-deployment.yml

Write-Host "Waiting for rollouts..." -ForegroundColor Cyan
kubectl rollout status deployment/rabbitmq -n $Namespace
kubectl rollout status deployment/notifications-worker -n $Namespace

kubectl get pods -n $Namespace -o wide
kubectl get svc -n $Namespace
