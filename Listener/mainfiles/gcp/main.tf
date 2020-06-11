provider "google" {
    project = "absolute-shell-279203"
    region = "asia-northeast1"
    zone = "asia-northeast1-b"
}

resource "google_cloud_run_service" "gcp_run"{
    name                = "amazingrace2"
    location            = "asia-northeast1"
    

    template {
        spec {
            containers {
                image = "asia.gcr.io/absolute-shell-279203/binarythistle/raceclient8080"
                env {
                  name = "ClientName"
                  value = "Google GCS"
                }
            }
        }
    }

    traffic {
        percent         = 100
        latest_revision = true
    }
}

data "google_iam_policy" "noauth" {
  binding {
    role = "roles/run.invoker"
    members = [
      "allUsers",
    ]
  }
}

resource "google_cloud_run_service_iam_policy" "noauth" {
  location    = google_cloud_run_service.gcp_run.location
  project     = google_cloud_run_service.gcp_run.project
  service     = google_cloud_run_service.gcp_run.name

  policy_data = data.google_iam_policy.noauth.policy_data
}
//Need to specify allow unathenticated
//https://cloud.google.com/run/docs/securing/managing-access?_ga=2.157791288.-323241022.1591184017&_gac=1.178523536.1591184037.EAIaIQobChMIncqmrcbl6QIVWSUrCh0ZnAwwEAAYASAAEgJX7fD_BwE