using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Data;
using FleetWise.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.Infrastructure.Tests;

/// <summary>
/// Regression tests for the maintenance-cost aggregations against a real SQLite
/// provider. The EF InMemory provider (used elsewhere in the suite) silently
/// accepts `GroupBy + Sum(decimal) + OrderByDescending` queries, but SQLite's
/// provider cannot translate them -- "SQLite does not support expressions of
/// type 'decimal' in ORDER BY clauses." These tests pin in-memory aggregation
/// as the required implementation strategy so the bug can't reappear.
/// </summary>
public class MaintenanceCostSqliteTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly FleetDbContext _context;

    public MaintenanceCostSqliteTests()
    {
        // Shared in-memory SQLite DB -- real SQLite provider, no file on disk.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new FleetDbContext(options);
        _context.Database.EnsureCreated();

        SeedData();
    }

    private void SeedData()
    {
        var vehicles = new[]
        {
            NewVehicle(id: 1, asset: "V-001", make: "Ford", model: "F-150"),
            NewVehicle(id: 2, asset: "V-002", make: "Chevy", model: "Silverado"),
            NewVehicle(id: 3, asset: "V-003", make: "Toyota", model: "Tacoma")
        };
        _context.Vehicles.AddRange(vehicles);

        _context.MaintenanceRecords.AddRange(
            NewRecord(vehicleId: 1, cost: 500.50m, date: new DateTime(2025, 1, 15)),
            NewRecord(vehicleId: 1, cost: 1200.00m, date: new DateTime(2025, 3, 1)),
            NewRecord(vehicleId: 2, cost: 250.00m, date: new DateTime(2025, 2, 20)),
            NewRecord(vehicleId: 3, cost: 3000.75m, date: new DateTime(2025, 4, 10)),
            NewRecord(vehicleId: 3, cost: 100.25m, date: new DateTime(2025, 4, 12))
        );
        _context.SaveChanges();
    }

    private static Vehicle NewVehicle(int id, string asset, string make, string model) => new()
    {
        Id = id,
        AssetNumber = asset,
        VIN = $"1VIN{id:D13}",
        Year = 2020,
        Make = make,
        Model = model,
        FuelType = FuelType.Gasoline,
        Status = VehicleStatus.Active,
        Department = "Public Works",
        CurrentMileage = 50000,
        AcquisitionDate = new DateTime(2020, 1, 1),
        AcquisitionCost = 30000m,
        LicensePlate = $"PLT{id:D4}",
        Location = "Main Garage"
    };

    private static MaintenanceRecord NewRecord(int vehicleId, decimal cost, DateTime date) => new()
    {
        VehicleId = vehicleId,
        MaintenanceType = MaintenanceType.OilChange,
        PerformedDate = date,
        MileageAtService = 45000,
        Description = "Test record",
        Cost = cost,
        TechnicianName = "Test Tech"
    };

    [Fact]
    public async Task GetVehiclesByMaintenanceCostAsync_OnSqlite_ReturnsVehiclesOrderedByTotalCost()
    {
        var repo = new VehicleRepository(_context);

        var result = await repo.GetVehiclesByMaintenanceCostAsync(topN: 10);

        Assert.Equal(3, result.Count);
        // V-003: 3000.75 + 100.25 = 3101.00 (highest)
        // V-001: 500.50 + 1200.00 = 1700.50
        // V-002: 250.00
        Assert.Equal("V-003", result[0].AssetNumber);
        Assert.Equal(3101.00m, result[0].TotalMaintenanceCost);
        Assert.Equal(2, result[0].RecordCount);

        Assert.Equal("V-001", result[1].AssetNumber);
        Assert.Equal(1700.50m, result[1].TotalMaintenanceCost);

        Assert.Equal("V-002", result[2].AssetNumber);
        Assert.Equal(250.00m, result[2].TotalMaintenanceCost);
    }

    [Fact]
    public async Task GetVehiclesByMaintenanceCostAsync_OnSqlite_RespectsTopN()
    {
        var repo = new VehicleRepository(_context);

        var result = await repo.GetVehiclesByMaintenanceCostAsync(topN: 2);

        Assert.Equal(2, result.Count);
        Assert.Equal("V-003", result[0].AssetNumber);
        Assert.Equal("V-001", result[1].AssetNumber);
    }

    [Fact]
    public async Task GetCostSummaryAsync_OnSqlite_GroupedByVehicle_OrdersByTotalCostDesc()
    {
        var repo = new MaintenanceRepository(_context);

        var result = await repo.GetCostSummaryAsync("vehicle");

        Assert.Equal(3, result.Count);
        Assert.Equal("V-003", result[0].GroupKey);
        Assert.Equal(3101.00m, result[0].TotalCost);
        Assert.Equal("V-001", result[1].GroupKey);
        Assert.Equal("V-002", result[2].GroupKey);
    }

    [Fact]
    public async Task GetCostSummaryAsync_OnSqlite_GroupedByType_Aggregates()
    {
        var repo = new MaintenanceRepository(_context);

        var result = await repo.GetCostSummaryAsync("type");

        var oil = Assert.Single(result);
        Assert.Equal("OilChange", oil.GroupKey);
        Assert.Equal(5051.50m, oil.TotalCost);
        Assert.Equal(5, oil.RecordCount);
    }

    [Fact]
    public async Task GetCostSummaryAsync_OnSqlite_GroupedByMonth_OrdersByMonthDesc()
    {
        var repo = new MaintenanceRepository(_context);

        var result = await repo.GetCostSummaryAsync("month");

        // Seeded across Jan, Feb, Mar, Apr 2025 -- expect descending month keys.
        Assert.Equal(4, result.Count);
        Assert.Equal("2025-04", result[0].GroupKey);
        Assert.Equal(3101.00m, result[0].TotalCost);
        Assert.Equal("2025-03", result[1].GroupKey);
        Assert.Equal("2025-02", result[2].GroupKey);
        Assert.Equal("2025-01", result[3].GroupKey);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
