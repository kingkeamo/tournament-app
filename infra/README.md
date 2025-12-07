# Infrastructure as Code

This directory contains Terraform configuration for provisioning all infrastructure components for the Tournament App.

## Prerequisites

- Terraform >= 1.0
- AWS CLI configured with appropriate credentials
- Neon API key
- GitHub token with repository permissions
- Fly.io API token

## Structure

```
infra/
├── main.tf              # Root module configuration
├── variables.tf         # Root variables
├── outputs.tf          # Root outputs
├── providers.tf        # Provider configuration
├── environments/       # Environment-specific configurations
│   ├── dev/
│   │   ├── main.tfvars # Dev variable values
│   │   └── backend.tf  # Dev backend configuration
│   └── prod/
│       ├── main.tfvars # Prod variable values
│       └── backend.tf  # Prod backend configuration
└── modules/            # Reusable modules
    ├── neon/           # Neon PostgreSQL database
    ├── cognito/        # Amazon Cognito authentication
    ├── fly/            # Fly.io application
    └── github-pages/   # GitHub Pages configuration
```

## Modules

### Neon Module
Provisions a Neon PostgreSQL database with:
- Project and branch
- Read-write endpoint
- Database instance

### Cognito Module
Provisions Amazon Cognito with:
- User pool
- PKCE-enabled app client (no client secret)
- Hosted UI domain

### Fly.io Module
Creates Fly.io application for API hosting.

### GitHub Pages Module
Enables GitHub Pages for the repository.

## Usage

### Initialize Terraform

```bash
cd infra/environments/dev
terraform init
```

### Plan Changes

```bash
terraform plan -var-file=main.tfvars
```

### Apply Changes

```bash
terraform apply -var-file=main.tfvars
```

### Required Variables

You'll need to provide these variables (via tfvars, environment variables, or CLI):

- `neon_api_key`: Neon API key
- `github_token`: GitHub personal access token
- `fly_api_token`: Fly.io API token
- `github_repository`: Repository name (e.g., "username/tournament-app")
- `cognito_domain_prefix`: Unique prefix for Cognito domain
- `fly_org`: Fly.io organization name

### Backend Configuration

The backend uses S3 for state storage. You'll need to create:
- S3 bucket: `tournament-app-terraform-state`
- DynamoDB table: `tournament-app-terraform-locks` (for state locking)

## Outputs

After applying, Terraform will output:
- Database connection string
- Cognito configuration (User Pool ID, Client ID, Domain, Authority)
- Fly.io app name and URL
- GitHub Pages URL

These outputs should be used to configure:
- API application settings
- Frontend application settings
- CI/CD pipelines

