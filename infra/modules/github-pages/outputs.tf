output "url" {
  description = "GitHub Pages URL"
  value       = "https://${split("/", var.repository)[0]}.github.io/${split("/", var.repository)[1]}"
}

