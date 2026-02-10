// Install from the NuGet package manager or command line:
// dotnet add package OpenAI

using AgenticAIService.Connectors;
using AgenticAIService.Models;
using AgenticAIService.Models.Azure;
using Microsoft.Extensions.Options;
using Microsoft.Test.VariationGeneration;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Text;
using Xunit.Sdk;

namespace AgenticAIService.AIServices
{
    public class AzureOpenAIAzureBoardQueryService : IAzureOpenAIAzureBoardQueryService
    {
        private readonly AgenticAIOptions _options;
        private readonly IConfiguration _configuration;

        public AzureOpenAIAzureBoardQueryService(IOptions<AgenticAIOptions> options, IConfiguration configuration)
        {
            _options = options.Value;
            _configuration = configuration;
        }

        public async Task<string> getResponse(string prompt)
        {
            return await ReturnAIResponse(prompt).ConfigureAwait(false);
        }

        public async Task<string> getAzureBoardRuntimeResponse(AzurePromptRequest promptRequest)
        {
            var cfg = getAzureBoardConfiguration(promptRequest);
            string orgUrl = cfg.OrgUrl ?? throw new InvalidOperationException("AzureBoardConnector:OrgUrl is not configured.");
            string personalAccessToken = cfg.PersonalAccessToken ?? throw new InvalidOperationException("AzureBoardConnector:PersonalAccessToken is not configured.");
            string projectName = cfg.ProjectName ?? throw new InvalidOperationException("AzureBoardConnector:ProjectName is not configured.");

            AzureBoardConnector abc = new AzureBoardConnector(
                orgUrl,
                personalAccessToken,
                projectName
                );

            var model = await abc.GetAllWorkItemsAsync();

            if (model == null || model.Count == 0)
            {
                return "No work items found.";
            }

            string prompt = promptBuilder(promptRequest, model);
            return await ReturnAIResponse(prompt).ConfigureAwait(false);
        }

       private AzureBoardConfig getAzureBoardConfiguration(AzurePromptRequest promptRequest)
        {
            var cfg = new AzureBoardConfig();
            string projectKeyId = promptRequest.azureBoardsConfig.ProjectKeyId ?? throw new InvalidOperationException("AzureBoardConnector:ProjectKeyId is not configured.");

            if (!string.IsNullOrWhiteSpace(projectKeyId))  //has value
            {
                var section = _configuration.GetSection("AzureBoardConnector");
                List<AzureBoardConfig>? connectors = section.Get<List<AzureBoardConfig>>();

                if (connectors == null || connectors.Count == 0)
                {
                    connectors = section.GetChildren()
                        .Select(c => new AzureBoardConfig
                        {
                            OrgUrl = c.GetValue<string>("orgUrl") ?? c.GetValue<string>("OrgUrl"),
                            PersonalAccessToken = c.GetValue<string>("personalAccessToken") ?? c.GetValue<string>("PersonalAccessToken"),
                            ProjectName = c.GetValue<string>("projectName") ?? c.GetValue<string>("ProjectName"),
                            ProjectDisplayName = c.GetValue<string>("projectDisplayName") ?? c.GetValue<string>("ProjectDisplayName"),
                            ProjectBoardType = c.GetValue<string>("projectBoardType") ?? c.GetValue<string>("ProjectBoardType"),
                            ProjectKeyId = c.GetValue<string>("projectKeyId") ?? c.GetValue<string>("ProjectKeyId"),
                        })
                        .Where(cfg => !string.IsNullOrWhiteSpace(cfg.OrgUrl)
                        && !string.IsNullOrWhiteSpace(cfg.PersonalAccessToken)
                        && !string.IsNullOrWhiteSpace(cfg.ProjectName)
                        && !string.IsNullOrWhiteSpace(cfg.ProjectKeyId))
                        .ToList();
                }

                if (connectors == null || connectors.Count == 0)
                {
                    throw new InvalidOperationException("AzureBoardConnector configuration is missing or empty. Expecting an array with at least one entry.");
                }

                // Use the first connector entry
                cfg = connectors?.Where(x => x.ProjectKeyId == projectKeyId).FirstOrDefault();
            }
            else
            {
                cfg = promptRequest.azureBoardsConfig;
            }
            return cfg;
        }

        private Task<string> ReturnAIResponse(string prompt)
        {
            try
            {
                ChatClient client = new(
                    credential: new ApiKeyCredential(_options.ApiKey),
                    model: "gpt-4o",
                    options: new OpenAIClientOptions()
                    {
                        Endpoint = new($"{_options.Endpoint}"),
                    });

                ChatCompletion completion = client.CompleteChat(new[] { new UserChatMessage(prompt) });

                var sb = new StringBuilder();
                foreach (ChatMessageContentPart contentPart in completion.Content)
                {
                    sb.Append(' ').Append(contentPart.Text);
                }

                return Task.FromResult(sb.ToString());
            }
            catch(Exception ex)
            {
                throw ex; 
            }
        }

        private string promptBuilder(AzurePromptRequest promptRequest, List<AzureBoardWorkItem> model)
        {
            string requestType = promptRequest?.RequestType;
            string userPrompt = promptRequest?.UserPrompt;
            string prompt = "";
            string workItemdata = getWorkItemDataContent(model);
            switch (requestType)
            {
                case "SUMMARY":
                    prompt = $@"
            Summarize the following project details and progress data for Project Management Reporting based on below Azure work items:
            Items: {workItemdata}            
            Provide:
            - Progress Summary
            - Key Highlights
            - Estimated Timeline for workitem Completion (calculate)
            - Risks
            - Recommendations
            - Next Steps
            ";
                    break;

                case "HTML_STATS":
                    prompt = $@"Refer Azure Dev Ops project data below -  
                Work Items Data : 
                {workItemdata}
                
                Total hours: 
                Generate project response with below statistics in HTML format.
                Provide:                           
                    - Progress Summary with HTML formatting
                    - Work Items Statistics as Pie Chart showing Work Item Status Distribution
                    - Timeline Estimation with Gantt Chart
                    - Completed vs Estimated Hours Assignments in Gantt Chart
                    - Total Story Points in each State as Pie Chart
                    - Epic, Features, UserStories as Pie Chart
                   
              Add Graphs and Charts in HTML format for better visualization with proper styling.
              Place 2 graphs side by side for better readability.
                NOTE: Only return response starting with HTML tag with formatted styling and no other previous text as this response is bound to UI dynamically.
            ";
                    break;

                case "DETAILED_TASK_ANALYSIS":
                    prompt = $@"Generate project response with below statistics in HTML format.
            Provide a detailed analysis of each work item for the project:

            Work Items: {workItemdata}

            Provide:
            - Task-wise completion status
            - Tasks pending or delayed
            - Assigned work analysis
            - Effort estimation vs actual hours
            - Any bottlenecks or delays

             NOTE: Only return response starting with HTML tag with formatted styling and no other previous text as this response is bound to UI dynamically.
            ";
                    break;

                case "RISKS_AND_BLOCKERS":
                    prompt = $@"
            Analyze the project work items to identify detailed risks and blockers:

            Work Items:
            {workItemdata}

            Provide:
            - Potential risks for each work item
            - Blockers preventing progress
            - Severity of risk (High/Medium/Low)
            - Suggested mitigation strategies
            - Recommendations to unblock tasks

             NOTE: Only return response starting with HTML tag with formatted styling and no other previous text as this response is bound to UI dynamically.
            ";
                    break;

            case "RECOMMENDATIONS":
                    prompt = $@"Generate project response with below statistics in HTML format.
            Based on the project work items analysis, provide detailed strategic recommendations and next steps:

            Work Items:
           {workItemdata}

            Provide:
            - Short-term recommendations (1-2 weeks)
            - Medium-term recommendations (2-4 weeks)
            - Long-term recommendations (1+ months)
            - Priority order for next tasks
            - Resource optimization suggestions
            - Timeline adjustments if needed
            - Team capacity planning recommendations

             NOTE: Only return response starting with HTML tag and no other previous text as this response is bound to UI dynamically.
            ";
                    break;

                default:
                    prompt = $@"
            Summarize the following project details and progress data for Project Management Reporting based on below Azure work items:

            Items: {workItemdata}
            Calculate Total Estimated hours of Work vs Total Compleeted Hours
            Estimated Timeline for workitem Completion
            Provide:
            - Progress Summary
            - Key Highlights
            - Risks
            - Recommendations
            - Next Steps
            ";
                    break;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("You are an AI assistant that responds to Azure DevOps projects related requests only");
            sb.AppendLine(prompt);
            sb.AppendLine(userPrompt);
            return sb.ToString();
        }

        private string getWorkItemDataContent(List<AzureBoardWorkItem> model)
        {   //convert to Stringbuilder
            string content = string.Join("\n", model.Select(x => $" Work Item Type : {x.WorkItemType} | Title : {x.Title} |  Status : {x.State} |  Completed Hours : {x.CompletedWork} | Original Estimate Hours : {x.OriginalEstimate} | Story Points : {x.StoryPoints} "));
            return content;
         }
    }

}