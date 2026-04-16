using System.ClientModel;
using FleetWise.Api.Plugins;
using FleetWise.Api.Services;
using FleetWise.Infrastructure.Data;
using FleetWise.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=fleetwise.db"));

// Repositories
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IPartRepository, PartRepository>();

// Vector store -- singleton because it holds all document embeddings in memory for the app lifetime
builder.Services.AddSingleton<InMemoryVectorStore>();

// AI Provider -- reads "AiProvider" from config to decide which LLM backend to use.
// All three providers use the OpenAI connector -- Ollama exposes an OpenAI-compatible API.
var aiProvider = builder.Configuration["AiProvider"] ?? "Ollama";

// Embedding generator -- registered on the app container (not just the Kernel) so that
// both the Kernel factory and the startup ingestion service can resolve it.
switch (aiProvider)
{
    case "Ollama":
        var ollamaEndpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434";
        var ollamaEmbeddingModel = builder.Configuration["Ollama:EmbeddingModel"] ?? "nomic-embed-text";
        var ollamaClient = new OpenAIClient(
            new ApiKeyCredential("ollama"),
            new OpenAIClientOptions { Endpoint = new Uri($"{ollamaEndpoint}/v1/") });
        builder.Services.AddOpenAIEmbeddingGenerator(
            modelId: ollamaEmbeddingModel,
            openAIClient: ollamaClient);
        break;
    case "AzureOpenAI":
        var azureEmbedEndpoint = builder.Configuration["AzureOpenAI:Endpoint"]
            ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is required");
        var azureEmbedModel = builder.Configuration["AzureOpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
        var azureEmbedKey = builder.Configuration["AzureOpenAI:ApiKey"]
            ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is required");
        builder.Services.AddAzureOpenAIEmbeddingGenerator(
            deploymentName: azureEmbedModel,
            endpoint: azureEmbedEndpoint,
            apiKey: azureEmbedKey);
        break;
    case "OpenAI":
        var openAiEmbedModel = builder.Configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
        var openAiEmbedKey = builder.Configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI:ApiKey is required");
        builder.Services.AddOpenAIEmbeddingGenerator(
            modelId: openAiEmbedModel,
            apiKey: openAiEmbedKey);
        break;
}

builder.Services.AddScoped(serviceProvider =>
{
    var kernelBuilder = Kernel.CreateBuilder();

    switch (aiProvider)
    {
        case "Ollama":
            var ollamaEndpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434";
            var ollamaModel = builder.Configuration["Ollama:ChatModel"] ?? "qwen2.5:7b";
            kernelBuilder.AddOpenAIChatCompletion(
                modelId: ollamaModel,
                apiKey: "ollama",  // Ollama ignores the API key, but the SDK requires a non-null value
                endpoint: new Uri($"{ollamaEndpoint}/v1/"));
            break;

        case "AzureOpenAI":
            var azureEndpoint = builder.Configuration["AzureOpenAI:Endpoint"]
                ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is required when AiProvider is AzureOpenAI");
            var azureModel = builder.Configuration["AzureOpenAI:ChatModel"] ?? "gpt-4o-mini";
            var azureApiKey = builder.Configuration["AzureOpenAI:ApiKey"]
                ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is required when AiProvider is AzureOpenAI");
            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: azureModel,
                endpoint: azureEndpoint,
                apiKey: azureApiKey);
            break;

        case "OpenAI":
            var openAiModel = builder.Configuration["OpenAI:ChatModel"] ?? "gpt-4o-mini";
            var openAiApiKey = builder.Configuration["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI:ApiKey is required when AiProvider is OpenAI");
            kernelBuilder.AddOpenAIChatCompletion(
                modelId: openAiModel,
                apiKey: openAiApiKey);
            break;

        default:
            throw new InvalidOperationException($"Unknown AiProvider: {aiProvider}. Valid values: Ollama, AzureOpenAI, OpenAI");
    }

    var kernel = kernelBuilder.Build();

    // Import plugins -- SK reads [KernelFunction] + [Description] attributes and
    // sends them to the LLM so it can decide which functions to call.
    var vehicleRepo = serviceProvider.GetRequiredService<IVehicleRepository>();
    kernel.ImportPluginFromObject(new FleetQueryPlugin(vehicleRepo), "FleetQuery");

    var maintenanceRepo = serviceProvider.GetRequiredService<IMaintenanceRepository>();
    kernel.ImportPluginFromObject(new MaintenancePlugin(maintenanceRepo, vehicleRepo), "Maintenance");

    var workOrderRepo = serviceProvider.GetRequiredService<IWorkOrderRepository>();
    var partRepo = serviceProvider.GetRequiredService<IPartRepository>();
    kernel.ImportPluginFromObject(new WorkOrderPlugin(workOrderRepo, partRepo), "WorkOrder");

    var embeddingService = serviceProvider.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
    var vectorStore = serviceProvider.GetRequiredService<InMemoryVectorStore>();
    kernel.ImportPluginFromObject(new DocumentSearchPlugin(embeddingService, vectorStore), "DocumentSearch");

    return kernel;
});

// Chat orchestration -- manages conversation history and function call tracking
builder.Services.AddScoped<IChatOrchestrationService, ChatOrchestrationService>();

builder.Logging.AddConsole();

// Controllers + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "FleetWise AI API", Version = "v1" });
});

// CORS for Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Seed data on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FleetDbContext>();
    SeedData.Initialize(context);
    var docsPath = Path.Combine(app.Environment.ContentRootPath, "..", "..", "data", "documents");

    if (Directory.Exists(docsPath))
    {
        var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        var vectorStore = scope.ServiceProvider.GetRequiredService<InMemoryVectorStore>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DocumentIngestionService>>();

        var ingestionService = new DocumentIngestionService(embeddingService, vectorStore, logger);
        await ingestionService.IngestDocumentsAsync(docsPath);
    }
    else
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning("Documents path not found: {DocsPath}. Skipping document ingestion.", docsPath);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FleetWise AI API v1");
    });
}

app.UseCors("AllowAngularDev");
app.MapControllers();

await app.RunAsync();

// Make the Program class accessible for integration tests
public partial class Program { }
