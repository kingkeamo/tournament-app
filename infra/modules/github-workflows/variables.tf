variable "repository" {
  description = "GitHub repository name (e.g., 'username/tournament-app')"
  type        = string
}

variable "github_token" {
  description = "GitHub token for API access"
  type        = string
  sensitive   = true
}

variable "fly_app_name" {
  description = "Fly.io app name (from Terraform outputs)"
  type        = string
}

variable "fly_app_url" {
  description = "Fly.io app URL (from Terraform outputs)"
  type        = string
}

variable "cognito_user_pool_id" {
  description = "Cognito User Pool ID (from Terraform outputs)"
  type        = string
}

variable "cognito_client_id" {
  description = "Cognito Client ID (from Terraform outputs)"
  type        = string
}

variable "cognito_authority" {
  description = "Cognito Authority (from Terraform outputs)"
  type        = string
}

variable "cognito_domain" {
  description = "Cognito Domain (from Terraform outputs)"
  type        = string
}

variable "github_pages_url" {
  description = "GitHub Pages URL (from Terraform outputs)"
  type        = string
}

variable "aws_region" {
  description = "AWS region for infrastructure"
  type        = string
  default     = "eu-west-2"
}

