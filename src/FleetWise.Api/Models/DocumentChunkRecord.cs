using Microsoft.Extensions.VectorData;

namespace FleetWise.Api.Models;

/// <summary>
/// A chunk of fleet management documentation stored in the vector store for RAG retrieval.
/// Each chunk is a section of a source document, embedded as a 768-dimensional vector
/// for semantic similarity search.
/// </summary>
/// <remarks>
/// This lives in the Api project (not Domain) because it requires SK vector store attributes.
/// The Domain project's DocumentChunk entity remains attribute-free by design.
/// </remarks>
public class DocumentChunkRecord
{
    [VectorStoreKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [VectorStoreData]
    public string Source { get; set; } = string.Empty;

    [VectorStoreData]
    public string Content { get; set; } = string.Empty;

    [VectorStoreVector(768)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}
