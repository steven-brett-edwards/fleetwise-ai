using FleetWise.Api.Controllers;
using FleetWise.Api.Models;
using FleetWise.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FleetWise.Api.Tests.Controllers;

/// <summary>
/// Tests for ChatController constructed with a mocked IChatOrchestrationService --
/// the same way DI builds it in production. The controller is a thin layer that
/// delegates entirely to the orchestration service; SK is invisible at the API boundary.
/// Stream tests use DefaultHttpContext with a MemoryStream as Response.Body.
/// </summary>
public class ChatControllerTests
{
    private readonly Mock<IChatOrchestrationService> _mockChatOrchestrationService = new();

    private ChatController CreateChatControllerWithMockedService()
    {
        return new ChatController(_mockChatOrchestrationService.Object);
    }

    private static ChatController WithHttpContext(ChatController controller)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    private static async IAsyncEnumerable<string> CreateAsyncStringStream(params string[] chunks)
    {
        foreach (var chunk in chunks)
        {
            yield return chunk;
            await Task.Yield();
        }
    }

    private static async IAsyncEnumerable<string> CreateEmptyAsyncStringStream()
    {
        await Task.Yield();
        yield break;
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(response.Body);
        return await reader.ReadToEndAsync();
    }

    // ── Chat ────────────────────────────────────────────────────────

    [Fact]
    public async Task Chat_WhenRequestReceived_ReturnsOkWithChatResponse()
    {
        // Setup
        var expectedChatResponse = new ChatResponse
        {
            Response = "The fleet has 35 vehicles.",
            ConversationId = "conv-001",
            FunctionsUsed = ["get_fleet_summary"]
        };

        _mockChatOrchestrationService
            .Setup(s => s.ProcessMessageAsync(It.IsAny<ChatRequest>()))
            .ReturnsAsync(expectedChatResponse);

        var chatControllerWithMockedService = CreateChatControllerWithMockedService();
        var chatRequest = new ChatRequest { Message = "How many vehicles?" };

        // Act
        var actionResult = await chatControllerWithMockedService.Chat(chatRequest);

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var chatResponse = Assert.IsType<ChatResponse>(okResult.Value);
        chatResponse.Response.Should().Be("The fleet has 35 vehicles.");
        chatResponse.ConversationId.Should().Be("conv-001");
        chatResponse.FunctionsUsed.Should().Contain("get_fleet_summary");
    }

    // ── Stream ──────────────────────────────────────────────────────

    [Fact]
    public async Task Stream_WhenChunksReceived_WritesServerSentEventsFormat()
    {
        // Setup
        _mockChatOrchestrationService
            .Setup(s => s.StreamMessageAsync(It.IsAny<ChatRequest>(), It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncStringStream("Hello", " World"));

        var chatControllerWithMockedService = WithHttpContext(CreateChatControllerWithMockedService());
        var chatRequest = new ChatRequest { Message = "Hi" };

        // Act
        await chatControllerWithMockedService.Stream(chatRequest, CancellationToken.None);

        // Result
        var responseBody = await ReadResponseBody(chatControllerWithMockedService.Response);
        responseBody.Should().Contain("data: Hello\n\n");
        responseBody.Should().Contain("data:  World\n\n");
    }

    [Fact]
    public async Task Stream_WhenChunkContainsNewlines_EscapesThemForFrameIntegrity()
    {
        // Setup -- a single chunk containing the kind of multi-line markdown
        // the agent emits (intro + table). Without escaping, the inner '\n'
        // would terminate the SSE event mid-frame and the client's parser
        // would drop the trailing rows -- the bug the markdown rendering
        // PR initially failed to fix on the wire.
        const string multilineChunk = "Public Works:\n| Asset | Year |\n|---|---|\n| V-1 | 2020 |";
        _mockChatOrchestrationService
            .Setup(s => s.StreamMessageAsync(It.IsAny<ChatRequest>(), It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncStringStream(multilineChunk));

        var chatControllerWithMockedService = WithHttpContext(CreateChatControllerWithMockedService());
        var chatRequest = new ChatRequest { Message = "Hi" };

        // Act
        await chatControllerWithMockedService.Stream(chatRequest, CancellationToken.None);

        // Result -- inner newlines became literal `\n`; only the trailing
        // `\n\n` separates SSE events.
        var responseBody = await ReadResponseBody(chatControllerWithMockedService.Response);
        const string expectedEscaped = "data: Public Works:\\n| Asset | Year |\\n|---|---|\\n| V-1 | 2020 |\n\n";
        responseBody.Should().Contain(expectedEscaped);
    }

    [Fact]
    public async Task Stream_WhenChunkContainsBackslashes_EscapesThemFirstSoClientCanReverseSafely()
    {
        // Setup -- if the model literally emits `\n` (two characters: a
        // backslash followed by `n`, e.g. inside a code block), the encoder
        // must escape the backslash *first* so the client doesn't decode it
        // as a newline. Wire format: `\\n` round-trips to the original
        // `\n` (two chars); a real newline would have been encoded as `\\n`
        // only via the newline replace, leaving the backslash untouched.
        _mockChatOrchestrationService
            .Setup(s => s.StreamMessageAsync(It.IsAny<ChatRequest>(), It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncStringStream("path: C:\\temp"));

        var chatControllerWithMockedService = WithHttpContext(CreateChatControllerWithMockedService());
        var chatRequest = new ChatRequest { Message = "Hi" };

        // Act
        await chatControllerWithMockedService.Stream(chatRequest, CancellationToken.None);

        // Result
        var responseBody = await ReadResponseBody(chatControllerWithMockedService.Response);
        responseBody.Should().Contain("data: path: C:\\\\temp\n\n");
    }

    [Fact]
    public async Task Stream_WhenStreamCompletes_WritesDoneMarker()
    {
        // Setup
        _mockChatOrchestrationService
            .Setup(s => s.StreamMessageAsync(It.IsAny<ChatRequest>(), It.IsAny<CancellationToken>()))
            .Returns(CreateAsyncStringStream("Hello"));

        var chatControllerWithMockedService = WithHttpContext(CreateChatControllerWithMockedService());
        var chatRequest = new ChatRequest { Message = "Hi" };

        // Act
        await chatControllerWithMockedService.Stream(chatRequest, CancellationToken.None);

        // Result
        var responseBody = await ReadResponseBody(chatControllerWithMockedService.Response);
        responseBody.Should().EndWith("data: [DONE]\n\n");
    }

    [Fact]
    public async Task Stream_WhenCalled_SetsCorrectSseResponseHeaders()
    {
        // Setup
        _mockChatOrchestrationService
            .Setup(s => s.StreamMessageAsync(It.IsAny<ChatRequest>(), It.IsAny<CancellationToken>()))
            .Returns(CreateEmptyAsyncStringStream());

        var chatControllerWithMockedService = WithHttpContext(CreateChatControllerWithMockedService());
        var chatRequest = new ChatRequest { Message = "Hi" };

        // Act
        await chatControllerWithMockedService.Stream(chatRequest, CancellationToken.None);

        // Result
        var response = chatControllerWithMockedService.Response;
        response.ContentType.Should().Be("text/event-stream");
        response.Headers.CacheControl.ToString().Should().Be("no-cache");
        response.Headers.Connection.ToString().Should().Be("keep-alive");
    }

    [Fact]
    public async Task Stream_WhenNoChunksReceived_StillWritesDoneMarker()
    {
        // Setup
        _mockChatOrchestrationService
            .Setup(s => s.StreamMessageAsync(It.IsAny<ChatRequest>(), It.IsAny<CancellationToken>()))
            .Returns(CreateEmptyAsyncStringStream());

        var chatControllerWithMockedService = WithHttpContext(CreateChatControllerWithMockedService());
        var chatRequest = new ChatRequest { Message = "Hi" };

        // Act
        await chatControllerWithMockedService.Stream(chatRequest, CancellationToken.None);

        // Result
        var responseBody = await ReadResponseBody(chatControllerWithMockedService.Response);
        responseBody.Should().Be("data: [DONE]\n\n");
    }
}
