terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 2.50"
    }
  }
}

# Declare the necessary variables
variable "azure_client_id" {
  description = "Azure Client ID (Service Principal)"
  type        = string
}

variable "azure_client_secret" {
  description = "Azure Client Secret (Service Principal)"
  type        = string
}

variable "azure_tenant_id" {
  description = "Azure Tenant ID"
  type        = string
}

variable "subscription_id" {
  description = "Azure Subscription ID"
  type        = string
}



# Declare the provider and use environment variables for Service Principal authentication
provider "azurerm" {
  features {}
  client_id       = var.azure_client_id       # Service Principal Client ID
  client_secret   = var.azure_client_secret   # Service Principal Client Secret
  tenant_id       = var.azure_tenant_id       # Azure Tenant ID
  subscription_id = var.subscription_id       # Azure Subscription ID
}

# Create an Azure Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "aipjmeportrg"
  location = "East US"
}

# Create an Azure App Service Plan (for Function App)
resource "azurerm_app_service_plan" "asp" {
  name                = "aipjmeport-asp"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "FunctionApp"
  reserved            = true  # This is for Linux-based services

  sku {
    tier = "Standard"//"Consumption"
    size =  "B1"// "Y1"  # Consumption Plan (free-tier for Function App)
  }
}


