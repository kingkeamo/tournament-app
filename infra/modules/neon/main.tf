resource "neon_project" "main" {
  name = "${var.project_name}-${var.environment}"

  settings {
    quota {
      active_time_seconds = 3600
      compute_time_seconds = 3600
      data_transfer_bytes = 5368709120
      logical_size_bytes = 5368709120
    }
  }
}

resource "neon_branch" "main" {
  project_id = neon_project.main.id
  name       = "main"
}

resource "neon_endpoint" "main" {
  project_id = neon_project.main.id
  branch_id  = neon_branch.main.id
  type       = "read_write"
}

resource "neon_database" "main" {
  project_id = neon_project.main.id
  branch_id  = neon_branch.main.id
  name       = "${var.project_name}_${var.environment}"
  owner_name = neon_project.main.default_role
}

