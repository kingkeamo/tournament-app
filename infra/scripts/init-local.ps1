# Initialize Terraform with local backend for testing (PowerShell)

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$EnvDir = Join-Path $ScriptDir "..\environments\dev"

Set-Location $EnvDir

Write-Host "Initializing Terraform with local backend..." -ForegroundColor Green

# Create local backend config if it doesn't exist
if (-not (Test-Path "backend-local.tf")) {
    @"
terraform {
  backend "local" {
    path = "terraform.tfstate"
  }
}
"@ | Out-File -FilePath "backend-local.tf" -Encoding UTF8
    Write-Host "Created backend-local.tf" -ForegroundColor Yellow
}

# Initialize with local backend
# Copy backend-local.tf to backend.tf temporarily for init
Copy-Item "backend-local.tf" -Destination "backend.tf" -Force
terraform init -reconfigure

Write-Host "`nTerraform initialized successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Ensure secrets.tfvars has all your credentials"
Write-Host "2. Set AWS credentials from secrets.tfvars:"
Write-Host "   .\set-aws-credentials.ps1"
Write-Host "3. Run: terraform plan -var-file=main.tfvars -var-file=secrets.tfvars"
Write-Host "4. Run: terraform apply -var-file=main.tfvars -var-file=secrets.tfvars"

