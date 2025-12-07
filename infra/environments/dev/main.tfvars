environment         = "dev"
aws_region          = "eu-west-2"  # London
project_name        = "tournament-app"
cognito_domain_prefix = "tournament-app-dev"  # Must be globally unique - add your username/identifier
fly_org             = "personal"
github_repository   = "kingkeamo/tournament-app"

allowed_callback_urls = [
  "https://localhost:7009/authentication/login-callback",
  "https://kingkeamo.github.io/tournament-app/authentication/login-callback"
]

allowed_logout_urls = [
  "https://localhost:7009/",
  "https://kingkeamo.github.io/tournament-app/"
]

