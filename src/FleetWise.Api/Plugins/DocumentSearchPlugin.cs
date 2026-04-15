using System.ComponentModel;
using System.Text;
using FleetWise.Api.Models;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;

namespace FleetWise.Api.Plugins;

/// <summary>
/// Semantic Kernel plugin that searches fleet management documentation using
/// vector similarity. The LLM calls this when users ask about policies,
/// procedures, SOPs, or compliance — information stored in documents rather
/// than the database.
/// </summary>
public class DocumentSearchPlugin(
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    InMemoryVectorStore vectorStore)
{
    private const string CollectionName = "fleet-documents";

    [KernelFunction("search_fleet_documentation")]
    [Description(
        "Search fleet management documentation for policies, procedures, and guidelines. " +
        "Use this for questions about: maintenance procedures, safety policies, work order SOPs, " +
        "vehicle lifecycle and replacement criteria, fuel management and anti-idling policies, " +
        "PPE requirements, accident reporting, parts ordering thresholds, and compliance guidelines. " +
        "Do NOT use this for questions about specific vehicles, work orders, or live fleet data -- " +
        "use the other fleet query functions for those.")]
    public async Task<string> SearchDocumentation(
        [Description("The search query describing what information you need")] string query,
        [Description("Number of results to return (default 3)")] int topK = 3)
    {
        var collection = vectorStore.GetCollection<string, DocumentChunkRecord>(CollectionName);

        // Embed the search query using the same model that embedded the documents
        var queryEmbedding = await embeddingGenerator.GenerateAsync(query);

        // Search for the most similar document chunks
        var searchResults = collection.SearchAsync(queryEmbedding.Vector, topK);

        var results = new StringBuilder();
        results.AppendLine($"Found the following relevant documentation for: \"{query}\"");
        results.AppendLine();

        var resultCount = 0;
        await foreach (var result in searchResults)
        {
            resultCount++;
            results.AppendLine($"--- Source: {result.Record.Source} (relevance: {result.Score:F3}) ---");
            results.AppendLine(result.Record.Content);
            results.AppendLine();
        }

        if (resultCount == 0)
        {
            return $"No documentation found matching: \"{query}\". Try rephrasing the query or ask about a specific policy area.";
        }

        return results.ToString();
    }
}
