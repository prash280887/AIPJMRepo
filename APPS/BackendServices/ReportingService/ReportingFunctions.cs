using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReportingService.Models;
using ReportingService.Services;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Markdig;

namespace ReportingService;

public class ReportingFunctions
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger _logger;
    private readonly IConfiguration _config;
    private readonly IMailerService _mailerService;

    public ReportingFunctions(IHttpClientFactory httpFactory, ILogger<ReportingFunctions> logger, IConfiguration config, IMailerService mailerService)
    {
        _httpFactory = httpFactory;
        _logger = logger;
        _config = config;
        _mailerService = mailerService;
    }

    // ------------------------
    // TIMER TRIGGER send report
    // ------------------------
    [Function("ReportSchedular")]
    public async Task Run([TimerTrigger("0 0 8 * * 1")] TimerInfo myTimer) //0 0 8 * * 1 every Monday = 1 , at 8 :00 UTC mins
    {
        _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);

        List<ProjectInfo> projects = await FetchAzureProjectsAsync();
    
        _logger.LogInformation("Schedular projects fetched : " +  projects.Count().ToString());
        foreach (var projectInfo in projects)
        {
            _logger.LogInformation("Schedular projects fetched Info : " + projectInfo?.OrgUrl + "/" + projectInfo?.ProjectName);

                string[] ReportTypes = new string[] { "SUMMARY", "DETAILED_TASK_ANALYSIS", "RISKS_AND_BLOCKERS", "RECOMMENDATIONS" };
                var reportData = new ReportData();
                reportData.projectTitle = string.IsNullOrEmpty(projectInfo.ProjectDisplayName) ? projectInfo.ProjectName : projectInfo.ProjectDisplayName;
                AzurePromptRequest promptRequest = null;

                foreach (string reportType in ReportTypes)
                {
                    promptRequest = new AzurePromptRequest
                    {
                        azureBoardsConfig = new AzureBoardConfig
                        {
                            OrgUrl = projectInfo?.OrgUrl,
                            PersonalAccessToken = projectInfo?.PersonalAccessToken,
                            ProjectName = projectInfo?.ProjectName,
                            ProjectDisplayName = projectInfo?.ProjectDisplayName,
                            ProjectBoardType = projectInfo?.ProjectBoardType,
                            ProjectKeyId = projectInfo?.ProjectKeyId
                        },
                        UserPrompt = "",
                        RequestType = reportType,
                        MaxTokens = 2000
                    };

                    switch (reportType)
                    {
                        case "SUMMARY":
                            reportData.projectsMessage = await GetReportData(promptRequest);
                            break;
                        case "DETAILED_TASK_ANALYSIS":
                            reportData.projectsMessageHtmlTasks = await GetReportData(promptRequest);
                            break;
                        case "RISKS_AND_BLOCKERS":
                            reportData.projectsMessageHtmlRisks = await GetReportData(promptRequest);
                            break;
                        case "RECOMMENDATIONS":
                            reportData.projectsMessageHtmlRecommendations = await GetReportData(promptRequest);
                            break;
                        default:
                            reportData.projectsMessage = await GetReportData(promptRequest);
                            break;
                    }
                }

                string reportContent = PrepareReportHtmlData(reportData);
                MailRequest scheduledMailRequest = new MailRequest()
                {
                    Attachments = new MailAttachment[]
                    {
                    new MailAttachment
                    {
                        ContentsAsAttachment = reportContent,
                        FileName = $"{reportData.projectTitle}_Report_{DateTime.UtcNow:yyyyMMdd}.html",
                        MimeType = "text/html"
                    }
                    },
                    Content = "Please find the attached project report.",
                    IsHtml = true,
                    PromptRequest = promptRequest
                };
                Thread.Sleep(60000);
                await SendReportMail(scheduledMailRequest);

                if (myTimer.ScheduleStatus is not null)
                {
                    _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
                }
            }        
    }

    // ----------------------------------------
    // NEW: HTTP TRIGGER to manually send report
    // ----------------------------------------

    [Function("SendReportOnDemand")]
    public async Task<HttpResponseData> SendOnDemand(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "send-report")]
    HttpRequestData req)
    {
        _logger.LogInformation("HTTP trigger received");

        MailRequest? mailRequest;

        try
        {
            mailRequest = await JsonSerializer.DeserializeAsync<MailRequest>(
                req.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (mailRequest == null)
            {
                throw new ArgumentNullException(nameof(mailRequest));
            }

            _logger.LogInformation(
                "mail send Request  received: {project}",
                mailRequest.PromptRequest?.azureBoardsConfig?.ProjectName);
           
            //for UI send report - mail has attachment 
          
            MailRequest sendMailRequest = new MailRequest()
            {
                Subject = mailRequest.Subject,
                Attachments = mailRequest?.Attachments,
                Content = mailRequest.Content,
                IsHtml = mailRequest.IsHtml,
                To = mailRequest?.To,
                Cc = mailRequest?.Cc,
                Bcc = mailRequest?.Bcc,
                PromptRequest = mailRequest.PromptRequest
            };
            await SendReportMail(sendMailRequest);

            var resp = req.CreateResponse(HttpStatusCode.OK);
            await resp.WriteStringAsync(
                $"Report processed successfully for project: {mailRequest.PromptRequest?.azureBoardsConfig.ProjectName}");

            AddCorsHeaders(req, resp);
            return resp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running report via HTTP trigger");

            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            await resp.WriteStringAsync($"Failed to process report: {ex.Message}");
            AddCorsHeaders(req, resp);
            return resp;
        }
    }

    //[Function("RunReportOnDemand")]
    //public async Task<HttpResponseData> RunOnDemand(
    //    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "run-report")]
    //    HttpRequestData req)
    //{
    //    _logger.LogInformation("HTTP trigger received");

    //    AzurePromptRequest? promptRequest;

    //    try
    //    {
    //        promptRequest = await JsonSerializer.DeserializeAsync<AzurePromptRequest>(
    //            req.Body,
    //            new JsonSerializerOptions
    //            {
    //                PropertyNameCaseInsensitive = true
    //            });

    //        if (promptRequest == null)
    //        {
    //            throw new ArgumentNullException(nameof(promptRequest));
    //        }

    //        _logger.LogInformation("Project received: {project}",promptRequest.azureBoardsConfig.ProjectName);

    //         string reportContent = await GetReportData(promptRequest);
    //            MailRequest mailRunRequest = new MailRequest()
    //            {
    //                PromptRequest = promptRequest,

    //            };

    //           await SendReportMail(mailRunRequest);

    //          var resp = req.CreateResponse(HttpStatusCode.OK);
    //          await resp.WriteStringAsync($"Report processed successfully for project: {promptRequest.azureBoardsConfig.ProjectName}");

    //        AddCorsHeaders(req, resp);
    //        return resp;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error running report via HTTP trigger");

    //        var resp = req.CreateResponse(HttpStatusCode.BadRequest);
    //        await resp.WriteStringAsync($"Failed to process report: {ex.Message}");
    //        AddCorsHeaders(req, resp);
    //        return resp;
    //    }
    //}



    private async Task SendReportMail(MailRequest mailRequest)
    {
        try
        {
             await _mailerService.SendEmailWithAttachmentAsync(mailRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error Sending mail report");
            throw;
        }
    }

    private async Task<string> GetReportData(AzurePromptRequest promptRequest)
    {
        try
        {
            var baseUrl = _config["AgenticAIService_BaseUrl"]?.TrimEnd('/')
                ?? throw new InvalidOperationException("AgenticAIService_BaseUrl not configured");

            var client = _httpFactory.CreateClient();

            var url = $"{baseUrl}/api/Agent/GetAzOpenAIAzureBoardRuntimeResponse";
            _logger.LogInformation("Calling {url}", url);

            var response = await client.PostAsJsonAsync(url, promptRequest);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return content;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running scheduled report");
            throw;
        }
    }

private string SanitizeHtml(string html)
{
    return System.Text.RegularExpressions.Regex.Replace(html, @"```html\s*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        .Replace("```", "");
}

    private string PrepareReportHtmlData(ReportData reportData)
    {
        var projectTitle = reportData.projectTitle;
        var bodyMessageMarkdown = Markdown.ToHtml(reportData.projectsMessage); 
        var bodyMessageHtml =  SanitizeHtml(reportData.projectsMessageHtml);
        var bodyMessageHtmlTasks = SanitizeHtml(reportData.projectsMessageHtmlTasks);
        var bodyMessageHtmlRisks = SanitizeHtml(reportData.projectsMessageHtmlRisks);
        var bodyMessageHtmlRecommendations = SanitizeHtml(reportData.projectsMessageHtmlRecommendations);

        var fullHtml = $@"<!DOCTYPE html>
            <html>
            <head>
              <meta charset=""UTF-8"" />
              <title>Markdown Export</title>
              <style>
                body {{ font-family: Arial, sans-serif; padding: 24px; }}
                pre {{ background: #f5f5f5; padding: 12px; }}
                code {{ background: #eee; }}
              </style>
            </head>
            <body>
              <h2>{projectTitle}</h2>
              {bodyMessageMarkdown}
              <hr />
              {bodyMessageHtml}
              <hr />
              {bodyMessageHtmlTasks}
              <hr />
              {bodyMessageHtmlRisks}
              <hr />
              {bodyMessageHtmlRecommendations}
            </body>
            </html>";

        return fullHtml;
    }

    private async Task<List<ProjectInfo>> FetchAzureProjectsAsync()
    {
        try
        {
            var baseUrl = _config["AgenticAIService_BaseUrl"]?.TrimEnd('/')
                ?? throw new InvalidOperationException("AgenticAIService_BaseUrl not configured");

            var client = _httpFactory.CreateClient();

            var url = $"{baseUrl}/api/Agent/GetAzOpenAIAzureBoardProjects";
            _logger.LogInformation("Fetching projects from {url}", url);

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // FIX: Use System.Net.Http.Json for deserialization instead of ReadAsAsync
            var projects = await response.Content.ReadFromJsonAsync<List<ProjectInfo>>();
            _logger.LogInformation("Successfully fetched {count} projects", projects?.Count ?? 0);

            return projects ?? new List<ProjectInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Azure projects");
            throw;
        }
    }

    private void AddCorsHeaders(HttpRequestData request, HttpResponseData response)
    {
    var allowedOrigins = new[]
        {
        "http://localhost:3000",
        "http://localhost:3001",
        "https://localhost:3000",
        "https://localhost:3001"
    };

        if (request.Headers.TryGetValues("Origin", out var origins))
        {
            var origin = origins.FirstOrDefault();

            if (allowedOrigins.Contains(origin))
            {
                response.Headers.Add("Access-Control-Allow-Origin", origin);
            }
        }

        response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
    }

}
