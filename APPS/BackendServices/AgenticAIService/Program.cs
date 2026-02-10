using AgenticAIService.AIServices;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Bind AgenticAI options from configuration (appsettings or environment)
builder.Services.Configure<AgenticAIOptions>(builder.Configuration.GetSection("AgenticAIAzureBoard"));

// Register named HttpClient for AgenticAI calls
builder.Services.AddHttpClient("AgenticAIAzureBoard")
    .ConfigureHttpClient((sp, client) =>
    {
        var opts = sp.GetRequiredService<IOptions<AgenticAIOptions>>().Value;
        if (!string.IsNullOrWhiteSpace(opts.Endpoint))
        {
            client.BaseAddress = new Uri(opts.Endpoint);
        }
        // don't set authorization header here; controller will set header per-request
    });



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Add Services 
builder.Services.AddScoped<IAzureOpenAIAzureBoardQueryService, AzureOpenAIAzureBoardQueryService>();
//builder.Services.AddScoped<IAzureOpenAIReportService, AzureOpenAIReportService>();


// Add CORS policy to allow requests from React frontend (localhost:3000 by default)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgenticAI API", Version = "v1" });
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgenticAI API V1"));
//}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowReactApp"); 


app.UseAuthorization();
app.MapControllers();

app.Run();


