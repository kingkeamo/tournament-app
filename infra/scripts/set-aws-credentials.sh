#!/bin/bash
# Script to set AWS credentials from secrets.tfvars
# This reads the AWS credentials from secrets.tfvars and sets them as environment variables

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SECRETS_FILE="$SCRIPT_DIR/../environments/dev/secrets.tfvars"

if [ ! -f "$SECRETS_FILE" ]; then
    echo "Error: secrets.tfvars not found at $SECRETS_FILE" >&2
    exit 1
fi

echo "Reading AWS credentials from secrets.tfvars..."

# Extract AWS credentials using grep and sed
ACCESS_KEY=$(grep 'aws_access_key_id' "$SECRETS_FILE" | sed -n 's/.*"\([^"]*\)".*/\1/p')
SECRET_KEY=$(grep 'aws_secret_access_key' "$SECRETS_FILE" | sed -n 's/.*"\([^"]*\)".*/\1/p')

if [ -n "$ACCESS_KEY" ]; then
    export AWS_ACCESS_KEY_ID="$ACCESS_KEY"
    echo "✓ AWS_ACCESS_KEY_ID set"
else
    echo "Warning: aws_access_key_id not found in secrets.tfvars" >&2
fi

if [ -n "$SECRET_KEY" ]; then
    export AWS_SECRET_ACCESS_KEY="$SECRET_KEY"
    echo "✓ AWS_SECRET_ACCESS_KEY set"
else
    echo "Warning: aws_secret_access_key not found in secrets.tfvars" >&2
fi

# Set default region
export AWS_DEFAULT_REGION="eu-west-2"
echo "✓ AWS_DEFAULT_REGION set to eu-west-2"

echo ""
echo "AWS credentials configured from secrets.tfvars"
echo "These are set for the current shell session only."
echo ""
echo "To use in your current shell, run:"
echo "  source $SCRIPT_DIR/set-aws-credentials.sh"

