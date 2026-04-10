using FleetWise.Domain.Enums;

namespace FleetWise.Domain.Entities;

/// <summary>
/// Represents a fleet vehicle or piece of equipment tracked from acquisition through disposal.
/// </summary>
public class Vehicle
{
    public int Id { get; set; }

    /// <summary>Unique fleet identifier (e.g., "V-2019-0042").</summary>
    public string AssetNumber { get; set; } = string.Empty;

    /// <summary>17-character Vehicle Identification Number.</summary>
    public string VIN { get; set; } = string.Empty;

    public int Year { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public FuelType FuelType { get; set; }
    public VehicleStatus Status { get; set; }

    /// <summary>Assigned department (e.g., "Public Works", "Parks and Recreation").</summary>
    public string Department { get; set; } = string.Empty;

    public string? AssignedDriver { get; set; }
    public int CurrentMileage { get; set; }
    public DateTime AcquisitionDate { get; set; }
    public decimal AcquisitionCost { get; set; }
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>Home base or garage location.</summary>
    public string Location { get; set; } = string.Empty;

    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = [];
    public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = [];
    public ICollection<WorkOrder> WorkOrders { get; set; } = [];
}
