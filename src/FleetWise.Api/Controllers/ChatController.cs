using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FleetWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(Kernel kernel) : ControllerBase
{
    private const string SystemPrompt =
        """
        You are FleetWise AI, an intelligent fleet management assistant for a municipal vehicle fleet.
        You have access to real-time fleet data through function calling. ALWAYS use your available
        functions to query actual data before answering -- never guess or fabricate fleet information.
        Be concise, professional, and helpful. When presenting data, format it clearly.
        """;

    /// <summary>Send a message to the AI assistant and receive a response.</summary>
    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();
        history.AddSystemMessage(SystemPrompt);
        history.AddUserMessage(request.Message);

        var executionSettings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var response = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings,
            kernel);

        return Ok(new ChatResponse
        {
            Response = response.Content ?? string.Empty,
            ConversationId = Guid.NewGuid().ToString()
        });
    }
}

public class ChatRequest
{
    public required string Message { get; set; }
}

public class ChatResponse
{
    public required string Response { get; set; }
    public required string ConversationId { get; set; }
}
