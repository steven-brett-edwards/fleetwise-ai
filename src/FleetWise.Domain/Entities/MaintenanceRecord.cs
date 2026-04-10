using FleetWise.Domain.Enums;

namespace FleetWise.Domain.Entities;

/// <summary>
/// A completed maintenance service record for a vehicle.
/// </summary>
public class MaintenanceRecord
{
    public int Id { get; set; }
    public int VehicleId { get; set; }

    /// <summary>Nullable -- some records predate the work order system.</summary>
    public int? WorkOrderId { get; set; }

    public MaintenanceType MaintenanceType { get; set; }
    public DateTime PerformedDate { get; set; }
    public int MileageAtService { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string TechnicianName { get; set; } = string.Empty;

    // Navigation properties
    public Vehicle Vehicle { get; set; } = null!;
    public WorkOrder? WorkOrder { get; set; }
}
