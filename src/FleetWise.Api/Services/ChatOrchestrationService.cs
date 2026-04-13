using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using FleetWise.Api.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FleetWise.Api.Services;

public class ChatOrchestrationService(Kernel kernel, ILogger<ChatOrchestrationService> logger) : IChatOrchestrationService
{
    private const string SystemPrompt =
        """
        You are FleetWise AI, an intelligent fleet management assistant for a municipal vehicle fleet.
        You have access to real-time fleet data through function calling. ALWAYS use your available
        functions to query actual data before answering -- never guess or fabricate fleet information.
        Be concise, professional, and helpful. When presenting data, format it clearly.
        If a user asks a follow-up question, use context from the conversation to understand what
        they are referring to.
        """;

    // Conversation state -- keyed by conversation ID, holds the full chat history
    // so follow-up questions like "Which of those are diesel?" work correctly.
    private static readonly ConcurrentDictionary<string, ChatHistory> Conversations = new();

    public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request)
    {
        var conversationId = request.ConversationId ?? Guid.NewGuid().ToString();
        var history = Conversations.GetOrAdd(conversationId, _ =>
        {
            var h = new ChatHistory();
            h.AddSystemMessage(SystemPrompt);
            return h;
        });

        history.AddUserMessage(request.Message);

        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        logger.LogInformation("Processing message for conversation {ConversationId}", conversationId);

        var response = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings,
            kernel);

        history.AddAssistantMessage(response.Content ?? string.Empty);

        // Extract which functions the LLM called during this turn by inspecting
        // the chat history for FunctionCallContent items added by SK.
        var functionsUsed = history
            .SelectMany(m => m.Items.OfType<FunctionCallContent>())
            .Select(fc => fc.FunctionName)
            .Distinct()
            .ToList();

        logger.LogInformation("Conversation {ConversationId}: used functions [{Functions}]",
            conversationId, string.Join(", ", functionsUsed));

        return new ChatResponse
        {
            Response = response.Content ?? string.Empty,
            ConversationId = conversationId,
            FunctionsUsed = functionsUsed
        };
    }

    public async IAsyncEnumerable<string> StreamMessageAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Streaming implementation will be added in Step 6
        var response = await ProcessMessageAsync(request);
        yield return response.Response;
    }
}
