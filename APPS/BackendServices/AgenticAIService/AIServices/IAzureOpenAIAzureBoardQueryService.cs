using AgenticAIService.Models;
using AgenticAIService.Models.Azure;

namespace AgenticAIService.AIServices
{
    public interface IAzureOpenAIAzureBoardQueryService
    {
        Task<string> getResponse(string prompt);


        Task<string> getAzureBoardRuntimeResponse(AzurePromptRequest promptRequest); 
    }
}
