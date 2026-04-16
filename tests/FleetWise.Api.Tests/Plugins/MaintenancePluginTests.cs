using FleetWise.Api.Plugins;
using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.SemanticKernel;
using Moq;

namespace FleetWise.Api.Tests.Plugins;

/// <summary>
/// Tests for MaintenancePlugin invoked through the Semantic Kernel.
/// Navigation properties (Vehicle) must be populated in mock data because
/// the plugin's projections access Vehicle.AssetNumber, Year, Make, Model, etc.
/// </summary>
public class MaintenancePluginTests
{
    private readonly Mock<IMaintenanceRepository> _mockMaintenanceRepository = new();
    private readonly Mock<IVehicleRepository> _mockVehicleRepository = new();

    private Kernel CreateKernelWithMaintenancePlugin()
    {
        var kernel = Kernel.CreateBuilder().Build();
        kernel.ImportPluginFromObject(
            new MaintenancePlugin(_mockMaintenanceRepository.Object, _mockVehicleRepository.Object),
            "Maintenance");
        return kernel;
    }

    private static Vehicle CreateTestVehicle(string assetNumber = "V-2019-0001", int currentMileage = 87432) => new()
    {
        Id = 1,
        AssetNumber = assetNumber,
        VIN = "1FTFW1E50KFA00001",
        Year = 2019,
        Make = "Ford",
        Model = "F-150 XL",
        FuelType = FuelType.Gasoline,
        Status = VehicleStatus.Active,
        Department = "Public Works",
        CurrentMileage = currentMileage,
        AcquisitionDate = new DateTime(2019, 3, 15),
        AcquisitionCost = 35000m,
        LicensePlate = "GOV-0001",
        Location = "Main Garage"
    };

    // ── get_overdue_maintenance ──────────────────────────────────────

    [Fact]
    public async Task GetOverdueMaintenance_WhenNoOverdueSchedules_ReturnsNoOverdueMessage()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetOverdueSchedulesAsync())
            .ReturnsAsync(new List<MaintenanceSchedule>());

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var overdueMaintenanceResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_overdue_maintenance");

        // Result
        overdueMaintenanceResult.ToString().Should().Be("No overdue maintenance schedules found.");
    }

    [Fact]
    public async Task GetOverdueMaintenance_WhenOverdueSchedulesExist_ReturnsFormattedSchedulesWithVehicleDetails()
    {
        // Setup
        var vehicleWithOverdueOilChange = CreateTestVehicle("V-2019-0001", 92000);
        var twoOverdueMaintenanceSchedules = new List<MaintenanceSchedule>
        {
            new()
            {
                Id = 1,
                VehicleId = 1,
                Vehicle = vehicleWithOverdueOilChange,
                MaintenanceType = MaintenanceType.OilChange,
                NextDueDate = new DateTime(2025, 12, 1),
                NextDueMileage = 90000,
                LastCompletedDate = new DateTime(2025, 6, 1)
            },
            new()
            {
                Id = 2,
                VehicleId = 1,
                Vehicle = vehicleWithOverdueOilChange,
                MaintenanceType = MaintenanceType.TireRotation,
                NextDueDate = new DateTime(2025, 11, 15),
                NextDueMileage = 88000,
                LastCompletedDate = new DateTime(2025, 5, 15)
            }
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetOverdueSchedulesAsync())
            .ReturnsAsync(twoOverdueMaintenanceSchedules);

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var overdueMaintenanceResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_overdue_maintenance");

        // Result
        var overdueMaintenanceJsonResponse = overdueMaintenanceResult.ToString();
        overdueMaintenanceJsonResponse.Should().Contain("Found 2 overdue maintenance schedules");
        overdueMaintenanceJsonResponse.Should().Contain("V-2019-0001");
        overdueMaintenanceJsonResponse.Should().Contain("2019 Ford F-150 XL");
        overdueMaintenanceJsonResponse.Should().Contain("92000");

        overdueMaintenanceJsonResponse.Should().Contain("OilChange"); // First schedule
        overdueMaintenanceJsonResponse.Should().Contain("90000");

        overdueMaintenanceJsonResponse.Should().Contain("TireRotation"); // Second schedule
        overdueMaintenanceJsonResponse.Should().Contain("88000");
    }

    // ── get_upcoming_maintenance ─────────────────────────────────────

    [Fact]
    public async Task GetUpcomingMaintenance_WhenNoUpcomingSchedules_ReturnsMessageWithDayAndMileageParameters()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetUpcomingSchedulesAsync(30, 5000))
            .ReturnsAsync(new List<MaintenanceSchedule>());

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var upcomingMaintenanceResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_upcoming_maintenance");

        // Result
        var noUpcomingSchedulesMessage = upcomingMaintenanceResult.ToString();
        noUpcomingSchedulesMessage.Should().Contain("30 days");
        noUpcomingSchedulesMessage.Should().Contain("5000 miles");
    }

    [Fact]
    public async Task GetUpcomingMaintenance_WhenUpcomingSchedulesExist_ReturnsFormattedSchedules()
    {
        // Setup
        var vehicleDueForBrakeInspection = CreateTestVehicle("V-2020-0005", 48000);
        var oneUpcomingBrakeInspection = new List<MaintenanceSchedule>
        {
            new()
            {
                Id = 3,
                VehicleId = 5,
                Vehicle = vehicleDueForBrakeInspection,
                MaintenanceType = MaintenanceType.BrakeInspection,
                NextDueDate = DateTime.UtcNow.AddDays(15),
                NextDueMileage = 50000
            }
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetUpcomingSchedulesAsync(30, 5000))
            .ReturnsAsync(oneUpcomingBrakeInspection);

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var upcomingMaintenanceResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_upcoming_maintenance");

        // Result
        var upcomingMaintenanceJsonResponse = upcomingMaintenanceResult.ToString();
        upcomingMaintenanceJsonResponse.Should().Contain("Found 1 upcoming maintenance schedules");
        upcomingMaintenanceJsonResponse.Should().Contain("V-2020-0005");
        upcomingMaintenanceJsonResponse.Should().Contain("BrakeInspection");
    }

    [Fact]
    public async Task GetUpcomingMaintenance_WhenCustomParametersProvided_PassesValuesToRepository()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetUpcomingSchedulesAsync(7, 1000))
            .ReturnsAsync(new List<MaintenanceSchedule>());

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var upcomingMaintenanceResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_upcoming_maintenance",
            new KernelArguments { ["withinDays"] = 7, ["withinMiles"] = 1000 });

        // Result
        upcomingMaintenanceResult.ToString().Should().Contain("7 days or 1000 miles");
        _mockMaintenanceRepository.Verify(r => r.GetUpcomingSchedulesAsync(7, 1000), Times.Once);
    }

    // ── get_vehicle_maintenance_history ──────────────────────────────

    [Fact]
    public async Task GetVehicleMaintenanceHistory_WhenNoRecordsFound_ReturnsNoRecordsMessageWithVehicleId()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetByVehicleIdAsync(99))
            .ReturnsAsync(new List<MaintenanceRecord>());

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var maintenanceHistoryResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_vehicle_maintenance_history",
            new KernelArguments { ["vehicleId"] = 99 });

        // Result
        maintenanceHistoryResult.ToString().Should().Be("No maintenance records found for vehicle ID 99.");
    }

    [Fact]
    public async Task GetVehicleMaintenanceHistory_WhenRecordsExist_ReturnsFormattedHistory()
    {
        // Setup
        var vehicleForHistoryLookup = CreateTestVehicle();
        var twoMaintenanceRecordsForVehicleOne = new List<MaintenanceRecord>
        {
            new()
            {
                Id = 1, VehicleId = 1, Vehicle = vehicleForHistoryLookup,
                MaintenanceType = MaintenanceType.OilChange,
                PerformedDate = new DateTime(2025, 6, 1),
                MileageAtService = 85000,
                Description = "Full synthetic oil change",
                Cost = 89.99m,
                TechnicianName = "Mike Torres"
            },
            new()
            {
                Id = 2, VehicleId = 1, Vehicle = vehicleForHistoryLookup,
                MaintenanceType = MaintenanceType.TireRotation,
                PerformedDate = new DateTime(2025, 3, 15),
                MileageAtService = 80000,
                Description = "Rotated all four tires",
                Cost = 45.00m,
                TechnicianName = "Sarah Chen"
            }
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetByVehicleIdAsync(1))
            .ReturnsAsync(twoMaintenanceRecordsForVehicleOne);

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var maintenanceHistoryResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_vehicle_maintenance_history",
            new KernelArguments { ["vehicleId"] = 1 });

        // Result
        var maintenanceHistoryJsonResponse = maintenanceHistoryResult.ToString();
        maintenanceHistoryJsonResponse.Should().Contain("Found 2 maintenance records for vehicle ID 1");

        maintenanceHistoryJsonResponse.Should().Contain("OilChange"); // First record
        maintenanceHistoryJsonResponse.Should().Contain("Mike Torres");
        maintenanceHistoryJsonResponse.Should().Contain("89.99");

        maintenanceHistoryJsonResponse.Should().Contain("TireRotation"); // Second record
        maintenanceHistoryJsonResponse.Should().Contain("Sarah Chen");
        maintenanceHistoryJsonResponse.Should().Contain("45");
    }

    // ── get_maintenance_history_by_asset_number ──────────────────────

    [Fact]
    public async Task GetMaintenanceHistoryByAssetNumber_WhenVehicleNotFound_ReturnsNoVehicleMessageWithAssetNumber()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetByAssetNumberAsync("V-9999-9999"))
            .ReturnsAsync((Vehicle?)null);

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var historyResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_history_by_asset_number",
            new KernelArguments { ["assetNumber"] = "V-9999-9999" });

        // Result
        historyResult.ToString().Should().Be("No vehicle found with asset number V-9999-9999.");
        _mockMaintenanceRepository.Verify(r => r.GetByVehicleIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetMaintenanceHistoryByAssetNumber_WhenVehicleFoundButNoRecords_ReturnsNoRecordsMessageWithAssetNumber()
    {
        // Setup
        var vehicleWithNoHistory = CreateTestVehicle(assetNumber: "V-2024-0030");
        vehicleWithNoHistory.Id = 30;

        _mockVehicleRepository
            .Setup(r => r.GetByAssetNumberAsync("V-2024-0030"))
            .ReturnsAsync(vehicleWithNoHistory);
        _mockMaintenanceRepository
            .Setup(r => r.GetByVehicleIdAsync(30))
            .ReturnsAsync(new List<MaintenanceRecord>());

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var historyResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_history_by_asset_number",
            new KernelArguments { ["assetNumber"] = "V-2024-0030" });

        // Result
        historyResult.ToString().Should().Be("No maintenance records found for vehicle V-2024-0030.");
    }

    [Fact]
    public async Task GetMaintenanceHistoryByAssetNumber_WhenRecordsExist_ReturnsFormattedHistory()
    {
        // Setup -- asset number resolves to internal vehicle Id, and records are loaded by that Id
        var vehicleForHistoryLookup = CreateTestVehicle(assetNumber: "V-2017-0007");
        vehicleForHistoryLookup.Id = 7;

        var twoMaintenanceRecordsForVehicleSeven = new List<MaintenanceRecord>
        {
            new()
            {
                Id = 1, VehicleId = 7, Vehicle = vehicleForHistoryLookup,
                MaintenanceType = MaintenanceType.OilChange,
                PerformedDate = new DateTime(2026, 1, 20),
                MileageAtService = 141000,
                Description = "Full synthetic oil change",
                Cost = 68.00m,
                TechnicianName = "Sarah Chen"
            },
            new()
            {
                Id = 2, VehicleId = 7, Vehicle = vehicleForHistoryLookup,
                MaintenanceType = MaintenanceType.BrakeInspection,
                PerformedDate = new DateTime(2025, 11, 5),
                MileageAtService = 138500,
                Description = "Front brake pads replaced",
                Cost = 320.50m,
                TechnicianName = "Mike Torres"
            }
        };

        _mockVehicleRepository
            .Setup(r => r.GetByAssetNumberAsync("V-2017-0007"))
            .ReturnsAsync(vehicleForHistoryLookup);
        _mockMaintenanceRepository
            .Setup(r => r.GetByVehicleIdAsync(7))
            .ReturnsAsync(twoMaintenanceRecordsForVehicleSeven);

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var historyResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_history_by_asset_number",
            new KernelArguments { ["assetNumber"] = "V-2017-0007" });

        // Result
        var historyJsonResponse = historyResult.ToString();
        historyJsonResponse.Should().Contain("Found 2 maintenance records for vehicle V-2017-0007");

        historyJsonResponse.Should().Contain("OilChange");
        historyJsonResponse.Should().Contain("Sarah Chen");
        historyJsonResponse.Should().Contain("68");
        
        historyJsonResponse.Should().Contain("BrakeInspection");
        historyJsonResponse.Should().Contain("Mike Torres");
        historyJsonResponse.Should().Contain("320.5");
    }

    // ── get_maintenance_cost_summary ─────────────────────────────────

    [Fact]
    public async Task GetMaintenanceCostSummary_WhenInvalidGroupByProvided_ReturnsErrorWithValidValues()
    {
        // Setup
        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var costSummaryResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_cost_summary",
            new KernelArguments { ["groupBy"] = "color" });

        // Result
        var invalidGroupByErrorMessage = costSummaryResult.ToString();
        invalidGroupByErrorMessage.Should().Contain("Invalid groupBy value 'color'");
        invalidGroupByErrorMessage.Should().Contain("vehicle, type, month");
    }

    [Fact]
    public async Task GetMaintenanceCostSummary_WhenGroupByProvidedWithDifferentCasing_AcceptsValueCaseInsensitively()
    {
        // Setup
        var oneVehicleCostGroup = new List<MaintenanceCostGroup>
        {
            new("V-2019-0001", 5200.00m, 8)
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetCostSummaryAsync("Vehicle"))
            .ReturnsAsync(oneVehicleCostGroup);

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var costSummaryResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_cost_summary",
            new KernelArguments { ["groupBy"] = "Vehicle" });

        // Result
        costSummaryResult.ToString().Should().Contain("Maintenance costs grouped by Vehicle");
    }

    [Fact]
    public async Task GetMaintenanceCostSummary_WhenNoCostData_ReturnsNoDataMessage()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetCostSummaryAsync("vehicle"))
            .ReturnsAsync(new List<MaintenanceCostGroup>());

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var costSummaryResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_cost_summary");

        // Result
        costSummaryResult.ToString().Should().Be("No maintenance cost data available.");
    }

    [Fact]
    public async Task GetMaintenanceCostSummary_WhenCostDataExists_ReturnsFormattedGroupedCosts()
    {
        // Setup
        var threeCostGroupsByMaintenanceType = new List<MaintenanceCostGroup>
        {
            new("OilChange", 2500.00m, 15),
            new("BrakeInspection", 1800.00m, 6),
            new("TireRotation", 900.00m, 12)
        };

        _mockMaintenanceRepository
            .Setup(r => r.GetCostSummaryAsync("type"))
            .ReturnsAsync(threeCostGroupsByMaintenanceType);

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        var costSummaryResult = await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_cost_summary",
            new KernelArguments { ["groupBy"] = "type" });

        // Result
        var costSummaryJsonResponse = costSummaryResult.ToString();
        costSummaryJsonResponse.Should().Contain("Maintenance costs grouped by type (3 groups)");

        costSummaryJsonResponse.Should().Contain("OilChange"); // First group
        costSummaryJsonResponse.Should().Contain("2500");

        costSummaryJsonResponse.Should().Contain("BrakeInspection"); // Second group
        costSummaryJsonResponse.Should().Contain("1800");

        costSummaryJsonResponse.Should().Contain("TireRotation"); // Third group
        costSummaryJsonResponse.Should().Contain("900");
    }

    [Fact]
    public async Task GetMaintenanceCostSummary_WhenNoGroupByProvided_DefaultsToVehicle()
    {
        // Setup
        _mockMaintenanceRepository
            .Setup(r => r.GetCostSummaryAsync("vehicle"))
            .ReturnsAsync(new List<MaintenanceCostGroup>());

        var kernelWithMaintenancePlugin = CreateKernelWithMaintenancePlugin();

        // Act
        await kernelWithMaintenancePlugin.InvokeAsync(
            "Maintenance", "get_maintenance_cost_summary");

        // Result
        _mockMaintenanceRepository.Verify(r => r.GetCostSummaryAsync("vehicle"), Times.Once);
    }
}
