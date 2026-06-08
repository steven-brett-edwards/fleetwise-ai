using FleetWise.Infrastructure.Repositories;
using FluentAssertions;

namespace FleetWise.Infrastructure.Tests;

public class MaintenanceRepositoryTests : SqliteRepositoryTestBase
{
    private readonly MaintenanceRepository _repository;

    public MaintenanceRepositoryTests()
    {
        Context.Vehicles.AddRange(
            NewVehicle(1, "V-001", currentMileage: 60_000),
            NewVehicle(2, "V-002", currentMileage: 98_000)
        );

        Context.MaintenanceRecords.AddRange(
            NewMaintenanceRecord(1, vehicleId: 1, performedDate: new DateTime(2026, 3, 1)),
            NewMaintenanceRecord(2, vehicleId: 1, performedDate: new DateTime(2026, 1, 1)),
            NewMaintenanceRecord(3, vehicleId: 2, performedDate: new DateTime(2026, 2, 15))
        );

        Context.MaintenanceSchedules.AddRange(
            // Overdue by date: NextDueDate in the past
            NewMaintenanceSchedule(1, vehicleId: 1, nextDueDate: DateTime.UtcNow.AddDays(-10)),
            // Overdue by mileage: vehicle at 60000, NextDueMileage = 55000
            NewMaintenanceSchedule(2, vehicleId: 1, nextDueMileage: 55_000),
            // Upcoming by date: within 30 days from now
            NewMaintenanceSchedule(3, vehicleId: 2, nextDueDate: DateTime.UtcNow.AddDays(15)),
            // Upcoming by mileage: vehicle at 98000, NextDueMileage = 100000, within 5000 miles
            NewMaintenanceSchedule(4, vehicleId: 2, nextDueMileage: 100_000),
            // Not due: NextDueDate far in the future
            NewMaintenanceSchedule(5, vehicleId: 1, nextDueDate: DateTime.UtcNow.AddDays(90))
        );

        Context.SaveChanges();

        _repository = new MaintenanceRepository(Context);
    }

    // ── GetByVehicleIdAsync ────────────────────────────────────────

    [Fact]
    public async Task GetByVehicleIdAsync_ReturnsOnlyRecordsForThatVehicleOrderedMostRecentFirst()
    {
        // Act
        var vehicleOneRecords = await _repository.GetByVehicleIdAsync(vehicleId: 1);

        // Result
        vehicleOneRecords.Should().HaveCount(2);
        vehicleOneRecords.Should().AllSatisfy(r => r.VehicleId.Should().Be(1));
        vehicleOneRecords.Select(r => r.PerformedDate).Should().BeInDescendingOrder();
    }

    // ── GetOverdueSchedulesAsync ───────────────────────────────────

    [Fact]
    public async Task GetOverdueSchedulesAsync_ReturnsDateAndMileageOverdueExcludesFutureDue()
    {
        // Setup (seeded in constructor):
        //   Schedule 1: NextDueDate 10 days ago             → overdue by date
        //   Schedule 2: Vehicle at 60 000, NextDue = 55 000 → overdue by mileage
        //   Schedule 5: NextDueDate 90 days from now        → not due yet

        // Act
        var overdueSchedules = await _repository.GetOverdueSchedulesAsync();

        // Result
        overdueSchedules.Should().Contain(s => s.Id == 1, "schedule 1 is overdue by date");
        overdueSchedules.Should().Contain(s => s.Id == 2, "schedule 2 is overdue by mileage");
        overdueSchedules.Should().NotContain(s => s.Id == 5, "schedule 5 is not due for 90 days");
    }

    // ── GetUpcomingSchedulesAsync ──────────────────────────────────

    [Fact]
    public async Task GetUpcomingSchedulesAsync_ReturnsDateAndMileageUpcomingExcludesFarFuture()
    {
        // Setup (seeded in constructor):
        //   Schedule 3: NextDueDate 15 days from now                              → upcoming by date (within 30-day window)
        //   Schedule 4: Vehicle at 98 000, NextDue = 100 000, within 5 000 miles  → upcoming by mileage
        //   Schedule 5: NextDueDate 90 days from now                              → outside window

        // Act
        var upcomingSchedules = await _repository.GetUpcomingSchedulesAsync();

        // Result
        upcomingSchedules.Should().Contain(s => s.Id == 3, "schedule 3 is due within 30 days");
        upcomingSchedules.Should().Contain(s => s.Id == 4, "schedule 4 is within 5 000-mile window");
        upcomingSchedules.Should().NotContain(s => s.Id == 5, "schedule 5 is 90 days out, beyond the window");
    }

    // ── GetCostSummaryAsync (unknown groupBy fallback) ─────────────

    [Fact]
    public async Task GetCostSummaryAsync_WithUnknownGroupByKey_FallsBackToVehicleGrouping()
    {
        // Setup (3 records: 2 for V-001, 1 for V-002)

        // Act
        var summary = await _repository.GetCostSummaryAsync(groupBy: "unknown_key");

        // Result -- unknown key falls back to vehicle grouping
        summary.Should().HaveCount(2);
        summary.Should().AllSatisfy(g => g.GroupKey.Should().StartWith("V-"));
        summary.Select(g => g.TotalCost).Should().BeInDescendingOrder();
    }
}
