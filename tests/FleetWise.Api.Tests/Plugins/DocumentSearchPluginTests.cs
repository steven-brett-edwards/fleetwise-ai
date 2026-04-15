using FleetWise.Api.Models;
using FleetWise.Api.Plugins;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Moq;

namespace FleetWise.Api.Tests.Plugins;

/// <summary>
/// Tests for DocumentSearchPlugin using a real InMemoryVectorStore seeded with
/// test data and a mocked IEmbeddingGenerator. The mock returns a fixed vector
/// so we can control which chunks are "similar" to the query.
/// </summary>
public class DocumentSearchPluginTests
{
    private readonly Mock<IEmbeddingGenerator<string, Embedding<float>>> _mockEmbeddingGenerator = new();
    private readonly InMemoryVectorStore _vectorStore = new();

    /// <summary>
    /// Creates a 768-dimensional vector with all zeros except one position set to 1.0.
    /// Two vectors with the same "hot" position will have high cosine similarity;
    /// different positions will have zero similarity.
    /// </summary>
    private static ReadOnlyMemory<float> CreateTestVector(int hotIndex)
    {
        var vector = new float[768];
        vector[hotIndex] = 1.0f;
        return new ReadOnlyMemory<float>(vector);
    }

    /// <summary>
    /// Configures the mock embedding generator to return a fixed vector for any input.
    /// We mock the batch interface method (IEnumerable&lt;string&gt;) because the single-string
    /// GenerateAsync is an extension method that delegates to it — Moq can't intercept
    /// extension methods directly.
    /// </summary>
    private void SetupEmbeddingGenerator(ReadOnlyMemory<float> vectorToReturn)
    {
        _mockEmbeddingGenerator
            .Setup(e => e.GenerateAsync(
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<EmbeddingGenerationOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GeneratedEmbeddings<Embedding<float>>([new Embedding<float>(vectorToReturn)]));
    }

    private async Task SeedVectorStoreAsync(params DocumentChunkRecord[] records)
    {
        var collection = _vectorStore.GetCollection<string, DocumentChunkRecord>("fleet-documents");
        await collection.EnsureCollectionExistsAsync();

        foreach (var record in records)
        {
            await collection.UpsertAsync(record);
        }
    }

    private Kernel CreateKernelWithDocumentSearchPlugin()
    {
        var kernel = Kernel.CreateBuilder().Build();
        kernel.ImportPluginFromObject(
            new DocumentSearchPlugin(_mockEmbeddingGenerator.Object, _vectorStore), "DocumentSearch");
        return kernel;
    }

    // ── search_fleet_documentation ──────────────────────────────────

    [Fact]
    public async Task SearchDocumentation_WhenMatchingChunksExist_ReturnsFormattedResultsWithSourceAttribution()
    {
        // Setup
        var antiIdlingChunk = new DocumentChunkRecord
        {
            Id = "fuel_0",
            Source = "fuel-management-policy.md",
            Content = "Vehicles must not idle for more than 3 consecutive minutes.",
            Embedding = CreateTestVector(0)
        };
        await SeedVectorStoreAsync(antiIdlingChunk);
        SetupEmbeddingGenerator(CreateTestVector(0));

        var kernel = CreateKernelWithDocumentSearchPlugin();

        // Act
        var result = await kernel.InvokeAsync("DocumentSearch", "search_fleet_documentation",
            new KernelArguments { ["query"] = "anti-idling policy" });

        // Result
        var response = result.ToString();
        response.Should().Contain("fuel-management-policy.md");
        response.Should().Contain("3 consecutive minutes");
        response.Should().Contain("relevance:");
    }

    [Fact]
    public async Task SearchDocumentation_WhenNoChunksExist_ReturnsNoDocumentationFoundMessage()
    {
        // Setup -- empty vector store (no seed data)
        var collection = _vectorStore.GetCollection<string, DocumentChunkRecord>("fleet-documents");
        await collection.EnsureCollectionExistsAsync();
        SetupEmbeddingGenerator(CreateTestVector(5));

        var kernel = CreateKernelWithDocumentSearchPlugin();

        // Act
        var result = await kernel.InvokeAsync("DocumentSearch", "search_fleet_documentation",
            new KernelArguments { ["query"] = "nonexistent topic" });

        // Result
        var response = result.ToString();
        response.Should().Contain("No documentation found");
        response.Should().Contain("nonexistent topic");
    }

    [Fact]
    public async Task SearchDocumentation_WhenMultipleChunksExist_ReturnsResultsWithRelevanceScores()
    {
        // Setup -- seed 3 chunks with different vectors
        var chunk1 = new DocumentChunkRecord
        {
            Id = "safety_0",
            Source = "safety-policies.md",
            Content = "Pre-trip inspection is required.",
            Embedding = CreateTestVector(0)
        };
        var chunk2 = new DocumentChunkRecord
        {
            Id = "safety_1",
            Source = "safety-policies.md",
            Content = "Accident reporting within 1 hour.",
            Embedding = CreateTestVector(1)
        };
        var chunk3 = new DocumentChunkRecord
        {
            Id = "fuel_0",
            Source = "fuel-management-policy.md",
            Content = "Fuel card assigned to vehicle.",
            Embedding = CreateTestVector(2)
        };
        await SeedVectorStoreAsync(chunk1, chunk2, chunk3);
        SetupEmbeddingGenerator(CreateTestVector(0));

        var kernel = CreateKernelWithDocumentSearchPlugin();

        // Act
        var result = await kernel.InvokeAsync("DocumentSearch", "search_fleet_documentation",
            new KernelArguments { ["query"] = "safety procedures", ["topK"] = 2 });

        // Result
        var response = result.ToString();
        response.Should().Contain("relevant documentation");
        response.Should().Contain("safety procedures");
    }

    [Fact]
    public async Task SearchDocumentation_WhenCalled_IncludesQueryInResponseHeader()
    {
        // Setup
        var chunk = new DocumentChunkRecord
        {
            Id = "wo_0",
            Source = "work-order-sop.md",
            Content = "Submit work orders through FleetWise.",
            Embedding = CreateTestVector(0)
        };
        await SeedVectorStoreAsync(chunk);
        SetupEmbeddingGenerator(CreateTestVector(0));

        var kernel = CreateKernelWithDocumentSearchPlugin();

        // Act
        var result = await kernel.InvokeAsync("DocumentSearch", "search_fleet_documentation",
            new KernelArguments { ["query"] = "how to submit a work order" });

        // Result
        var response = result.ToString();
        response.Should().Contain("how to submit a work order");
        response.Should().Contain("work-order-sop.md");
    }
}
