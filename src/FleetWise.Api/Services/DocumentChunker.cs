namespace FleetWise.Api.Services;

/// <summary>
/// Pure text-processing functions that split markdown documents into chunks
/// suitable for embedding. Separated from DocumentIngestionService so the
/// testable logic lives in its own coverage-tracked class.
/// </summary>
public static class DocumentChunker
{
    private const int MaxChunkLength = 500;

    /// <summary>
    /// Splits a markdown document into chunks at ## heading boundaries.
    /// If a section exceeds <see cref="MaxChunkLength"/> characters,
    /// it is sub-split by paragraph (double newline) boundaries.
    /// </summary>
    public static List<string> ChunkByHeadings(string content)
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
    public static List<string> ChunkByParagraphs(string section)
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
