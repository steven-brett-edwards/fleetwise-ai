using FleetWise.Infrastructure.Repositories;
using FluentAssertions;

namespace FleetWise.Infrastructure.Tests;

public class PartRepositoryTests : SqliteRepositoryTestBase
{
    private readonly PartRepository _repository;

    public PartRepositoryTests()
    {
        Context.Parts.AddRange(
            // Category "Brakes": 2 parts, one below threshold, one above
            NewPart(1, "BRK-001", "Brake Pad Set",  "Brakes",  quantityInStock: 2, reorderThreshold: 5),
            NewPart(2, "BRK-002", "Brake Rotor",    "Brakes",  quantityInStock: 8, reorderThreshold: 3),
            // Category "Filters": 1 part at threshold (at or below means <= reorderThreshold)
            NewPart(3, "FLT-001", "Air Filter",     "Filters", quantityInStock: 4, reorderThreshold: 4)
        );
        Context.SaveChanges();

        _repository = new PartRepository(Context);
    }

    // ── GetAllAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllPartsOrderedByCategoryThenName()
    {
        // Act
        var allParts = await _repository.GetAllAsync();

        // Result
        allParts.Should().HaveCount(3);
        // Ordering: Brakes/Brake Pad Set, Brakes/Brake Rotor, Filters/Air Filter
        allParts[0].Category.Should().Be("Brakes");
        allParts[0].Name.Should().Be("Brake Pad Set");
        allParts[1].Category.Should().Be("Brakes");
        allParts[1].Name.Should().Be("Brake Rotor");
        allParts[2].Category.Should().Be("Filters");
    }

    // ── GetBelowReorderThresholdAsync ──────────────────────────────

    [Fact]
    public async Task GetBelowReorderThresholdAsync_ReturnsPartsWhereQuantityIsAtOrBelowThreshold()
    {
        // Setup:
        //   BRK-001: stock 2 <= threshold 5 → below (should be returned)
        //   BRK-002: stock 8 >  threshold 3 → above (should NOT be returned)
        //   FLT-001: stock 4 <= threshold 4 → at threshold (should be returned)

        // Act
        var lowStockParts = await _repository.GetBelowReorderThresholdAsync();

        // Result
        lowStockParts.Should().HaveCount(2);
        lowStockParts.Select(p => p.PartNumber).Should().Contain("BRK-001");
        lowStockParts.Select(p => p.PartNumber).Should().Contain("FLT-001");
        lowStockParts.Select(p => p.PartNumber).Should().NotContain("BRK-002");
    }

    [Fact]
    public async Task GetBelowReorderThresholdAsync_ReturnsPartsOrderedByQuantityAscending()
    {
        // Setup: BRK-001 (stock 2), FLT-001 (stock 4) both below/at threshold

        // Act
        var lowStockParts = await _repository.GetBelowReorderThresholdAsync();

        // Result
        lowStockParts.Select(p => p.QuantityInStock).Should().BeInAscendingOrder();
    }
}
