namespace FleetWise.Api.Models;

public class ChatRequest
{
    public required string Message { get; set; }
    public string? ConversationId { get; set; }
}

public class ChatResponse
{
    public required string Response { get; set; }
    public required string ConversationId { get; set; }
    public List<string> FunctionsUsed { get; set; } = [];
}
