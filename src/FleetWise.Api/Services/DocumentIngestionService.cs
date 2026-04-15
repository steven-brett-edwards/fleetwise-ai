using System.Diagnostics.CodeAnalysis;
using FleetWise.Api.Models;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace FleetWise.Api.Services;

/// <summary>
/// Reads fleet management documents from disk, splits them into chunks,
/// generates vector embeddings, and stores them in the vector store.
/// Runs once at application startup — similar to SeedData.Initialize for SQL.
/// </summary>
/// <remarks>
/// Excluded from coverage because this is pure I/O orchestration (file reads,
/// embedding API calls, vector store writes). The testable chunking logic lives
/// in <see cref="DocumentChunker"/>.
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "I/O orchestration — chunking logic tested via DocumentChunker")]
public class DocumentIngestionService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    InMemoryVectorStore vectorStore,
    ILogger<DocumentIngestionService> logger)
{
    private const string CollectionName = "fleet-documents";

    public async Task IngestDocumentsAsync(string documentsPath)
    {
        var collection = vectorStore.GetCollection<string, DocumentChunkRecord>(CollectionName);
        await collection.EnsureCollectionExistsAsync();

        var files = Directory.GetFiles(documentsPath, "*.md");
        logger.LogInformation("Found {FileCount} document(s) in {Path}", files.Length, documentsPath);

        var totalChunks = 0;

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var content = await File.ReadAllTextAsync(file);
            var chunks = DocumentChunker.ChunkByHeadings(content);

            // Generate embeddings for all chunks in a single batch call
            var embeddings = await embeddingGenerator.GenerateAsync(chunks);

            for (var i = 0; i < chunks.Count; i++)
            {
                var record = new DocumentChunkRecord
                {
                    Id = $"{fileName}_{i}",
                    Source = fileName,
                    Content = chunks[i],
                    Embedding = embeddings[i].Vector
                };

                await collection.UpsertAsync(record);
            }

            totalChunks += chunks.Count;
            logger.LogInformation("Ingested {ChunkCount} chunks from {FileName}", chunks.Count, fileName);
        }

        logger.LogInformation("Document ingestion complete: {TotalChunks} total chunks from {FileCount} files",
            totalChunks, files.Length);
    }
}
