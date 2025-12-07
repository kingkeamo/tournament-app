terraform {
  required_version = ">= 1.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
    neon = {
      source  = "kislerdm/neon"
      version = "~> 0.2"
    }
    github = {
      source  = "integrations/github"
      version = "~> 5.0"
    }
  }

  # Backend configuration is provided via backend.tf in environment directories
  # No default backend here - each environment has its own backend.tf
}

provider "aws" {
  region = var.aws_region
}

provider "neon" {
  api_key = var.neon_api_key
}

provider "github" {
  token = var.github_token
}

