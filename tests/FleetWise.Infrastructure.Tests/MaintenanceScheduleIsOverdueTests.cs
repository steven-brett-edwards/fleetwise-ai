using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FluentAssertions;

namespace FleetWise.Infrastructure.Tests;

public class MaintenanceScheduleIsOverdueTests
{
    private static Vehicle VehicleWithMileage(int mileage) => new()
    {
        AssetNumber = "V-001",
        VIN = "1VIN0000000000001",
        Year = 2020,
        Make = "Ford",
        Model = "F-150",
        FuelType = FuelType.Gasoline,
        Status = VehicleStatus.Active,
        Department = "Public Works",
        CurrentMileage = mileage,
        AcquisitionDate = new DateTime(2020, 1, 1),
        AcquisitionCost = 30_000m,
        LicensePlate = "PLT0001",
        Location = "Main Garage"
    };

    // ── Date-based overdue ─────────────────────────────────────────

    [Fact]
    public void IsOverdue_WhenNextDueDateIsPast_ReturnsTrue()
    {
        // Setup
        var overdueByDateSchedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(-1),
            Vehicle = VehicleWithMileage(50_000)
        };

        // Result
        overdueByDateSchedule.IsOverdue.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_WhenNextDueDateIsInFuture_ReturnsFalse()
    {
        // Setup
        var notYetDueByDateSchedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(30),
            Vehicle = VehicleWithMileage(50_000)
        };

        // Result
        notYetDueByDateSchedule.IsOverdue.Should().BeFalse();
    }

    // ── Mileage-based overdue ──────────────────────────────────────

    [Fact]
    public void IsOverdue_WhenCurrentMileageReachesNextDueMileage_ReturnsTrue()
    {
        // Setup
        var overdueByMileageSchedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(30),
            NextDueMileage = 55_000,
            Vehicle = VehicleWithMileage(60_000)
        };

        // Result
        overdueByMileageSchedule.IsOverdue.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_WhenCurrentMileageBelowNextDueMileage_ReturnsFalse()
    {
        // Setup
        var notYetDueByMileageSchedule = new MaintenanceSchedule
        {
            NextDueDate = DateTime.UtcNow.AddDays(30),
            NextDueMileage = 100_000,
            Vehicle = VehicleWithMileage(60_000)
        };

        // Result
        notYetDueByMileageSchedule.IsOverdue.Should().BeFalse();
    }

    // ── Vehicle null guard ─────────────────────────────────────────

    [Fact]
    public void IsOverdue_WhenVehicleIsNullAndOnlyMileageTriggerSet_ReturnsFalse()
    {
        // Setup (Vehicle is null -- mileage check is skipped per the null guard)
        var scheduleWithNullVehicle = new MaintenanceSchedule
        {
            NextDueMileage = 55_000,
            Vehicle = null!
        };

        // Result
        scheduleWithNullVehicle.IsOverdue.Should().BeFalse();
    }

    // ── No triggers set ────────────────────────────────────────────

    [Fact]
    public void IsOverdue_WhenNeitherNextDueDateNorMileageIsSet_ReturnsFalse()
    {
        // Setup
        var scheduleWithNoTriggers = new MaintenanceSchedule
        {
            NextDueDate = null,
            NextDueMileage = null,
            Vehicle = VehicleWithMileage(50_000)
        };

        // Result
        scheduleWithNoTriggers.IsOverdue.Should().BeFalse();
    }
}
