using System.Text.Json;
using FleetWise.Api.Controllers;
using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FleetWise.Api.Tests.Controllers;

/// <summary>
/// Tests for MaintenanceController constructed with a mocked IMaintenanceRepository --
/// the same way DI builds it in production. This controller projects
/// MaintenanceSchedule entities into anonymous types before returning,
/// so tests serialize the result to JSON to verify the projection shape.
/// </summary>
public class MaintenanceControllerTests
{
    private readonly Mock<IMaintenanceRepository> _mockMaintenanceRepository = new();

    private MaintenanceController CreateMaintenanceControllerWithMockedRepository()
    {
        return new MaintenanceController(_mockMaintenanceRepository.Object);
    }

    private static Vehicle CreateTestVehicle() => new()
    {
        Id = 1,
        AssetNumber = "V-2019-0001",
        VIN = "1FTFW1E50KFA00001",
        Year = 2019,
        Make = "Ford",
        Model = "F-150 XL",
        FuelType = FuelType.Gasoline,
        Status = VehicleStatus.Active,
        Department = "Public Works",
        CurrentMileage = 87432,
        AcquisitionDate = new DateTime(2019, 3, 15),
        AcquisitionCost = 35000m,
        LicensePlate = "GOV-0001",
        Location = "Main Garage"
    };

    // ── GetOverdue ──────────────────────────────────────────────────

    [Fact]
    public async Task GetOverdue_WhenOverdueSchedulesExist_ReturnsOkWithProjectedResults()
    {
        // Setup
        var vehicleWithOverdueMaintenance = CreateTestVehicle();
        var oneOverdueSchedule = new List<MaintenanceSchedule>
        {
            new()
            {
                Id = 1, VehicleId = 1, Vehicle = vehicleWithOverdueMaintenance,
                MaintenanceType = MaintenanceType.OilChange,
                NextDueDate = new DateTime(2025, 12, 1),
                NextDueMileage = 85000,
                LastCompletedDate = new DateTime(2025, 6, 1),
                LastCompletedMileage = 80000
            }
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetOverdueSchedulesAsync())
            .ReturnsAsync(oneOverdueSchedule);

        var maintenanceControllerWithMockedRepository = CreateMaintenanceControllerWithMockedRepository();

        // Act
        var actionResult = await maintenanceControllerWithMockedRepository.GetOverdue();

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var projectedJson = JsonSerializer.Serialize(okResult.Value);
        projectedJson.Should().Contain("V-2019-0001");
        projectedJson.Should().Contain("2019 Ford F-150 XL");
        projectedJson.Should().Contain("OilChange");
        projectedJson.Should().Contain("87432");
    }

    [Fact]
    public async Task GetOverdue_WhenNoOverdueSchedules_ReturnsOkWithEmptyCollection()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetOverdueSchedulesAsync())
            .ReturnsAsync(new List<MaintenanceSchedule>());

        var maintenanceControllerWithMockedRepository = CreateMaintenanceControllerWithMockedRepository();

        // Act
        var actionResult = await maintenanceControllerWithMockedRepository.GetOverdue();

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var projectedJson = JsonSerializer.Serialize(okResult.Value);
        projectedJson.Should().Be("[]");
    }

    // ── GetUpcoming ─────────────────────────────────────────────────

    [Fact]
    public async Task GetUpcoming_WithDefaultParameters_ReturnsOkWithProjectedSchedules()
    {
        // Setup
        var vehicleDueForService = CreateTestVehicle();
        var oneUpcomingSchedule = new List<MaintenanceSchedule>
        {
            new()
            {
                Id = 2, VehicleId = 1, Vehicle = vehicleDueForService,
                MaintenanceType = MaintenanceType.TireRotation,
                NextDueDate = DateTime.UtcNow.AddDays(15),
                NextDueMileage = 90000
            }
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetUpcomingSchedulesAsync(30, 5000))
            .ReturnsAsync(oneUpcomingSchedule);

        var maintenanceControllerWithMockedRepository = CreateMaintenanceControllerWithMockedRepository();

        // Act
        var actionResult = await maintenanceControllerWithMockedRepository.GetUpcoming();

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var projectedJson = JsonSerializer.Serialize(okResult.Value);
        projectedJson.Should().Contain("V-2019-0001");
        projectedJson.Should().Contain("TireRotation");
    }

    [Fact]
    public async Task GetUpcoming_WithCustomParameters_PassesParametersToRepository()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetUpcomingSchedulesAsync(7, 1000))
            .ReturnsAsync(new List<MaintenanceSchedule>());

        var maintenanceControllerWithMockedRepository = CreateMaintenanceControllerWithMockedRepository();

        // Act
        await maintenanceControllerWithMockedRepository.GetUpcoming(days: 7, miles: 1000);

        // Result
        _mockMaintenanceRepository.Verify(
            r => r.GetUpcomingSchedulesAsync(7, 1000), Times.Once);
    }
}
