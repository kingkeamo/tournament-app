terraform {
  backend "s3" {
    bucket         = "tournament-app-terraform-state"
    key            = "prod/terraform.tfstate"
    region         = "us-east-1"
    encrypt        = true
    dynamodb_table = "tournament-app-terraform-locks"
  }
}

