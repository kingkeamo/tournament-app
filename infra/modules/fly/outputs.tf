output "app_name" {
  description = "Fly.io application name"
  value       = "${var.project_name}-${var.environment}"
}

output "app_url" {
  description = "Fly.io application URL"
  value       = "https://${var.project_name}-${var.environment}.fly.dev"
}

