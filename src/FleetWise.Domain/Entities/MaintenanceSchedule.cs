using FleetWise.Domain.Enums;

namespace FleetWise.Domain.Entities;

/// <summary>
/// Defines a preventive maintenance schedule for a vehicle, tracking intervals and due dates.
/// </summary>
public class MaintenanceSchedule
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public MaintenanceType MaintenanceType { get; set; }

    /// <summary>Trigger PM at this mileage interval.</summary>
    public int? IntervalMiles { get; set; }

    /// <summary>Trigger PM at this time interval (in days).</summary>
    public int? IntervalDays { get; set; }

    public DateTime? LastCompletedDate { get; set; }
    public int? LastCompletedMileage { get; set; }

    /// <summary>Calculated next due mileage based on last completed + interval.</summary>
    public int? NextDueMileage { get; set; }

    /// <summary>Calculated next due date based on last completed + interval.</summary>
    public DateTime? NextDueDate { get; set; }

    /// <summary>
    /// True if the maintenance is past due by date or mileage.
    /// Computed from NextDueDate/NextDueMileage and the vehicle's current state.
    /// </summary>
    public bool IsOverdue
    {
        get
        {
            if (NextDueDate.HasValue && NextDueDate.Value < DateTime.UtcNow)
                return true;
            if (NextDueMileage.HasValue && Vehicle != null && Vehicle.CurrentMileage >= NextDueMileage.Value)
                return true;
            return false;
        }
    }

    // Navigation property
    public Vehicle Vehicle { get; set; } = null!;
}
