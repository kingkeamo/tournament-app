# Get the repository
data "github_repository" "main" {
  full_name = var.repository
}

# Create .github directory placeholder if it doesn't exist
# This ensures the directory structure exists before creating workflow files
resource "github_repository_file" "github_dir_placeholder" {
  repository          = data.github_repository.main.name
  branch              = "main"
  file                = ".github/.gitkeep"
  content             = "# This file ensures .github directory exists"
  commit_message      = "Create .github directory structure"
  commit_author       = "Terraform"
  commit_email        = "terraform@example.com"
  overwrite_on_create = true
}

# Build and Test workflow (static - doesn't need Terraform outputs)
resource "github_repository_file" "build_test" {
  repository          = data.github_repository.main.name
  branch              = "main"
  file                = ".github/workflows/build-test.yml"
  content             = file("${path.module}/templates/build-test.yml")
  commit_message      = "Update build-test workflow via Terraform"
  commit_author       = "Terraform"
  commit_email        = "terraform@example.com"
  overwrite_on_create = true
  
  depends_on = [github_repository_file.github_dir_placeholder]
}

# Deploy Infrastructure workflow
resource "github_repository_file" "deploy_infra" {
  repository          = data.github_repository.main.name
  branch              = "main"
  file                = ".github/workflows/deploy-infra.yml"
  content             = replace(file("${path.module}/templates/deploy-infra.yml"), "AWS_REGION_PLACEHOLDER", var.aws_region)
  commit_message      = "Update deploy-infra workflow via Terraform"
  commit_author       = "Terraform"
  commit_email        = "terraform@example.com"
  overwrite_on_create = true
  
  depends_on = [github_repository_file.github_dir_placeholder]
}

# Deploy API workflow (uses secrets, but we can document the values)
resource "github_repository_file" "deploy_api" {
  repository          = data.github_repository.main.name
  branch              = "main"
  file                = ".github/workflows/deploy-api.yml"
  content             = file("${path.module}/templates/deploy-api.yml")
  commit_message      = "Update deploy-api workflow via Terraform"
  commit_author       = "Terraform"
  commit_email        = "terraform@example.com"
  overwrite_on_create = true
  
  depends_on = [github_repository_file.github_dir_placeholder]
}

# Deploy Web workflow (uses secrets with fallback)
resource "github_repository_file" "deploy_web" {
  repository          = data.github_repository.main.name
  branch              = "main"
  file                = ".github/workflows/deploy-web.yml"
  content             = replace(file("${path.module}/templates/deploy-web.yml"), "API_BASE_URL_PLACEHOLDER", var.fly_app_url)
  commit_message      = "Update deploy-web workflow via Terraform"
  commit_author       = "Terraform"
  commit_email        = "terraform@example.com"
  overwrite_on_create = true
  
  depends_on = [github_repository_file.github_dir_placeholder]
}

# Smoke Tests workflow (uses secrets with fallback)
resource "github_repository_file" "smoke_tests" {
  repository          = data.github_repository.main.name
  branch              = "main"
  file                = ".github/workflows/smoke-tests.yml"
  content             = replace(
    replace(
      replace(file("${path.module}/templates/smoke-tests.yml"), "FLY_APP_URL_PLACEHOLDER", var.fly_app_url),
      "COGNITO_DOMAIN_PLACEHOLDER", var.cognito_domain
    ),
    "GITHUB_PAGES_URL_PLACEHOLDER", var.github_pages_url
  )
  commit_message      = "Update smoke-tests workflow via Terraform"
  commit_author       = "Terraform"
  commit_email        = "terraform@example.com"
  overwrite_on_create = true
  
  depends_on = [github_repository_file.github_dir_placeholder]
}

