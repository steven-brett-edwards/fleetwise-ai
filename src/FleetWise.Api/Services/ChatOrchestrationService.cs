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
        You also have access to fleet management documentation covering policies, procedures, and SOPs. 
        Use `search_fleet_documentation` for policy questions, how-to procedures, and compliance guidance. 
        Use your live data functions for questions about specific vehicles, work orders, costs, and fleet status. 
        Combine both when appropriate.
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

        logger.LogInformation(
            "Streaming message for conversation {ConversationId} (history depth: {Depth})",
            conversationId,
            history.Count);

        // GetStreamingChatMessageContentsAsync returns chunks as they arrive.
        // SK handles the function calling loop internally; tool calls execute
        // before text chunks start flowing. SK mutates the `history` object in
        // place as it runs the tool-call loop (appending assistant tool_calls
        // messages and tool results), but it does NOT append the final streamed
        // assistant text -- we accumulate that ourselves below.
        var chunks = SafeStreamChunksAsync(chatCompletionService, history, executionSettings, conversationId, cancellationToken);

        var fullResponse = new System.Text.StringBuilder();
        await foreach (var chunk in chunks.WithCancellation(cancellationToken))
        {
            fullResponse.Append(chunk);
            yield return chunk;
        }

        if (fullResponse.Length > 0)
        {
            history.AddAssistantMessage(fullResponse.ToString());
        }
    }

    // Wraps the streaming call so that any exception thrown by the LLM connector
    // (e.g. Groq rejecting a malformed tool-call history, rate limits, timeouts)
    // is logged with full context and surfaced to the client as a plain-text
    // error chunk instead of tearing down the HTTP response with an unhandled
    // exception. Terminating mid-stream strips the CORS headers the browser
    // needs, so the client sees a spurious CORS error instead of the real cause.
    private async IAsyncEnumerable<string> SafeStreamChunksAsync(
        IChatCompletionService chatCompletionService,
        ChatHistory history,
        OpenAIPromptExecutionSettings executionSettings,
        string conversationId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        IAsyncEnumerator<StreamingChatMessageContent>? enumerator = null;
        string? startupError = null;
        try
        {
            enumerator = chatCompletionService
                .GetStreamingChatMessageContentsAsync(history, executionSettings, kernel, cancellationToken)
                .GetAsyncEnumerator(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to start streaming for conversation {ConversationId}", conversationId);
            startupError = ex.Message;
        }

        if (startupError is not null || enumerator is null)
        {
            yield return $"\n\n*An error occurred: {startupError ?? "unable to start stream"}*";
            yield break;
        }

        try
        {
            while (true)
            {
                StreamingChatMessageContent? chunk;
                string? midStreamError = null;
                try
                {
                    if (!await enumerator.MoveNextAsync())
                    {
                        yield break;
                    }
                    chunk = enumerator.Current;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error streaming chunk for conversation {ConversationId}", conversationId);
                    midStreamError = ex.Message;
                    chunk = null;
                }

                if (midStreamError is not null)
                {
                    yield return $"\n\n*An error occurred mid-stream: {midStreamError}*";
                    yield break;
                }

                if (chunk is not null && !string.IsNullOrEmpty(chunk.Content))
                {
                    yield return chunk.Content;
                }
            }
        }
        finally
        {
            await enumerator.DisposeAsync();
        }
    }
}
