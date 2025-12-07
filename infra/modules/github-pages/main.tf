# Get the repository information
data "github_repository" "main" {
  full_name = var.repository
}

# Enable GitHub Pages via GitHub API
# The GitHub provider doesn't have a direct github_repository_pages resource,
# so we use the GitHub REST API via PowerShell
resource "null_resource" "github_pages" {
  triggers = {
    repository = var.repository
  }
  
  provisioner "local-exec" {
    command     = "$headers = @{'Authorization' = 'Bearer ${var.github_token}'; 'Accept' = 'application/vnd.github+json'; 'X-GitHub-Api-Version' = '2022-11-28'}; $body = @{source = @{branch = 'gh-pages'; path = '/'}} | ConvertTo-Json; try { Invoke-RestMethod -Uri 'https://api.github.com/repos/${var.repository}/pages' -Method PUT -Headers $headers -Body $body -ContentType 'application/json' -ErrorAction Stop; Write-Host 'GitHub Pages enabled successfully' } catch { Write-Host 'GitHub Pages may already be enabled or repository does not exist: ' $_.Exception.Message }"
    interpreter = ["PowerShell", "-Command"]
    on_failure  = continue
  }
}
