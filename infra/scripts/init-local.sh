#!/bin/bash
# Initialize Terraform with local backend for testing

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_DIR="$SCRIPT_DIR/../environments/dev"

cd "$ENV_DIR"

echo "Initializing Terraform with local backend..."

# Create local backend config if it doesn't exist
if [ ! -f "backend-local.tf" ]; then
  cat > backend-local.tf << 'EOF'
terraform {
  backend "local" {
    path = "terraform.tfstate"
  }
}
EOF
  echo "Created backend-local.tf"
fi

# Initialize with local backend
terraform init -backend-config=backend-local.tf -reconfigure

echo "Terraform initialized successfully!"
echo ""
echo "Next steps:"
echo "1. Update main.tfvars with your values"
echo "2. Set environment variables for sensitive values:"
echo "   export TF_VAR_neon_api_key='your-key'"
echo "   export TF_VAR_github_token='your-token'"
echo "   export TF_VAR_fly_api_token='your-token'"
echo "3. Run: terraform plan -var-file=main.tfvars"
echo "4. Run: terraform apply -var-file=main.tfvars"

