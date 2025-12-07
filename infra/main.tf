# Commented out - using existing Neon database
# Uncomment when you want Terraform to manage a new database
# module "neon" {
#   source = "./modules/neon"
#
#   project_name = var.project_name
#   environment  = var.environment
# }

module "cognito" {
  source = "./modules/cognito"

  project_name          = var.project_name
  environment           = var.environment
  domain_prefix         = var.cognito_domain_prefix
  allowed_callback_urls = var.allowed_callback_urls
  allowed_logout_urls   = var.allowed_logout_urls
}

module "fly" {
  source = "./modules/fly"

  project_name = var.project_name
  environment  = var.environment
  fly_org      = var.fly_org
  fly_api_token = var.fly_api_token
}

module "github_pages" {
  source = "./modules/github-pages"

  repository   = var.github_repository
  github_token = var.github_token
}

