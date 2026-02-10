using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportingService.Services;

var builder = FunctionsApplication.CreateBuilder(args);

// Enable HTTP triggers & middleware pipeline
builder.ConfigureFunctionsWebApplication();

// App Insights
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Add HttpClient support (fixes IHttpClientFactory DI errors)
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IMailerService, MailerService>();
// ⬇️ If you want to register typed HttpClient, example:
// builder.Services.AddHttpClient("jira", client =>
// {
//     client.BaseAddress = new Uri("https://your-jira-url.com");
// });

builder.Build().Run();
