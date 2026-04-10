using FleetWise.Domain.Enums;

namespace FleetWise.Domain.Entities;

/// <summary>
/// Represents a maintenance or repair work order for a vehicle.
/// </summary>
public class WorkOrder
{
    public int Id { get; set; }

    /// <summary>Human-readable work order number (e.g., "WO-2026-00142").</summary>
    public string WorkOrderNumber { get; set; } = string.Empty;

    public int VehicleId { get; set; }
    public WorkOrderStatus Status { get; set; }
    public Priority Priority { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? AssignedTechnician { get; set; }
    public decimal? LaborHours { get; set; }

    /// <summary>Total cost including parts and labor.</summary>
    public decimal? TotalCost { get; set; }

    public string? Notes { get; set; }

    // Navigation property
    public Vehicle Vehicle { get; set; } = null!;
}
