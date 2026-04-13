using FleetWise.Api.Models;

namespace FleetWise.Api.Services;

public interface IChatOrchestrationService
{
    Task<ChatResponse> ProcessMessageAsync(ChatRequest request);
    IAsyncEnumerable<string> StreamMessageAsync(ChatRequest request, CancellationToken cancellationToken = default);
}
