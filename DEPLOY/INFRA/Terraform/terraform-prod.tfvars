# terraform-{env}.tfvars
// NOTE : Azure Service Credentials (SPN) will be conditonal picked up separately
// for each env : dev and prod fropm Github Secrets  , so no need to hardocode here. 

# Other Environment-Specific Variables
environment = "prod"

# Resource Group and Location for Dev
resource_group_name = "prod-rg"
location            = "East US"

# App Service Plan and Web App Configuration for Dev
app_service_plan_name = "prod-app-service-plan"
sku = "P1"  # Dev might use smaller scale (B1)
app_service_name = "prod-web-app"

# Storage Account Configuration for Dev
storage_account_name = "prodstorageacct"

