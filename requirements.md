🎱 Tournament App – Requirements & Architecture Specification
📌 Project Overview

The Tournament App is a modern web application designed to help pub/bar tournament organisers manage pool tournaments. The system enables:

A responsive mobile-friendly UI for players

An admin UI for managing tournaments

Automated bracket generation

Live match score tracking

Player statistics and ranking

Optional identity provider login (Google, Apple, Azure AD, etc.)

The application will use Clean Architecture, .NET 8, Blazor WebAssembly, MudBlazor, CQRS with MediatR, Neon PostgreSQL, and DbUp for database migrations.
Infrastructure will be fully automated using Terraform and GitHub Actions, with hosting on Fly.io (API) and GitHub Pages (frontend).

🧩 Architecture Overview
Frontend

Blazor WebAssembly

MudBlazor

Hosted on GitHub Pages (static hosting)

Backend API

.NET 8 Web API

Clean Architecture

CQRS with MediatR

Dapper for queries

DbUp for migrations

Hosted on Fly.io

Authentication

Amazon Cognito

Uses PKCE flow for WASM auth

Supports external identity providers:

Google

Apple

Azure AD

Any OIDC provider

Database

Neon.tech PostgreSQL

Serverless/freely scaled

Terraform-managed

DbUp-controlled schema

CI/CD

GitHub Actions:

Build/Test pipeline

Infra provisioning (Terraform)

API deploy (Fly.io)

Web deploy (GitHub Pages)

Full smoke test suite

📂 Required Folder Structure
tournament-app/
│
├── infra/
│   ├── main.tf
│   ├── variables.tf
│   ├── outputs.tf
│   ├── providers.tf
│   ├── environments/
│   │   ├── dev/
│   │   │   ├── main.tfvars
│   │   │   └── backend.tf
│   │   └── prod/
│   │       ├── main.tfvars
│   │       └── backend.tf
│   │
│   ├── modules/
│   │   ├── fly/                    # Fly.io app & secrets
│   │   ├── neon/                   # Neon DB
│   │   ├── cognito/                # Amazon Cognito
│   │   └── github-pages/           # Enable Pages
│   │
│   └── README.md
│
├── src/
│   ├── TournamentApp.Api/
│   ├── TournamentApp.Application/
│   ├── TournamentApp.Domain/
│   ├── TournamentApp.Infrastructure/
│   │   ├── DbUp/                   # Migration scripts
│   │   └── Data/                   # Dapper repositories
│   └── TournamentApp.Web/
│
├── tests/
│   ├── TournamentApp.UnitTests/
│   ├── TournamentApp.ApiTests/
│   └── TournamentApp.IntegrationTests/
│
├── .github/
│   └── workflows/
│       ├── build-test.yml
│       ├── deploy-infra.yml
│       ├── deploy-api.yml
│       ├── deploy-web.yml
│       └── smoke-tests.yml
│
├── fly.toml
├── README.md
└── .gitignore

📑 Database Requirements
Conventions

Use PascalCase table names

No underscores anywhere

Use plural names where applicable

Primary keys named Id

Foreign keys named EntityId

Required Tables (initial schema)
Players
Tournaments
TournamentPlayers
Matches

Created using DbUp

Migrations stored in:

src/TournamentApp.Infrastructure/DbUp/
    0001-init.sql
    0002-add-matches.sql
    ...


DbUp will:

Ensure schema exists

Apply migrations in order

Run automatically in API startup

🎯 Functional Requirements
Phase 1 – MVP
Admin Features

Create tournament

Add/remove players

Generate single elimination bracket

Edit match scores

Advance winners automatically

View bracket visually (MudBlazor)

Track tournament progress live

Player Features

Mobile-friendly bracket viewer

Optional player registration (Phase 3)

Optional identity provider login

🧪 API Requirements

Endpoints (CQRS-driven):

Health
GET /health

Database Check
GET /db-check

Tournaments
POST /tournaments
GET /tournaments
GET /tournaments/{id}

Players
POST /players
GET /players

Bracket
POST /tournaments/{id}/generate
GET /tournaments/{id}/bracket

Matches
POST /matches/{id}/score

🧠 CQRS Requirements
Commands

CreateTournamentCommand

AddPlayerCommand

GenerateBracketCommand

UpdateMatchScoreCommand

Queries

GetTournamentListQuery

GetTournamentQuery

GetBracketQuery

🔐 Authentication Requirements

Amazon Cognito User Pool

PKCE App Client (no client secret required)

Hosted UI domain

Callback URLs for GitHub Pages

Ability to add external identity providers

Optional Pre-SignUp Lambda to link accounts

API validates JWT access tokens

🌐 Hosting Requirements
Frontend

Build Blazor WASM

Deploy /wwwroot to GitHub Pages

Enable HTTPS

Create 404.html fallback for SPA routing

Backend API

Deploy container to Fly.io

Store secrets in Fly.io:

PostgreSQL connection

Cognito settings

Health checks configured

Database

Neon.tech PostgreSQL via Terraform

DbUp applies schema at API startup

🔧 CI/CD Requirements
Workflow 1 – Build & Test (build-test.yml)

Build API

Build Web

Run tests

Validate Terraform

Workflow 2 – Terraform Infra Deploy (deploy-infra.yml)

Terraform init → plan → apply

Output API URL, DB connection, cognito info

Workflow 3 – API Deploy (deploy-api.yml)

Build & push image

Fly.io deploy

Call /health and /db-check

Workflow 4 – Web Deploy (deploy-web.yml)

Build Blazor

Deploy to Pages

Validate homepage (HTTP 200)

Workflow 5 – Smoke Test (smoke-tests.yml)

Hit static site

Hit API

Hit DB (via API)

Validate Cognito openid document

🧭 Developer Workflow

Clone repo:

git clone https://github.com/<youraccount>/tournament-app


Open folder in Cursor:

C:\git\evolve\tournament-app


Ask Cursor:

“Generate Clean Architecture solution”

“Create Terraform modules”

“Create GitHub Actions workflows”

“Add DbUp migrations”

“Implement tournament domain entities”

Commit & push

GitHub Actions handles:

Infra

API deploy

Web deploy

Smoke tests