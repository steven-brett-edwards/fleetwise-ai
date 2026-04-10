namespace FleetWise.Domain.Entities;

/// <summary>
/// A part in the fleet maintenance parts inventory.
/// </summary>
public class Part
{
    public int Id { get; set; }
    public string PartNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    /// <summary>Category grouping (e.g., "Brakes", "Filters", "Fluids").</summary>
    public string Category { get; set; } = string.Empty;

    public int QuantityInStock { get; set; }
    public int ReorderThreshold { get; set; }
    public decimal UnitCost { get; set; }

    /// <summary>Warehouse or bin location.</summary>
    public string Location { get; set; } = string.Empty;
}
