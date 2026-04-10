namespace FleetWise.Domain.Entities;

/// <summary>
/// A chunk of documentation stored in the vector store for RAG retrieval.
/// This entity lives in the vector store (InMemory or Qdrant), not in SQL.
/// </summary>
public class DocumentChunk
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Source { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ReadOnlyMemory<float> Embedding { get; set; }
}
