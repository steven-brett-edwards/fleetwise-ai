using FleetWise.Api.Models;
using FleetWise.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatOrchestrationService chatService) : ControllerBase
{
    /// <summary>Send a message to the AI assistant and receive a response.</summary>
    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        var response = await chatService.ProcessMessageAsync(request);
        return Ok(response);
    }

    /// <summary>Send a message and receive a streaming response via Server-Sent Events.</summary>
    [HttpPost("stream")]
    public async Task Stream([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        await foreach (var chunk in chatService.StreamMessageAsync(request, cancellationToken))
        {
            // Escape backslashes first (so `\` doesn't double-unescape on the
            // client), then \r and \n. Without this, any chunk containing a
            // newline (which markdown blocks always do) terminates the SSE
            // event mid-frame and the client drops the trailing half --
            // collapsing tables and lists onto a single line.
            var escaped = chunk
                .Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
            await Response.WriteAsync($"data: {escaped}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        await Response.WriteAsync("data: [DONE]\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
}
