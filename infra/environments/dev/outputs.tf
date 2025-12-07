# Commented out - using existing Neon database
# Uncomment when using Terraform-managed Neon module
# output "database_connection_string" {
#   description = "PostgreSQL connection string"
#   value       = module.neon.connection_string
#   sensitive   = true
# }
#
# output "database_host" {
#   description = "PostgreSQL host"
#   value       = module.neon.host
# }
#
# output "database_name" {
#   description = "PostgreSQL database name"
#   value       = module.neon.database_name
# }

output "cognito_user_pool_id" {
  description = "Cognito User Pool ID"
  value       = module.cognito.user_pool_id
}

output "cognito_client_id" {
  description = "Cognito App Client ID"
  value       = module.cognito.client_id
}

output "cognito_domain" {
  description = "Cognito Hosted UI domain"
  value       = module.cognito.domain
}

output "cognito_authority" {
  description = "Cognito OIDC authority URL"
  value       = module.cognito.authority
}

output "fly_app_name" {
  description = "Fly.io application name"
  value       = module.fly.app_name
}

output "fly_app_url" {
  description = "Fly.io application URL"
  value       = module.fly.app_url
}

output "github_pages_url" {
  description = "GitHub Pages URL"
  value       = module.github_pages.url
}

