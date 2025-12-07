resource "null_resource" "fly_app" {
  triggers = {
    app_name = "${var.project_name}-${var.environment}"
  }

  provisioner "local-exec" {
    command     = "if (Get-Command flyctl -ErrorAction SilentlyContinue) { flyctl apps create ${self.triggers.app_name} --org ${var.fly_org} } else { $env:Path += ';C:\\Users\\$env:USERNAME\\.fly\\bin'; flyctl apps create ${self.triggers.app_name} --org ${var.fly_org} }"
    interpreter = ["PowerShell", "-Command"]
    on_failure  = continue
    environment = {
      FLY_API_TOKEN = var.fly_api_token
    }
  }
}

