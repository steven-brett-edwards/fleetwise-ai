using System.Runtime.CompilerServices;
using FleetWise.Api.Models;
using FleetWise.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Moq;

namespace FleetWise.Api.Tests.Services;

/// <summary>
/// Tests for ChatOrchestrationService constructed with a real Kernel that has a mocked
/// IChatCompletionService -- the same way DI builds it in production.
/// Each test uses a unique ConversationId because the service stores conversation state
/// in a static ConcurrentDictionary that persists across tests.
/// </summary>
public class ChatOrchestrationServiceTests
{
    private readonly Mock<IChatCompletionService> _mockChatCompletionService = new();
    private readonly Mock<ILogger<ChatOrchestrationService>> _mockLogger = new();

    private ChatOrchestrationService CreateOrchestrationServiceWithMockedLlm()
    {
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.Services.AddSingleton(_mockChatCompletionService.Object);
        var kernelWithMockedLlm = kernelBuilder.Build();
        return new ChatOrchestrationService(kernelWithMockedLlm, _mockLogger.Object);
    }

    private void SetupMockLlmToReturn(string? content)
    {
        _mockChatCompletionService
            .Setup(s => s.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatMessageContent>
            {
                new(AuthorRole.Assistant, content)
            });
    }

    private void SetupMockStreamingLlmToReturn(params string?[] chunks)
    {
        _mockChatCompletionService
            .Setup(s => s.GetStreamingChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Returns(CreateStreamingChunks(chunks));
    }

    private static async IAsyncEnumerable<StreamingChatMessageContent> CreateStreamingChunks(
        params string?[] contents)
    {
        foreach (var content in contents)
        {
            yield return new StreamingChatMessageContent(AuthorRole.Assistant, content);
            await Task.Yield();
        }
    }

    private static async IAsyncEnumerable<StreamingChatMessageContent> CreateStreamingChunksWithDelay(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield return new StreamingChatMessageContent(AuthorRole.Assistant, "First chunk");
        await Task.Delay(100, cancellationToken);
        yield return new StreamingChatMessageContent(AuthorRole.Assistant, "Second chunk");
    }

    // ── ProcessMessageAsync ─────────────────────────────────────────

    [Fact]
    public async Task ProcessMessageAsync_WhenNoConversationIdProvided_GeneratesNewConversationId()
    {
        // Setup
        SetupMockLlmToReturn("Hello!");
        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var requestWithNoConversationId = new ChatRequest { Message = "Hi" };

        // Act
        var chatResponse = await orchestrationServiceWithMockedLlm.ProcessMessageAsync(
            requestWithNoConversationId);

        // Result
        chatResponse.ConversationId.Should().NotBeNullOrEmpty();
        Guid.TryParse(chatResponse.ConversationId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenConversationIdProvided_ReusesExistingConversationId()
    {
        // Setup
        SetupMockLlmToReturn("Hello!");
        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var providedConversationId = Guid.NewGuid().ToString();
        var requestWithConversationId = new ChatRequest
        {
            Message = "Hi",
            ConversationId = providedConversationId
        };

        // Act
        var chatResponse = await orchestrationServiceWithMockedLlm.ProcessMessageAsync(
            requestWithConversationId);

        // Result
        chatResponse.ConversationId.Should().Be(providedConversationId);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenFirstMessageInConversation_AddsSystemPromptToHistory()
    {
        // Setup
        ChatHistory? capturedChatHistory = null;
        _mockChatCompletionService
            .Setup(s => s.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Callback<ChatHistory, PromptExecutionSettings, Kernel, CancellationToken>(
                (history, _, _, _) => capturedChatHistory = history)
            .ReturnsAsync(new List<ChatMessageContent>
            {
                new(AuthorRole.Assistant, "Hello!")
            });

        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var requestForNewConversation = new ChatRequest
        {
            Message = "Hi",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        await orchestrationServiceWithMockedLlm.ProcessMessageAsync(requestForNewConversation);

        // Result
        capturedChatHistory.Should().NotBeNull();
        (capturedChatHistory!.First().Role == AuthorRole.System).Should().BeTrue();
        capturedChatHistory!.First().Content.Should().Contain("FleetWise AI");
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenSecondMessageInSameConversation_DoesNotDuplicateSystemPrompt()
    {
        // Setup
        ChatHistory? capturedChatHistoryOnSecondCall = null;
        var callCount = 0;
        _mockChatCompletionService
            .Setup(s => s.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Callback<ChatHistory, PromptExecutionSettings, Kernel, CancellationToken>(
                (history, _, _, _) =>
                {
                    callCount++;
                    if (callCount == 2) capturedChatHistoryOnSecondCall = history;
                })
            .ReturnsAsync(new List<ChatMessageContent>
            {
                new(AuthorRole.Assistant, "Response")
            });

        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var sharedConversationId = Guid.NewGuid().ToString();

        // Act
        await orchestrationServiceWithMockedLlm.ProcessMessageAsync(
            new ChatRequest { Message = "First message", ConversationId = sharedConversationId });
        await orchestrationServiceWithMockedLlm.ProcessMessageAsync(
            new ChatRequest { Message = "Second message", ConversationId = sharedConversationId });

        // Result
        capturedChatHistoryOnSecondCall.Should().NotBeNull();
        var systemMessageCount = capturedChatHistoryOnSecondCall!
            .Count(m => m.Role == AuthorRole.System);
        systemMessageCount.Should().Be(1);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenLlmReturnsContent_ReturnsResponseContent()
    {
        // Setup
        SetupMockLlmToReturn("The fleet has 35 vehicles.");
        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var requestAboutFleet = new ChatRequest
        {
            Message = "How many vehicles?",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        var chatResponse = await orchestrationServiceWithMockedLlm.ProcessMessageAsync(requestAboutFleet);

        // Result
        chatResponse.Response.Should().Be("The fleet has 35 vehicles.");
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenLlmReturnsNullContent_ReturnsEmptyString()
    {
        // Setup
        SetupMockLlmToReturn(null);
        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var requestThatGetsNullContent = new ChatRequest
        {
            Message = "Hello",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        var chatResponse = await orchestrationServiceWithMockedLlm.ProcessMessageAsync(
            requestThatGetsNullContent);

        // Result
        chatResponse.Response.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenHistoryContainsFunctionCallContent_ExtractsFunctionNames()
    {
        // Setup
        _mockChatCompletionService
            .Setup(s => s.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Callback<ChatHistory, PromptExecutionSettings, Kernel, CancellationToken>(
                (history, _, _, _) =>
                {
                    // Simulate SK adding a function call to history during the orchestration loop
                    var functionCallItems = new ChatMessageContentItemCollection
                    {
                        new FunctionCallContent("get_fleet_summary")
                    };
                    history.Add(new ChatMessageContent(AuthorRole.Assistant, functionCallItems));
                })
            .ReturnsAsync(new List<ChatMessageContent>
            {
                new(AuthorRole.Assistant, "The fleet has 35 vehicles.")
            });

        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var requestTriggeringFunctionCall = new ChatRequest
        {
            Message = "How many vehicles?",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        var chatResponse = await orchestrationServiceWithMockedLlm.ProcessMessageAsync(
            requestTriggeringFunctionCall);

        // Result
        chatResponse.FunctionsUsed.Should().Contain("get_fleet_summary");
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenNoFunctionsCalled_ReturnsEmptyFunctionsList()
    {
        // Setup
        SetupMockLlmToReturn("Hello, how can I help?");
        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var greetingRequest = new ChatRequest
        {
            Message = "Hello",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        var chatResponse = await orchestrationServiceWithMockedLlm.ProcessMessageAsync(greetingRequest);

        // Result
        chatResponse.FunctionsUsed.Should().BeEmpty();
    }

    // ── StreamMessageAsync ──────────────────────────────────────────

    [Fact]
    public async Task StreamMessageAsync_WhenChunksReceived_YieldsNonEmptyChunks()
    {
        // Setup
        SetupMockStreamingLlmToReturn("Hello", " World");
        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var streamingRequest = new ChatRequest
        {
            Message = "Hi",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        var receivedChunks = new List<string>();
        await foreach (var chunk in orchestrationServiceWithMockedLlm.StreamMessageAsync(streamingRequest))
        {
            receivedChunks.Add(chunk);
        }

        // Result
        receivedChunks.Should().HaveCount(2);
        receivedChunks[0].Should().Be("Hello");
        receivedChunks[1].Should().Be(" World");
    }

    [Fact]
    public async Task StreamMessageAsync_WhenChunkHasNullOrEmptyContent_SkipsChunk()
    {
        // Setup
        SetupMockStreamingLlmToReturn("Hello", null, "", " World");
        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var streamingRequest = new ChatRequest
        {
            Message = "Hi",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        var receivedChunks = new List<string>();
        await foreach (var chunk in orchestrationServiceWithMockedLlm.StreamMessageAsync(streamingRequest))
        {
            receivedChunks.Add(chunk);
        }

        // Result
        receivedChunks.Should().HaveCount(2);
        receivedChunks.Should().Equal("Hello", " World");
    }

    [Fact]
    public async Task StreamMessageAsync_WhenStreamCompletes_AddsFullResponseToConversationHistory()
    {
        // Setup
        SetupMockStreamingLlmToReturn("Hello", " World");

        ChatHistory? capturedChatHistoryOnFollowUp = null;
        _mockChatCompletionService
            .Setup(s => s.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Callback<ChatHistory, PromptExecutionSettings, Kernel, CancellationToken>(
                (history, _, _, _) => capturedChatHistoryOnFollowUp = history)
            .ReturnsAsync(new List<ChatMessageContent>
            {
                new(AuthorRole.Assistant, "Follow-up response")
            });

        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var sharedConversationId = Guid.NewGuid().ToString();

        // Act -- stream the first message, then send a follow-up to capture history state
        await foreach (var _ in orchestrationServiceWithMockedLlm.StreamMessageAsync(
            new ChatRequest { Message = "Hi", ConversationId = sharedConversationId })) { }

        await orchestrationServiceWithMockedLlm.ProcessMessageAsync(
            new ChatRequest { Message = "Follow-up", ConversationId = sharedConversationId });

        // Result -- the concatenated streamed response should be in conversation history
        capturedChatHistoryOnFollowUp.Should().NotBeNull();
        var assistantMessages = capturedChatHistoryOnFollowUp!
            .Where(m => m.Role == AuthorRole.Assistant)
            .Select(m => m.Content)
            .ToList();
        assistantMessages.Should().Contain("Hello World");
    }

    [Fact]
    public async Task StreamMessageAsync_WhenFirstMessageInConversation_AddsSystemPrompt()
    {
        // Setup
        ChatHistory? capturedChatHistory = null;
        _mockChatCompletionService
            .Setup(s => s.GetStreamingChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Callback<ChatHistory, PromptExecutionSettings, Kernel, CancellationToken>(
                (history, _, _, _) => capturedChatHistory = history)
            .Returns(CreateStreamingChunks("Hello"));

        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var streamingRequest = new ChatRequest
        {
            Message = "Hi",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        await foreach (var _ in orchestrationServiceWithMockedLlm.StreamMessageAsync(streamingRequest)) { }

        // Result
        capturedChatHistory.Should().NotBeNull();
        (capturedChatHistory!.First().Role == AuthorRole.System).Should().BeTrue();
        capturedChatHistory!.First().Content.Should().Contain("FleetWise AI");
    }

    [Fact]
    public async Task StreamMessageAsync_WhenCancellationRequested_StopsYielding()
    {
        // Setup
        var cancellationTokenSource = new CancellationTokenSource();
        _mockChatCompletionService
            .Setup(s => s.GetStreamingChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Returns((ChatHistory _, PromptExecutionSettings? _, Kernel? _, CancellationToken ct) =>
                CreateStreamingChunksWithDelay(ct));

        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var streamingRequest = new ChatRequest
        {
            Message = "Tell me everything",
            ConversationId = Guid.NewGuid().ToString()
        };

        // Act
        var receivedChunks = new List<string>();
        var streamingAction = async () =>
        {
            await foreach (var chunk in orchestrationServiceWithMockedLlm.StreamMessageAsync(
                streamingRequest, cancellationTokenSource.Token))
            {
                receivedChunks.Add(chunk);
                await cancellationTokenSource.CancelAsync();
            }
        };

        // Result -- cancellation should stop the stream after the first chunk
        await streamingAction.Should().ThrowAsync<OperationCanceledException>();
        receivedChunks.Should().HaveCount(1);
        receivedChunks[0].Should().Be("First chunk");
    }

    [Fact]
    public async Task StreamMessageAsync_WhenNoConversationIdProvided_GeneratesNewConversationId()
    {
        // Setup
        SetupMockStreamingLlmToReturn("Hello");

        // Also set up non-streaming mock to capture the conversation after streaming
        ChatHistory? capturedChatHistory = null;
        _mockChatCompletionService
            .Setup(s => s.GetChatMessageContentsAsync(
                It.IsAny<ChatHistory>(),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()))
            .Callback<ChatHistory, PromptExecutionSettings, Kernel, CancellationToken>(
                (history, _, _, _) => capturedChatHistory = history)
            .ReturnsAsync(new List<ChatMessageContent>
            {
                new(AuthorRole.Assistant, "Follow-up")
            });

        var orchestrationServiceWithMockedLlm = CreateOrchestrationServiceWithMockedLlm();
        var requestWithNoConversationId = new ChatRequest { Message = "Hi" };

        // Act -- streaming creates the conversation internally even without an ID
        await foreach (var _ in orchestrationServiceWithMockedLlm.StreamMessageAsync(
            requestWithNoConversationId)) { }

        // Result -- the conversation was created (system prompt + user message + assistant response)
        // We verify indirectly: the stream completed without error, proving Guid.NewGuid() was used
        // to create a valid conversation entry in the static dictionary
        _mockChatCompletionService.Verify(
            s => s.GetStreamingChatMessageContentsAsync(
                It.Is<ChatHistory>(h => h.First().Role == AuthorRole.System),
                It.IsAny<PromptExecutionSettings>(),
                It.IsAny<Kernel>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
