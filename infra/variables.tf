variable "aws_region" {
  description = "AWS region for Cognito"
  type        = string
  default     = "us-east-1"
}

variable "neon_api_key" {
  description = "Neon API key for database provisioning"
  type        = string
  sensitive   = true
}

variable "github_token" {
  description = "GitHub token for Pages configuration"
  type        = string
  sensitive   = true
}

variable "github_repository" {
  description = "GitHub repository name (e.g., 'username/tournament-app')"
  type        = string
}

variable "environment" {
  description = "Environment name (dev, prod)"
  type        = string
}

variable "project_name" {
  description = "Project name prefix for resources"
  type        = string
  default     = "tournament-app"
}

variable "cognito_domain_prefix" {
  description = "Prefix for Cognito hosted UI domain"
  type        = string
}

variable "fly_org" {
  description = "Fly.io organization name"
  type        = string
}

variable "fly_api_token" {
  description = "Fly.io API token"
  type        = string
  sensitive   = true
}

variable "allowed_callback_urls" {
  description = "Allowed callback URLs for Cognito (e.g., GitHub Pages URLs)"
  type        = list(string)
  default     = []
}

variable "allowed_logout_urls" {
  description = "Allowed logout URLs for Cognito"
  type        = list(string)
  default     = []
}

