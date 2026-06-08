using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.Infrastructure.Tests;

public abstract class SqliteRepositoryTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly FleetDbContext Context;

    protected SqliteRepositoryTestBase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<FleetDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new FleetDbContext(options);
        Context.Database.EnsureCreated();
    }

    protected static Vehicle NewVehicle(
        int id,
        string asset,
        VehicleStatus status = VehicleStatus.Active,
        string department = "Public Works",
        FuelType fuelType = FuelType.Gasoline,
        string make = "Ford",
        string model = "F-150",
        int currentMileage = 50_000) => new()
    {
        Id = id,
        AssetNumber = asset,
        VIN = $"1VIN{id:D13}",
        Year = 2020,
        Make = make,
        Model = model,
        FuelType = fuelType,
        Status = status,
        Department = department,
        CurrentMileage = currentMileage,
        AcquisitionDate = new DateTime(2020, 1, 1),
        AcquisitionCost = 30_000m,
        LicensePlate = $"PLT{id:D4}",
        Location = "Main Garage"
    };

    protected static WorkOrder NewWorkOrder(
        int id,
        int vehicleId,
        string workOrderNumber,
        WorkOrderStatus status = WorkOrderStatus.Open,
        Priority priority = Priority.Medium,
        DateTime? requestedDate = null) => new()
    {
        Id = id,
        VehicleId = vehicleId,
        WorkOrderNumber = workOrderNumber,
        Status = status,
        Priority = priority,
        Description = $"Work order {workOrderNumber}",
        RequestedDate = requestedDate ?? new DateTime(2026, 1, 1)
    };

    protected static Part NewPart(
        int id,
        string partNumber,
        string name,
        string category,
        int quantityInStock,
        int reorderThreshold) => new()
    {
        Id = id,
        PartNumber = partNumber,
        Name = name,
        Category = category,
        QuantityInStock = quantityInStock,
        ReorderThreshold = reorderThreshold,
        UnitCost = 10m,
        Location = "Bin A1"
    };

    protected static MaintenanceRecord NewMaintenanceRecord(
        int id,
        int vehicleId,
        DateTime performedDate,
        decimal cost = 500m,
        int? workOrderId = null) => new()
    {
        Id = id,
        VehicleId = vehicleId,
        WorkOrderId = workOrderId,
        MaintenanceType = MaintenanceType.OilChange,
        PerformedDate = performedDate,
        MileageAtService = 40_000,
        Description = "Service",
        Cost = cost,
        TechnicianName = "Tech"
    };

    protected static MaintenanceSchedule NewMaintenanceSchedule(
        int id,
        int vehicleId,
        DateTime? nextDueDate = null,
        int? nextDueMileage = null,
        int? intervalMiles = null,
        int? intervalDays = null) => new()
    {
        Id = id,
        VehicleId = vehicleId,
        MaintenanceType = MaintenanceType.OilChange,
        NextDueDate = nextDueDate,
        NextDueMileage = nextDueMileage,
        IntervalMiles = intervalMiles,
        IntervalDays = intervalDays
    };

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
