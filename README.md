# Tournament App

A modern web application for managing pool tournaments in pubs and bars. Built with Clean Architecture, .NET 8, Blazor WebAssembly, and MudBlazor.

## Features

- **Tournament Management**: Create and manage tournaments
- **Player Management**: Add and organize players
- **Automated Bracket Generation**: Single elimination bracket generation
- **Live Score Tracking**: Update match scores and automatically advance winners
- **Mobile-Friendly UI**: Responsive design for players and admins
- **Authentication**: Amazon Cognito integration with PKCE flow

## Architecture

The application follows Clean Architecture principles with the following layers:

- **Domain**: Core entities and business logic
- **Application**: CQRS commands/queries with MediatR
- **Infrastructure**: Data access (Dapper), DbUp migrations, external services
- **API**: ASP.NET Core Web API with JWT authentication
- **Web**: Blazor WebAssembly frontend with MudBlazor

## Tech Stack

- **Backend**: .NET 8, ASP.NET Core Web API
- **Frontend**: Blazor WebAssembly, MudBlazor
- **Database**: Neon PostgreSQL
- **ORM**: Dapper
- **Migrations**: DbUp
- **CQRS**: MediatR
- **Authentication**: Amazon Cognito
- **Infrastructure**: Terraform
- **CI/CD**: GitHub Actions
- **Hosting**: Fly.io (API), GitHub Pages (Frontend)

## Prerequisites

- .NET 8 SDK
- Docker (for building API container)
- Terraform >= 1.0 (for infrastructure)
- AWS CLI configured (for Cognito)
- Neon API key
- Fly.io account
- GitHub account

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/tournament-app
cd tournament-app
```

### 2. Set Up Local Database (Choose One Option)

#### Option A: Using Neon CLI (Quick Setup for Local Development)

For quick local development, you can use the Neon CLI:

```bash
# Install Neon CLI (if not already installed)
npm install -g neonctl

# Initialize and create a local Neon project
npx neonctl@latest init

# Follow the prompts to create a project and get your connection string
```

This will create a Neon project and provide you with a connection string to use in user secrets.

#### Option B: Using Terraform (Infrastructure as Code)

For production-like setup, use Terraform (see [Infrastructure Setup](#infrastructure-setup) section below).

### 3. Configure User Secrets

The application uses placeholder values in `appsettings.json` files. Configure actual values using .NET user secrets for local development.

#### API Project

```bash
cd src/TournamentApp.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-postgresql-connection-string"
dotnet user-secrets set "Cognito:UserPoolId" "your-cognito-user-pool-id"
dotnet user-secrets set "Cognito:ClientId" "your-cognito-client-id"
dotnet user-secrets set "Cognito:Authority" "https://cognito-idp.region.amazonaws.com/user-pool-id"
```

#### Web Project

```bash
cd src/TournamentApp.Web
dotnet user-secrets init
dotnet user-secrets set "ApiBaseUrl" "http://localhost:5000"
dotnet user-secrets set "Cognito:ClientId" "your-cognito-client-id"
dotnet user-secrets set "Cognito:Authority" "https://cognito-idp.region.amazonaws.com/user-pool-id"
dotnet user-secrets set "Cognito:RedirectUri" "http://localhost:5000/authentication/login-callback"
```

### 4. Run Database Migrations

The API will automatically run DbUp migrations on startup. Ensure your database connection string is configured correctly.

### 5. Run the Application

#### API

```bash
cd src/TournamentApp.Api
dotnet run
```

The API will be available at `http://localhost:5000`

#### Web (Frontend)

```bash
cd src/TournamentApp.Web
dotnet run
```

The web app will be available at `http://localhost:5001` (or the port shown in the console)

### 6. Build and Test

```bash
dotnet build
dotnet test
```

## Infrastructure Setup

See [infra/README.md](infra/README.md) for detailed Terraform setup instructions.

### Quick Start

1. Configure Terraform variables in `infra/environments/dev/main.tfvars`
2. Initialize Terraform:
   ```bash
   cd infra/environments/dev
   terraform init
   ```
3. Plan and apply:
   ```bash
   terraform plan -var-file=main.tfvars
   terraform apply -var-file=main.tfvars
   ```

## CI/CD

The project includes GitHub Actions workflows for:

- **Build & Test**: Builds all projects and runs tests
- **Deploy Infrastructure**: Provisions infrastructure with Terraform
- **Deploy API**: Builds and deploys API to Fly.io
- **Deploy Web**: Builds and deploys Blazor WASM to GitHub Pages
- **Smoke Tests**: Validates deployed infrastructure and applications

## Project Structure

```
tournament-app/
├── infra/                    # Terraform infrastructure
│   ├── modules/             # Reusable Terraform modules
│   └── environments/        # Environment-specific configs
├── src/
│   ├── TournamentApp.Api/           # Web API
│   ├── TournamentApp.Application/   # CQRS commands/queries
│   ├── TournamentApp.Domain/       # Domain entities
│   ├── TournamentApp.Infrastructure/# Data access, migrations
│   └── TournamentApp.Web/          # Blazor WASM frontend
├── tests/                   # Test projects
└── .github/workflows/       # GitHub Actions workflows
```

## API Endpoints

### Health
- `GET /health` - Health check
- `GET /db-check` - Database connectivity check

### Tournaments
- `POST /api/tournaments` - Create tournament
- `GET /api/tournaments` - List all tournaments
- `GET /api/tournaments/{id}` - Get tournament by ID

### Players
- `POST /api/players` - Create player
- `GET /api/players` - List all players

### Bracket
- `POST /api/tournaments/{id}/bracket/generate` - Generate bracket
- `GET /api/tournaments/{id}/bracket` - Get bracket

### Matches
- `POST /api/matches/{id}/score` - Update match score

## Database Schema

The database uses PascalCase naming with no underscores:

- **Players**: Id, Name, CreatedAt
- **Tournaments**: Id, Name, Status, CreatedAt
- **TournamentPlayers**: TournamentId, PlayerId (junction table)
- **Matches**: Id, TournamentId, Round, Position, Player1Id, Player2Id, Score1, Score2, WinnerId, Status, CreatedAt

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests and ensure they pass
5. Submit a pull request

## License

Apache License 2.0 - see [LICENSE](LICENSE) file for details.
