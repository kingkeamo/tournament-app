# Terraform Local Setup Guide

This guide will help you set up and test Terraform infrastructure locally.

## Prerequisites

1. **Terraform installed** (>= 1.0)
   ```bash
   # Check version
   terraform version
   ```

2. **Required API Keys/Tokens:**
   - Neon API key: Get from https://console.neon.tech/
   - GitHub token: Create at https://github.com/settings/tokens (needs `repo` scope)
   - Fly.io API token: 
     - **For local testing**: Personal token from https://fly.io/user/personal_access_tokens
     - **For CI/CD/Production**: Organization token (recommended)
       ```bash
       fly tokens create org --org <your-org-name> --name "terraform-token"
       ```
   - AWS credentials: Configure via `aws configure` or environment variables

3. **AWS Resources for Backend:**
   - S3 bucket for Terraform state: `tournament-app-terraform-state`
   - DynamoDB table for state locking: `tournament-app-terraform-locks`

## Quick Start

### 1. Configure Sensitive Variables

**Option A: Use secrets.tfvars file (Recommended)**

Create `infra/environments/dev/secrets.tfvars` (this file is gitignored):

```hcl
fly_api_token = "your-fly-api-token"
neon_api_key = "your-neon-api-key"
github_token = "your-github-token"
aws_access_key_id = "your-aws-access-key-id"
aws_secret_access_key = "your-aws-secret-access-key"
```

Then set AWS credentials from the file and run terraform:

**PowerShell (Recommended):**
```powershell
.\infra\scripts\set-aws-credentials.ps1
terraform plan -var-file=main.tfvars -var-file=secrets.tfvars
terraform apply -var-file=main.tfvars -var-file=secrets.tfvars
```

**Bash/WSL (Alternative):**
```bash
source infra/scripts/set-aws-credentials.sh
terraform plan -var-file=main.tfvars -var-file=secrets.tfvars
terraform apply -var-file=main.tfvars -var-file=secrets.tfvars
```

**Option B: Use Environment Variables**

```bash
export TF_VAR_neon_api_key="your-neon-api-key"
export TF_VAR_github_token="your-github-token"
export TF_VAR_fly_api_token="your-fly-api-token"
export AWS_ACCESS_KEY_ID="your-aws-access-key-id"
export AWS_SECRET_ACCESS_KEY="your-aws-secret-access-key"
export AWS_DEFAULT_REGION="eu-west-2"
```

**⚠️ Important**: Never commit sensitive tokens to git. The `secrets.tfvars` file is gitignored.

### 2. Update Dev Configuration

Edit `infra/environments/dev/main.tfvars` with your actual values:

```hcl
environment         = "dev"
aws_region          = "us-east-1"
project_name        = "tournament-app"
cognito_domain_prefix = "tournament-app-dev-yourname"  # Must be globally unique
fly_org             = "your-fly-org-name"
github_repository   = "kingkeamo/tournament-app"

allowed_callback_urls = [
  "https://localhost:7009/authentication/login-callback",
  "https://kingkeamo.github.io/tournament-app/authentication/login-callback"
]

allowed_logout_urls = [
  "https://localhost:7009/",
  "https://kingkeamo.github.io/tournament-app/"
]
```

### 3. Set Up AWS Backend (Optional - can use local backend for testing)

For local testing, you can use a local backend instead of S3:

Create `infra/environments/dev/backend-local.tf`:
```hcl
terraform {
  backend "local" {
    path = "terraform.tfstate"
  }
}
```

Then initialize with:
```bash
terraform init -backend-config=backend-local.tf
```

Or create the S3 backend resources:
```bash
# Create S3 bucket
aws s3 mb s3://tournament-app-terraform-state --region us-east-1

# Create DynamoDB table for locking
aws dynamodb create-table \
  --table-name tournament-app-terraform-locks \
  --attribute-definitions AttributeName=LockID,AttributeType=S \
  --key-schema AttributeName=LockID,KeyType=HASH \
  --billing-mode PAY_PER_REQUEST \
  --region us-east-1
```

### 4. Initialize Terraform

```bash
cd infra/environments/dev
terraform init
```

### 5. Validate Configuration

```bash
terraform validate
```

### 6. Plan Changes (Dry Run)

```bash
# If using secrets.tfvars:
terraform plan -var-file=main.tfvars -var-file=secrets.tfvars

# If using environment variables:
terraform plan -var-file=main.tfvars
```

This will show you what resources will be created without actually creating them.

### 7. Apply Infrastructure

**⚠️ Warning: This will create real resources and may incur costs!**

```bash
# If using secrets.tfvars:
terraform apply -var-file=main.tfvars -var-file=secrets.tfvars

# If using environment variables:
terraform apply -var-file=main.tfvars
```

Review the plan carefully, then type `yes` to confirm.

### 8. View Outputs

After applying, get the outputs:

```bash
terraform output
```

Or get specific outputs:
```bash
terraform output database_connection_string
terraform output cognito_user_pool_id
terraform output cognito_client_id
```

### 9. Update Application Configuration

Use the Terraform outputs to update:
- API `appsettings.json` or user secrets (connection string, Cognito settings)
- Frontend `appsettings.json` (Cognito settings, API URL)

### 10. Destroy Infrastructure (Cleanup)

When done testing:

```bash
# If using secrets.tfvars:
terraform destroy -var-file=main.tfvars -var-file=secrets.tfvars

# If using environment variables:
terraform destroy -var-file=main.tfvars
```

## Testing Strategy

1. **Start Small**: Test with just the Neon database first
2. **Incremental**: Add Cognito, then Fly.io, then GitHub Pages
3. **Verify**: Check each resource after creation
4. **Clean Up**: Destroy resources when done to avoid costs

## Troubleshooting

### Provider Authentication Issues

- **AWS**: Run `aws configure` or set `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY`
- **Neon**: Verify API key at https://console.neon.tech/
- **GitHub**: Token needs `repo` scope
- **Fly.io**: Verify token at https://fly.io/user/personal_access_tokens

### Backend Issues

If S3 backend fails, use local backend for testing (see step 3).

### State Lock Issues

If state is locked, check DynamoDB table or delete lock file if using local backend.

## Next Steps

After infrastructure is provisioned:
1. Update API configuration with connection string and Cognito settings
2. Update frontend configuration with Cognito settings
3. Test API deployment to Fly.io
4. Test frontend deployment to GitHub Pages
5. Run smoke tests

