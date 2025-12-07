# Terraform Infrastructure Outputs

This file contains the outputs from Terraform infrastructure deployment.

**⚠️ Note**: This file may contain sensitive information. Consider adding it to `.gitignore` if committing to version control.

## Dev Environment Outputs

Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")

### Cognito Configuration

- **User Pool ID**: `eu-west-2_rJ7t72Q1D`
- **Client ID**: `15a5efkttap10vfd93nh341bek`
- **Authority**: `https://cognito-idp.eu-west-2.amazonaws.com/eu-west-2_rJ7t72Q1D`
- **Domain**: `tournament-app-dev.auth.eu-west-2.amazoncognito.com`
- **Hosted UI URL**: `https://tournament-app-dev.auth.eu-west-2.amazoncognito.com`

### Fly.io Configuration

- **App Name**: `tournament-app-dev`
- **App URL**: `https://tournament-app-dev.fly.dev`
- **Status**: ✅ App created successfully

### GitHub Pages Configuration

- **Repository**: `kingkeamo/tournament-app`
- **Pages URL**: `https://kingkeamo.github.io/tournament-app`
- **Status**: ✅ Pages enabled via Terraform (using GitHub API)
- **Source Branch**: `gh-pages`

## Application Configuration

### API (TournamentApp.Api)

Update `appsettings.json` or user secrets with:

```json
{
  "Cognito": {
    "UserPoolId": "eu-west-2_rJ7t72Q1D",
    "ClientId": "15a5efkttap10vfd93nh341bek",
    "Authority": "https://cognito-idp.eu-west-2.amazonaws.com/eu-west-2_rJ7t72Q1D"
  }
}
```

### Web (TournamentApp.Web)

Update `appsettings.json` with:

```json
{
  "ApiBaseUrl": "https://tournament-app-dev.fly.dev",
  "Cognito": {
    "ClientId": "15a5efkttap10vfd93nh341bek",
    "Authority": "https://cognito-idp.eu-west-2.amazonaws.com/eu-west-2_rJ7t72Q1D",
    "RedirectUri": "https://kingkeamo.github.io/tournament-app/authentication/login-callback",
    "Domain": "tournament-app-dev.auth.eu-west-2.amazoncognito.com"
  }
}
```

## Next Steps

1. ✅ Cognito resources created - configuration updated in appsettings
2. ✅ Fly.io app created - ready for API deployment
3. ✅ GitHub Pages enabled - ready for frontend deployment
4. Update API user secrets with connection string (if using new database)
5. Deploy API to Fly.io
6. Deploy frontend to GitHub Pages (push to `gh-pages` branch)
7. Test authentication flow

