# AgenticAIService

This is a minimal .NET 8 Web API that exposes a single controller endpoint to forward prompts to an external Agentic AI/OpenAI-style endpoint.

## Configuration

### Prerequisites

Before running the AgenticAIService, you need to configure the `appsettings.json` file with the following credentials:

#### 1. **Azure OpenAI API Key**
   - Navigate to `appsettings.json`
   - Locate the `"AgenticAIAzureBoard"` section
   - Set `"ApiKey"` to your Azure OpenAI API key:
     ```json
     "AgenticAIAzureBoard": {
       "Endpoint": "https://ai-3026-applibkit-resource.openai.azure.com/openai/v1/",
       "ApiKey": "YOUR_AZURE_OPENAI_API_KEY_HERE",
       "ApiKeyHeader": "api-key"
     }
     ```

#### 2. **Azure DevOps Personal Access Token (PAT)**
   - Locate the `"AzureBoardConnector"` array in `appsettings.json`
   - For each Azure Board configuration, add your **Personal Access Token** to the `"personalAccessToken"` field:
     ```json
     "AzureBoardConnector": [
       {
         "orgUrl": "https://dev.azure.com/YourOrganization",
         "personalAccessToken": "YOUR_AZURE_DEVOPS_PAT_HERE",
         "projectName": "YourProjectName",
         ...
       }
     ]
     ```
   - **Note:** Generate PATs in Azure DevOps at: Settings → Personal access tokens

### Authorization Patterns

The controller supports two authorization patterns:
 - API-key header: set `AgenticAI:ApiKeyHeader` to the header name (default `api-key`) and `AgenticAI:ApiKey` to the key value.
 - Authorization Bearer: set `AgenticAI:ApiKeyHeader` to `Authorization` and `AgenticAI:ApiKey` to the token; the controller will send `Authorization: Bearer <token>`.
