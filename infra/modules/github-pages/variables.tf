variable "repository" {
  description = "GitHub repository name (e.g., 'username/tournament-app')"
  type        = string
}

variable "github_token" {
  description = "GitHub token for API access"
  type        = string
  sensitive   = true
}

