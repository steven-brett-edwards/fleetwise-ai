using FleetWise.Api.Models;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace FleetWise.Api.Services;

/// <summary>
/// Reads fleet management documents from disk, splits them into chunks,
/// generates vector embeddings, and stores them in the vector store.
/// Runs once at application startup — similar to SeedData.Initialize for SQL.
/// </summary>
public class DocumentIngestionService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    InMemoryVectorStore vectorStore,
    ILogger<DocumentIngestionService> logger)
{
    private const string CollectionName = "fleet-documents";
    private const int MaxChunkLength = 500;

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
            var chunks = ChunkByHeadings(content);

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

    /// <summary>
    /// Splits a markdown document into chunks at ## heading boundaries.
    /// If a section exceeds <see cref="MaxChunkLength"/> characters,
    /// it is sub-split by paragraph (double newline) boundaries.
    /// </summary>
    internal static List<string> ChunkByHeadings(string content)
    {
        var chunks = new List<string>();
        var sections = content.Split("\n## ", StringSplitOptions.RemoveEmptyEntries);

        foreach (var section in sections)
        {
            // Re-add the heading prefix that was removed by split (except for the first section
            // which starts with the # title, not ##)
            var text = section == sections[0] ? section.Trim() : $"## {section.Trim()}";

            if (text.Length <= MaxChunkLength)
            {
                chunks.Add(text);
            }
            else
            {
                // Sub-split long sections by paragraph
                chunks.AddRange(ChunkByParagraphs(text));
            }
        }

        return chunks;
    }

    /// <summary>
    /// Splits a long section into paragraph-sized chunks, keeping each
    /// under <see cref="MaxChunkLength"/> characters.
    /// </summary>
    internal static List<string> ChunkByParagraphs(string section)
    {
        var chunks = new List<string>();
        var paragraphs = section.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        var current = string.Empty;

        foreach (var paragraph in paragraphs)
        {
            var trimmed = paragraph.Trim();
            if (current.Length == 0)
            {
                current = trimmed;
            }
            else if (current.Length + trimmed.Length + 2 <= MaxChunkLength)
            {
                current = $"{current}\n\n{trimmed}";
            }
            else
            {
                chunks.Add(current);
                current = trimmed;
            }
        }

        if (current.Length > 0)
        {
            chunks.Add(current);
        }

        return chunks;
    }
}
