# Script to set AWS credentials from secrets.tfvars
# This reads the AWS credentials from secrets.tfvars and sets them as environment variables

$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SecretsFile = Join-Path $ScriptDir "..\environments\dev\secrets.tfvars"

if (-not (Test-Path $SecretsFile)) {
    Write-Host "Error: secrets.tfvars not found at $SecretsFile" -ForegroundColor Red
    exit 1
}

Write-Host "Reading AWS credentials from secrets.tfvars..." -ForegroundColor Green

# Read the secrets file line by line
$lines = Get-Content $SecretsFile

foreach ($line in $lines) {
    $line = $line.Trim()
    
    if ($line -match '^aws_access_key_id\s*=\s*"(.*)"') {
        $accessKey = $matches[1]
        $env:AWS_ACCESS_KEY_ID = $accessKey
        Write-Host "✓ AWS_ACCESS_KEY_ID set" -ForegroundColor Green
    }
    
    if ($line -match '^aws_secret_access_key\s*=\s*"(.*)"') {
        $secretKey = $matches[1]
        $env:AWS_SECRET_ACCESS_KEY = $secretKey
        Write-Host "✓ AWS_SECRET_ACCESS_KEY set" -ForegroundColor Green
    }
}

# Set default region
$env:AWS_DEFAULT_REGION = "eu-west-2"
Write-Host "✓ AWS_DEFAULT_REGION set to eu-west-2" -ForegroundColor Green

Write-Host ""
Write-Host "AWS credentials configured from secrets.tfvars" -ForegroundColor Green
Write-Host "These are set for the current PowerShell session only." -ForegroundColor Cyan
