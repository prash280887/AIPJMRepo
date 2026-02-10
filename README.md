

# aipjm - AI Project Management Reports  


## Project Team – IntelliTrace

## 👥 Team Members 

--- 

**Prashant Akhouri**
📧 Designation: Technical Architect  
📧 Email: Prashant.akhouri@winwire.com  


<div align="center">

# aipjm - AI Project Management Reports

</div>
 

This solution is developed using core technologies -  React | .NET 8 Microservices | Azure AI

## Problem Statement

Organizations frequently struggle with manual report preparation, inconsistent formats, and limited analytical insights. aipjm addresses these challenges by providing automated, AI-driven reporting across multiple formats. The solution delivers standardized, insightful, and extensible project reporting suitable for enterprise environments.

## Scope

The system should automatically pull project and time-track data from Azure Boards and convert it into a unified format. It can generate and send reports of emails based in specific or user-provided template formats. Azure Agentic AI capabilities would be injected to summarize project-related progress, risks, highlights, and next steps. The final report must be exportable in stand formats of PDF/Word/HTML. Users should be able to schedule report generation and review/edit the AI-generated content before publishing. Reports can be scheduled or manually run to send final mail with report data to respective recipients.

## Getting Started

### Code Setup and Configurations 
Locally , Open the Repo in VS code , 
Goto terminal , run the rundev.ps1 file in SHELL folder to launch the UI and  services. 
By default aipjm react app UI will be launched at https://localhost:3000 
#### Login with demo credentials
- **Username**: `admin@aipjm.com`
- **Password**: `password`

You'll receive a JWT token and be redirected to the Home page.

***NOTE*** : Before running the UI , you need to configure the `appsettings.json` file with the following credentials:

####  **  Azure OpenAI API Key**
   - Navigate to `appsettings.json`
   - Locate the `"AgenticAIAzureBoard"` section
   - Set `"ApiKey"` to your Azure OpenAI API key:
   Eg.
     ```json
     "AgenticAIAzureBoard": {
       "Endpoint": "https://ai-3026-applibkit-resource.openai.azure.com/openai/v1/",
       "ApiKey": "YOUR_AZURE_OPENAI_API_KEY_HERE",
       "ApiKeyHeader": "api-key"
     }
     ```

####  **Azure DevOps Personal Access Token (PAT)**
   - Locate the `"AzureBoardConnector"` array in `appsettings.json`
   - For each Azure Board configuration, add your **Personal Access Token** to the `"personalAccessToken"` field:
   Eg.
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

    **Note:** Generate PATs in Azure DevOps at: Settings → Personal access tokens

NOTE - (Optionally) To configure AAD Login -  Create Service Principal for React App regirations and bind to UI Login.
Configure App reg entries in file at - APPS\ReactAppUI\src\azure-auth\msalConfig.js

```json
export const msalConfig = {
    auth : {
        clientId: "", // Application (client) ID from the Azure portal App Reg
        authority: "https://login.microsoftonline.com/<yourTenantInfo>.onmicrosoft.com/",// Your AAD authority
        redirectUri: "http://localhost:3000/", // Your react app redirect URI configured in App reg  
    },
    cache: {
    cacheLocation: "localStorage", // or sessionStorage
    storeAuthStateInCookie: false, 
    cacheExpirationMilliseconds: 1000 // 1 hour
  }
};
export const loginRequest = {
    scopes: ["openid", "profile", "User.Read"] // Add the scopes you need
   };

```
#### Email Configurations 
The solution uses Azure SendGrid email services .You can add your org's sendgrid API KEY email configs 
in Function App Environemnt varibales or local.settings.json .
Eg.
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AgenticAIService_BaseUrl": "https://<AgenticAAIservicebaseurl>/",//this is Agentic AI Service base url
    "SendGridApiKey": "SG.XXXXXXXXXXXXX" , // add SENDGRID API KEY here 
     "SmtpFromEmail": "prashant.akhouri@winwire.com",
    "SmtpToEmail": "prashant.akhouri@winwire.com",
    "SmtpCcEmail": "kishore.jakkani@WinWire.com",
    "SmtpBccEmail": "pavani.Sowdepalli@WinWire.com"
  },
  "Host": {
    "CORS": "*",
    "CORSCredentials": false
  },
 
}

```
NOTE : Azure Send Grid Service must have your local IP whitelisted by IT team for receieving emails

### Code Build and Run 
A)
Localy you can build solution annd run code by runnig the rundev.ps1 in SHELL  folder
SHELL> .\rundev.ps1

Or
B) 
Buidl annd run  APIM.sln in Visual Studio for backend hosting

And run react UI in VSccode Terminal :
APPS\ReactAppUI\src> npm install 
 APPS\ReactAppUI\src> npm start

## Solutioning Approach

The solution follows a layered modern microservices-inspired backend architecture powered by modern Agentic AI capabilities. A React-based frontend communicates with secure backend APIs and Azure AI Foundry services. Authentication (AAD), reporting, and AI intelligence are cleanly separated to enable scalability, resilience, and independent evolution.

### Major highlights of solution approach include -

- Automated Data Extraction: Pull project related data from Azure Boards connectivity services. 
- Convert extracted data into a unified internal format. 
- Report Templates and Data formats: Support predefined templates and multiple AI generated report layouts. 
- AI Summarization: Generate concise summaries of progress, risks, highlights, and next steps.  - Multi-Format Export: Export reports as PDF, Word, HTML, with optional JSON for Finance/Operations. 
- Report Scheduling & Automation: Schedule weekly, monthly, or ad-hoc report generation via Mailing service integrations.
- Version Control & Audit: Maintain version history and track changes for compliance and accountability


## Design Thinking

Design thought involves looking into the below areas -

#### Major Tech Stack involved -

➤ Frontend UI: React (latest) with TypeScript for a modern, maintainable UI. ➤ Backend Microservices: .NET 8 and ASP.NET Core for high-performance Web APIs. ➤ Azure AI & Data: Azure Data Foundry OpenAI (agentic), DevOps, REST APIs, Azure SQL

➤ Azure Functions: .NET 8 with Scheduler and Email Integration. ➤ Mail Service: Third party Emal services like SendGrid or any other SMTP mail server. ➤ Infrastructure: Azure App Services, Azure Functions, Azure SQL, Terraform, and GitHub.

#### Scalability & Performance

The architecture supports horizontal scaling of APIs and serverless components.

UI component Lay loads, Caching and asynchronous processing reduce latency and external API load.

Multiple Open API endpoint support for different boards (currently Azure), templates and categories of reports

The design is suitable for enterprise-scale workloads, multiple service plans and future multi-region deployment.

#### Security Checks

The platform uses Azure AD JWT-based authentication with HTTPS and TLS encryption.

Authorization code flow with PKCE integrations for SPA React UI login for security standards.

Secrets and API keys are centrally managed using Azure Key Vault.

Stateless services enable secure horizontal scaling without session persistence.

#### Future Enhancements:

Planned enhancements include multi-AI API provider support, with additional models to serve, filter and process the project data and generate the report with different categories of details like Summary, Statistics, Graphs, Bar charts.

Advanced report analytics, and report scheduling can be further customized based on user needs as periodic enhancement activities.

Additional integrations with Jira, GitHub, and other tools are supported through extensible connectors.

Long-term goals include predictive analytics, compliance reporting, and enterprise-grade security and code quality integration features.


## Execution Strategy

The MVP implementation plan focuses on validating the core reporting workflow with minimal but complete functionality, as mentioned below:

➤ AI Report Portal to log in via SSO or AAD popup-based Authentication (App registrations) . ➤ Azure Boards Integration Module for fetching the ADO data and relevant project items.

➤ Azure OpenAI API report generation and a react JS based web UI. ➤ Azure Function for scheduling and emailing AI-generated ADO reports periodically to the recipient team group. ➤ Azure DevOps integration for deployment of the React web app and backend API, AI services to Azure using CICI Pipelines and IaC like Terraform. ➤ This establishes a strong technical foundation for other future enhancements which can be planned post MVP completion.

### **Design Diagram **
The solution follows a **layered, microservices-inspired architecture** even in a single deployment:

```
┌─────────────────────────────────────────────────────────────┐
│                    React Frontend (SPA)                      │
│          (Component-based, State Management, Routing)        │
└─────────────────────────────────┬───────────────────────────┘
                                  │
        ┌─────────────────────────┼─────────────────────────┐
        │                         │                         │
        ▼                         ▼                         ▼
┌──────────────────┐    ┌──────────────────┐    ┌──────────────────┐
│  Auth Service    │    │  Report Service  │    │  AI Agent Service│
│  (WebAPI)        │    │  (Function App)  │    │  (Web API)       │
│                  │    │                  │    │                  │
│ • JWT Generation │    │ • Scheduling     │    │ • Azure Connector│
│ • Token Validate │    │ • EmailAPIReport │    │ • Azure Open API |
│                  │    │                  │    │ • Prompt Response│
└──────────────────┘    └──────────────────┘    └──────────────────┘
        │                         │                         │
        └─────────────────────────┼─────────────────────────┘
                                  │
                    ┌─────────────────────────┐
                    │   Shared Infrastructure │
                    │   • App Configs         │
                    │   • App sevices         |
                    |     • Fn Apps sevices   |
                    │   • Logging             │
                    └─────────────────────────┘
```

#### **Key Design Principles**

| Principle | Implementation |
|-----------|-----------------|
| **Modularity** | Three independent .NET services + React frontend (can scale independently) |
| **Statelessness** | API services are stateless, enabling horizontal scaling |
| **Security-First** | JWT auth, Bearer tokens, Config encryption, API key abstraction |
| **Extensibility** | Plugin-based AI connectors (currently Azure OpenAI, extensible to others) |
| **Cloud-Native** | Terraform IaC, serverless Function App, Static website hosting |
| **Resilience** | Error handling, retry logic, graceful degradation |

**Deliverables**:
- ✅ Authentication system (JWT-based)
- ✅ Azure DevOps connector (read projects, work items)
- ✅ Azure OpenAI integration (basic prompts)
- ✅ Report generation as formatted text + HTML (.md)
- ✅ Basic UI for project selection and report viewing
- ✅ Send Project AI report as Mail attachment

# aipjm - AI Project Management Report

**Document Version**: 1.0  
**Last Updated**: December 2025  
**Author**: AI Project Reporting Team ( IntelliTrace Team )

## Executive Summary

aipjm is an enterprise-grade, AI-powered project reporting interface designed to leverage Azure Agentic AI capabilities. The platform automates the generation of intelligent project reports, insights, and analytics, significantly reducing manual effort while improving decision quality. aipjm demonstrates a robust, scalable approach to AI-driven project intelligence for generating reports with several custom aspects, including summarized progress, risks, highlights, and next steps. The MVP validates the business value and technical feasibility of the solution. The platform is well-positioned for enterprise adoption and future expansion. By leveraging Azure OpenAI’s agentic capabilities and modern cloud-native architecture, the solution delivers real-time, actionable project intelligence.

## Base Project Structure

```
aipjm/APPS
├── ReactAPPUI/          # React frontend (Create React App)
│   ├── src/
│   │   ├── components/       # React components (Login, Home, Header,axios)
│   │   ├── App.tsx           # Main app with routing
│   │   └── index.tsx         # Entry point
│
└── ReportingService/        # .NET 8 Web Function APP 
    ├── ReportingFunctions.cs/   # Reporting Schecular and APi for sending report 
    ├── Program.cs           # FunctionApp configuration
    ├── host.json            # JWT App settings


└── AgenticAIservice/        # .NET 8 Web API for Azure Open Agentic AI  Service 
    ├── Controllers/          # Azure AI API endpoints
    ├── Models/               # Data models
    ├── Program.cs            # ASP.NET Core configuration
    ├── appsettings.json      # Agentic AAI and Azure Project settings for project configurations

└── WebApiService/        # .NET 8 Web API for backend business and Authentications 
    ├── Controllers/          # API endpoints
    ├── Models/               # Data models
    ├── Services/             # JwtService for token generation
    ├── Program.cs            # ASP.NET Core configuration
    ├── appsettings.json      # JWT and app settings
```
#### Agentic AI Report Generation : AgenticAIService**

**Processing Flow**:
```
1. Receive: { projectKeyId, userPrompt, requestType }
   ↓
2. Load: Azure DevOps config for project
   ↓
3. Query: Fetch work items, epics, sprints from Azure DevOps
   ↓
4. Aggregate: Summarize data (blockers, velocity, timeline)
   ↓
5. Prompt: Construct AI prompt with context + user request
   ↓
6. Generate: Call Azure OpenAI Agentic API
   ↓
7. Format: Apply template (HTML/Markdown/Text)
   ↓
8. Return: Report string to caller or Send Mail
```

#### Report Download and Send Report Service
```
- Generaated Report on avaible on UI brower in multiple tabs to view.
- On successful generaationof report , **downlaod** and **send report** buttons are enabled on browser.
- On click of Download button , Report rendered  on browser in all the  tabs is consolidated into single report ( currenlty HTML formmat ) and gets downloaded on the browswer.
- On click of Send Report button , Report rendered  on browser in all the  tabs is consolidated into single report ( currenlty HTML formmat ) and send as e-mail attachment to the logged in recipient (and other backend configured recipient mails)..

```

#### Scheduled Reporting Service: Azure Function App**
```csharp
// Purpose: Scheduled reports, email integration
ReportingFunctions.cs               // Scheduled trigger
├── Timer trigger: "0 0 8 * * MON" (8 AM Mondays)
├── Http trigger API enpoint (for manual)
├── Fetch reports and projects data 
├── Services/MailerService.cs 
└── Send via SendGrid/SMTPas per request or schdule (Mail service configurable service)
```


### Flow Diagrams

#### **Report Generation Flow**

###### A) MANUAL FLOW 

```
┌──────────────────────────────────────────────────────────────┐
│ User Logs in the portal        │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ User selects or sets project & generate reports              │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│  React sends POST /api/Agent/<AIAgentServiceEndoint>         │
│    Payload: AzurePromptRequest { config, userPrompt, ...}    │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ WebAPI validates JWT token, forwards to AgenticAIService  │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│  AgenticAIService executes:                                  │
│    connects Query Azure DevOps API for project work items    │
│    extracts relevant data in well strucctured objects        │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│  AI AGent service creates custom query prompts from          │
│   extracted ADO dataa nd sends tp Agentic Open AI            │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│  Invoke Azure OpenAI Agentic API                             │
│    Model: sets GPT-4 (Multi-turn agent)                      │
│    Response: Structured report with analysis & insights      │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│  Formats Response in UI:                                     │
│    - HTML with CSS styling (for UI display)                  │
│    - Markdown for export                                     │
└────────────────────────┬─────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ Return to React UI, update state, display report             │
└──────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────┐
│ UI Options enabled : Send mail or Download Report as Html    │
└──────────────────────────────────────────────────────────────┘

```
B) Automated Report Generation and Email Scheduled via Azure Function App Run : 

```
Function App contains a Timer function - ReportSchedular which is currently runs every 1st day of week. (configurable)

The scheduled run settings is ("0 0 8 * * 1") i.e run every Monday at 08:00:00 AM

This function extracts AI generated reports for each of the Azure DevOps projects configured in backend AgenticAIService appsettings.json
and sends them as attchement in the mail via Send Grid mailer (to configured ecipients set in uncction app environment settings).

```

#### **Authentication Flow**

A) Local User Login
```
┌─────────────────────────────┐
│ 1. User submits credentials │
│    (username, password)      │
└────────────┬────────────────┘
             │
             ▼
┌─────────────────────────────────────────────┐
│ 2. React POST /api/auth/ValidateUser        │
│    (HTTPS, no auth needed yet)              │
└────────────┬────────────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────┐
│ 3. WebAPI validates credentials             │
│    (DB lookup or LDAP/AAD integration)       │
└────────────┬─────────────────────────────────┘
             │
             ▼
┌───────────────────────────────────────────────┐
│ 4.  Generate JWT token                        │
│    Payload: { username, role etc. }           │
│                                               │
└────────────┬──────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────┐
│ 5. Return JWT to React ui              │
│    Store in localStorage               │
└────────────┬───────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────┐
│ 5. ON AUTH Success : Loads Home page     │
└──────────────────────────────────────────┘

```
B) Azure AD Login (Winwire)

```
React Ui app is also integrated with Winbuild registered Azure AD  , that alloows user to login via their winwire account credwentials.  

┌─────────────────────────────┐
│ 1.Select Azure AD Login     │
│    (Winwire creddentials)   |  
└────────────┬────────────────┘
             │
             ▼
┌─────────────────────────────────────────────┐
│ 2. AAD Server Autentication  Validation     │
└────────────┬────────────────────────────────┘
             │
             ▼
┌───────────────────────────────────────────────┐
│ 4. on Success return AAD JWT token            │
│    Payload: acccesstoken                      |
|   { username,email, role etc. }               │
│                                               │
└────────────┬──────────────────────────────────┘
             │
             ▼
┌────────────────────────────────────────┐
│ 5. Return JWT to React ui              │
│    Store in localStorage               │
└────────────┬───────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────┐
│ 5. Loads Home page                       │
└──────────────────────────────────────────┘

```