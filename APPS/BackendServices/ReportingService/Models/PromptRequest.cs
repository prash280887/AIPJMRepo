namespace ReportingService;

public class AzurePromptRequest
{
    public AzureBoardConfig azureBoardsConfig { get; set; }
    public string? UserPrompt { get; set; }

    public string? RequestType { get; set; } 
    public int? MaxTokens { get; set; }
}
