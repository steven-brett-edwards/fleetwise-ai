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
}
