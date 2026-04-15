using FleetWise.Api.Services;
using FluentAssertions;

namespace FleetWise.Api.Tests.Services;

/// <summary>
/// Tests for DocumentIngestionService's text chunking logic -- the pure functions
/// that split markdown documents into appropriately sized chunks for embedding.
/// These are unit tests (no I/O, no mocks, no embedding API) because chunking
/// quality directly determines RAG retrieval quality.
/// </summary>
public class DocumentChunkerTests
{
    // ── ChunkByHeadings ─────────────────────────────────────────────

    [Fact]
    public void ChunkByHeadings_WhenDocumentHasMultipleHeadings_SplitsAtEachHeading()
    {
        // Setup
        var document = """
            # Title

            ## Section One

            Content of section one.

            ## Section Two

            Content of section two.
            """;

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result
        chunks.Should().HaveCount(3);
        chunks[0].Should().StartWith("# Title");
        chunks[1].Should().StartWith("## Section One");
        chunks[2].Should().StartWith("## Section Two");
    }

    [Fact]
    public void ChunkByHeadings_WhenDocumentHasNoHeadings_ReturnsSingleChunk()
    {
        // Setup
        var document = "Just a plain paragraph with no headings.";

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result
        chunks.Should().ContainSingle()
            .Which.Should().Be("Just a plain paragraph with no headings.");
    }

    [Fact]
    public void ChunkByHeadings_WhenSectionExceedsMaxLength_SubSplitsByParagraph()
    {
        // Setup -- create a section longer than 500 chars
        var longParagraph1 = new string('A', 300);
        var longParagraph2 = new string('B', 300);
        var document = $"# Title\n\n## Long Section\n\n{longParagraph1}\n\n{longParagraph2}";

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result -- title chunk + 2 sub-chunks from the long section
        chunks.Should().HaveCount(3);
        chunks[0].Should().StartWith("# Title");
        chunks[1].Should().Contain(longParagraph1);
        chunks[2].Should().Contain(longParagraph2);
    }

    [Fact]
    public void ChunkByHeadings_WhenSectionIsExactlyMaxLength_DoesNotSubSplit()
    {
        // Setup -- create a section exactly at the 500-char boundary
        var content = new string('X', 487); // "## A\n\n" prefix = 6 chars + "A" heading = 1 + content
        var document = $"## A\n\n{content}";

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result
        var totalLength = chunks[0].Length;
        totalLength.Should().BeLessThanOrEqualTo(500);
        chunks.Should().ContainSingle();
    }

    [Fact]
    public void ChunkByHeadings_WhenDocumentHasOnlyTitle_ReturnsSingleChunk()
    {
        // Setup
        var document = "# Fleet Management Policies";

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result
        chunks.Should().ContainSingle()
            .Which.Should().Be("# Fleet Management Policies");
    }

    [Fact]
    public void ChunkByHeadings_WhenHeadingHasNoContent_StillCreatesChunk()
    {
        // Setup
        var document = "# Title\n\n## Empty Section\n\n## Another Section\n\nSome content.";

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result
        chunks.Should().HaveCount(3);
        chunks[1].Should().StartWith("## Empty Section");
        chunks[2].Should().StartWith("## Another Section");
    }

    [Fact]
    public void ChunkByHeadings_WhenContentHasLeadingAndTrailingWhitespace_TrimsChunks()
    {
        // Setup
        var document = "  # Title  \n\n## Section  \n\n  Content  ";

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result
        chunks.Should().AllSatisfy(chunk =>
        {
            chunk.Should().NotStartWith(" ");
            chunk.Should().NotEndWith(" ");
        });
    }

    [Fact]
    public void ChunkByHeadings_WhenUsedWithRealDocument_ProducesReasonableChunkCount()
    {
        // Setup -- simulate a realistic document structure
        var document = """
            # Preventive Maintenance Procedures

            ## Oil Changes

            Gasoline vehicles: every 5,000 miles. Diesel: every 7,500 miles.

            ## Tire Rotation

            Required every 7,500 miles for all vehicles.

            ## Brake Inspection

            Required every 15,000 miles or 12 months.
            """;

        // Act
        var chunks = DocumentChunker.ChunkByHeadings(document);

        // Result
        chunks.Should().HaveCount(4); // title + 3 sections
        chunks.Should().AllSatisfy(chunk =>
            chunk.Length.Should().BeLessThanOrEqualTo(500));
    }

    // ── ChunkByParagraphs ───────────────────────────────────────────

    [Fact]
    public void ChunkByParagraphs_WhenShortParagraphs_CombinesThemIntoOneChunk()
    {
        // Setup
        var section = "Short paragraph one.\n\nShort paragraph two.\n\nShort paragraph three.";

        // Act
        var chunks = DocumentChunker.ChunkByParagraphs(section);

        // Result -- all fit under 500 chars, so combined into one
        chunks.Should().ContainSingle();
        chunks[0].Should().Contain("Short paragraph one.");
        chunks[0].Should().Contain("Short paragraph three.");
    }

    [Fact]
    public void ChunkByParagraphs_WhenParagraphsExceedMaxLength_SplitsAcrossBoundary()
    {
        // Setup
        var para1 = new string('A', 300);
        var para2 = new string('B', 300);
        var section = $"{para1}\n\n{para2}";

        // Act
        var chunks = DocumentChunker.ChunkByParagraphs(section);

        // Result
        chunks.Should().HaveCount(2);
        chunks[0].Should().Be(para1);
        chunks[1].Should().Be(para2);
    }

    [Fact]
    public void ChunkByParagraphs_WhenMultipleParagraphsFitTogether_GroupsEfficiently()
    {
        // Setup -- 3 paragraphs: first two fit together, third needs its own chunk
        var para1 = new string('A', 200);
        var para2 = new string('B', 200);
        var para3 = new string('C', 200);
        var section = $"{para1}\n\n{para2}\n\n{para3}";

        // Act
        var chunks = DocumentChunker.ChunkByParagraphs(section);

        // Result -- para1+para2 fit (402 chars with \n\n), para3 is separate
        chunks.Should().HaveCount(2);
        chunks[0].Should().Contain(para1);
        chunks[0].Should().Contain(para2);
        chunks[1].Should().Be(para3);
    }

    [Fact]
    public void ChunkByParagraphs_WhenSingleParagraph_ReturnsSingleChunk()
    {
        // Setup
        var section = "Just one paragraph with no double-newlines.";

        // Act
        var chunks = DocumentChunker.ChunkByParagraphs(section);

        // Result
        chunks.Should().ContainSingle()
            .Which.Should().Be("Just one paragraph with no double-newlines.");
    }

    [Fact]
    public void ChunkByParagraphs_WhenEmptyString_ReturnsEmptyList()
    {
        // Act
        var chunks = DocumentChunker.ChunkByParagraphs(string.Empty);

        // Result
        chunks.Should().BeEmpty();
    }

    [Fact]
    public void ChunkByParagraphs_WhenParagraphsHaveExtraWhitespace_TrimsContent()
    {
        // Setup
        var section = "  First paragraph  \n\n  Second paragraph  ";

        // Act
        var chunks = DocumentChunker.ChunkByParagraphs(section);

        // Result
        chunks.Should().ContainSingle();
        chunks[0].Should().NotStartWith(" ");
        chunks[0].Should().Contain("First paragraph");
        chunks[0].Should().Contain("Second paragraph");
    }
}
