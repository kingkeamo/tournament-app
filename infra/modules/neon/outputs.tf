output "connection_string" {
  description = "PostgreSQL connection string"
  value       = "postgresql://${neon_project.main.default_role}:${neon_project.main.default_role_password}@${neon_endpoint.main.host}/${neon_database.main.name}?sslmode=require"
  sensitive   = true
}

output "host" {
  description = "PostgreSQL host"
  value       = neon_endpoint.main.host
}

output "database_name" {
  description = "Database name"
  value       = neon_database.main.name
}

output "user" {
  description = "Database user"
  value       = neon_project.main.default_role
}

output "password" {
  description = "Database password"
  value       = neon_project.main.default_role_password
  sensitive   = true
}

