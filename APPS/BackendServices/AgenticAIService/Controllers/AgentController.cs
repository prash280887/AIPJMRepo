using AgenticAIService.AIServices;
using AgenticAIService.Models;
using AgenticAIService.Models.Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Writers;
using AgenticAIService.Connectors;
using System.Net.Http.Headers;

namespace AgenticAIService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly AgenticAIOptions _options;
    private readonly IConfiguration _configuration;
    private readonly IAzureOpenAIAzureBoardQueryService _agentAIQueryService;

    public AgentController(IHttpClientFactory httpFactory, IOptions<AgenticAIOptions> options,
       IAzureOpenAIAzureBoardQueryService agentAIQueryService ,
      IConfiguration configuration )
    {
        _httpFactory = httpFactory;
        _options = options.Value;
        _agentAIQueryService = agentAIQueryService;
        _configuration = configuration;
    }


    [HttpPost("GetAzOpenAIAzureBoardRuntimeResponse")]
    public async Task<IActionResult> GetAzOpenAIAzureBoardRuntimeResponse([FromBody] AzurePromptRequest promptRequest)
    {
        try
        {
            string content = await _agentAIQueryService.getAzureBoardRuntimeResponse(promptRequest).ConfigureAwait(false);

            return await Task.FromResult<IActionResult>(Ok(content)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return await Task.FromResult<IActionResult>(StatusCode(500, new { error = "internal_error", details = ex.Message })).ConfigureAwait(false);
        }
    }


    [HttpGet("GetAzOpenAIAzureBoardProjects")]
    public async Task<List<AzureBoardConfig>> GetAzOpenAIAzureBoardProjects()
    {
        var section = _configuration.GetSection("AzureBoardConnector");
        List<AzureBoardConfig>? connectors = section.Get<List<AzureBoardConfig>>();

        if (connectors == null || connectors.Count == 0)
        {
            connectors = section.GetChildren()
                .Select(c => new AzureBoardConfig
                {
                    OrgUrl = c.GetValue<string>("orgUrl") ?? c.GetValue<string>("OrgUrl"),
                    PersonalAccessToken = "", //dont bind
                    ProjectName = c.GetValue<string>("projectName") ?? c.GetValue<string>("ProjectName"),
                    ProjectDisplayName = c.GetValue<string>("projectDisplayName") ?? c.GetValue<string>("ProjectDisplayName"),
                    ProjectBoardType = c.GetValue<string>("projectBoardType") ?? c.GetValue<string>("ProjectBoardType"),
                    ProjectKeyId = c.GetValue<string>("projectKeyId") ?? c.GetValue<string>("ProjectKeyId"),
                })
                .Where(cfg => !string.IsNullOrWhiteSpace(cfg.OrgUrl) && !string.IsNullOrWhiteSpace(cfg.PersonalAccessToken) && !string.IsNullOrWhiteSpace(cfg.ProjectName))
                .ToList();
        }

        if (connectors == null || connectors.Count == 0)
        {
            throw new InvalidOperationException("AzureBoardConnector configuration is missing or empty. Expecting an array with at least one entry.");
        }

       return connectors;

    }
       
}
