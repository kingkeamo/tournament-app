environment         = "prod"
aws_region          = "us-east-1"
project_name        = "tournament-app"
cognito_domain_prefix = "tournament-app-prod"
fly_org             = "your-fly-org"
github_repository   = "kingkeamo/tournament-app"

allowed_callback_urls = [
  "https://kingkeamo.github.io/tournament-app/authentication/login-callback"
]

allowed_logout_urls = [
  "https://kingkeamo.github.io/tournament-app/"
]

