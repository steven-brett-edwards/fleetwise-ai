using FleetWise.Api.Controllers;
using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FleetWise.Api.Tests.Controllers;

/// <summary>
/// Tests for VehiclesController constructed with mocked repositories --
/// the same way DI builds it in production. The controller depends on three
/// repositories (IVehicleRepository, IWorkOrderRepository, IMaintenanceRepository)
/// because it serves vehicle data alongside related maintenance and work order data.
/// </summary>
public class VehiclesControllerTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository = new();
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository = new();
    private readonly Mock<IMaintenanceRepository> _mockMaintenanceRepository = new();

    private VehiclesController CreateVehiclesControllerWithMockedRepositories()
    {
        return new VehiclesController(
            _mockVehicleRepository.Object,
            _mockWorkOrderRepository.Object,
            _mockMaintenanceRepository.Object);
    }

    private static Vehicle CreateTestVehicle(int id = 1, string assetNumber = "V-2019-0001") => new()
    {
        Id = id,
        AssetNumber = assetNumber,
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

    // ── GetAll ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WhenNoFiltersProvided_ReturnsOkWithAllVehicles()
    {
        // Setup
        var twoVehicles = new List<Vehicle>
        {
            CreateTestVehicle(1, "V-2019-0001"),
            CreateTestVehicle(2, "V-2020-0002")
        };

        _mockVehicleRepository
            .Setup(r => r.GetAllAsync(null, null, null))
            .ReturnsAsync(twoVehicles);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetAll();

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedVehicles = Assert.IsType<List<Vehicle>>(okResult.Value);
        returnedVehicles.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WhenStatusFilterProvided_PassesFilterToRepository()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetAllAsync(VehicleStatus.Active, null, null))
            .ReturnsAsync(new List<Vehicle>());

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        await vehiclesControllerWithMockedRepositories.GetAll(status: VehicleStatus.Active);

        // Result
        _mockVehicleRepository.Verify(
            r => r.GetAllAsync(VehicleStatus.Active, null, null), Times.Once);
    }

    // ── GetById ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_WhenVehicleExists_ReturnsOkWithVehicle()
    {
        // Setup
        var existingVehicle = CreateTestVehicle(1, "V-2019-0001");
        _mockVehicleRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingVehicle);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetById(1);

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedVehicle = Assert.IsType<Vehicle>(okResult.Value);
        returnedVehicle.AssetNumber.Should().Be("V-2019-0001");
    }

    [Fact]
    public async Task GetById_WhenVehicleDoesNotExist_ReturnsNotFound()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Vehicle?)null);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetById(999);

        // Result
        Assert.IsType<NotFoundResult>(actionResult);
    }

    // ── GetMaintenanceHistory ───────────────────────────────────────

    [Fact]
    public async Task GetMaintenanceHistory_WhenVehicleExists_ReturnsOkWithMaintenanceRecords()
    {
        // Setup
        var existingVehicle = CreateTestVehicle(1);
        _mockVehicleRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingVehicle);

        var twoMaintenanceRecords = new List<MaintenanceRecord>
        {
            new()
            {
                Id = 1, VehicleId = 1, Vehicle = existingVehicle,
                MaintenanceType = MaintenanceType.OilChange,
                PerformedDate = new DateTime(2025, 6, 1),
                MileageAtService = 85000,
                Description = "Full synthetic oil change",
                Cost = 89.99m, TechnicianName = "Mike Torres"
            }
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetByVehicleIdAsync(1))
            .ReturnsAsync(twoMaintenanceRecords);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetMaintenanceHistory(1);

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedRecords = Assert.IsType<List<MaintenanceRecord>>(okResult.Value);
        returnedRecords.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMaintenanceHistory_WhenVehicleDoesNotExist_ReturnsNotFound()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Vehicle?)null);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetMaintenanceHistory(999);

        // Result
        Assert.IsType<NotFoundResult>(actionResult);
    }

    // ── GetWorkOrders ───────────────────────────────────────────────

    [Fact]
    public async Task GetWorkOrders_WhenVehicleExists_ReturnsOkWithWorkOrders()
    {
        // Setup
        var existingVehicle = CreateTestVehicle(1);
        _mockVehicleRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingVehicle);

        var oneWorkOrder = new List<WorkOrder>
        {
            new()
            {
                Id = 1, WorkOrderNumber = "WO-2026-00025",
                VehicleId = 1, Vehicle = existingVehicle,
                Status = WorkOrderStatus.Open, Priority = Priority.High,
                Description = "Brake inspection",
                RequestedDate = new DateTime(2026, 4, 1),
                AssignedTechnician = "Mike Torres"
            }
        };

        _mockWorkOrderRepository
            .Setup(r => r.GetByVehicleIdAsync(1))
            .ReturnsAsync(oneWorkOrder);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetWorkOrders(1);

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedWorkOrders = Assert.IsType<List<WorkOrder>>(okResult.Value);
        returnedWorkOrders.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetWorkOrders_WhenVehicleDoesNotExist_ReturnsNotFound()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Vehicle?)null);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetWorkOrders(999);

        // Result
        Assert.IsType<NotFoundResult>(actionResult);
    }

    // ── GetSummary ──────────────────────────────────────────────────

    [Fact]
    public async Task GetSummary_ReturnsOkWithFleetSummary()
    {
        // Setup
        var fleetSummary = new FleetSummary(
            35,
            new Dictionary<string, int> { ["Active"] = 30, ["InShop"] = 5 },
            new Dictionary<string, int> { ["Gasoline"] = 20, ["Diesel"] = 15 },
            new Dictionary<string, int> { ["Public Works"] = 35 }
        );

        _mockVehicleRepository
            .Setup(r => r.GetFleetSummaryAsync())
            .ReturnsAsync(fleetSummary);

        var vehiclesControllerWithMockedRepositories = CreateVehiclesControllerWithMockedRepositories();

        // Act
        var actionResult = await vehiclesControllerWithMockedRepositories.GetSummary();

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedSummary = Assert.IsType<FleetSummary>(okResult.Value);
        returnedSummary.TotalVehicles.Should().Be(35);
    }
}
