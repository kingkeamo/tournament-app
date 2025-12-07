variable "project_name" {
  description = "Project name prefix"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "domain_prefix" {
  description = "Prefix for Cognito hosted UI domain"
  type        = string
}

variable "allowed_callback_urls" {
  description = "Allowed callback URLs for OAuth"
  type        = list(string)
  default     = []
}

variable "allowed_logout_urls" {
  description = "Allowed logout URLs"
  type        = list(string)
  default     = []
}

