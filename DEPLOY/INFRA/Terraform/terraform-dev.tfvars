# terraform-{env}.tfvars
// NOTE : Azure Service Credentials (SPN) will be conditonal pickedup separately
// for each env : dev and prod fropm Github Secrets  , so no need to hardocode here. 

# Other Environment-Specific Variables
environment = "dev"

# Resource Group and Location for Dev
resource_group_name = "dev-rg"
location            = "East US"

# App Service Plan and Web App Configuration for Dev
app_service_plan_name = "dev-app-service-plan"
sku = "B1"  # Dev might use smaller scale (B1)
app_service_name = "dev-web-app"

# Storage Account Configuration for Dev
storage_account_name = "devstorageacct"

