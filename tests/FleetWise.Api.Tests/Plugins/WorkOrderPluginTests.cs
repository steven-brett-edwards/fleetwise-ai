using FleetWise.Api.Plugins;
using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.SemanticKernel;
using Moq;

namespace FleetWise.Api.Tests.Plugins;

/// <summary>
/// Tests for WorkOrderPlugin invoked through the Semantic Kernel.
/// This plugin depends on two repositories (IWorkOrderRepository + IPartRepository),
/// showing that a single SK plugin can aggregate multiple data sources.
/// </summary>
public class WorkOrderPluginTests
{
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository = new();
    private readonly Mock<IPartRepository> _mockPartRepository = new();

    private Kernel CreateKernelWithWorkOrderPlugin()
    {
        var kernel = Kernel.CreateBuilder().Build();
        kernel.ImportPluginFromObject(
            new WorkOrderPlugin(_mockWorkOrderRepository.Object, _mockPartRepository.Object), "WorkOrder");
        return kernel;
    }

    private static Vehicle CreateTestVehicle(string assetNumber = "V-2019-0001") => new()
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
        CurrentMileage = 87432,
        AcquisitionDate = new DateTime(2019, 3, 15),
        AcquisitionCost = 35000m,
        LicensePlate = "GOV-0001",
        Location = "Main Garage"
    };

    // ── get_open_work_orders ─────────────────────────────────────────

    [Fact]
    public async Task GetOpenWorkOrders_WhenNoOpenWorkOrders_ReturnsNoOpenWorkOrdersMessage()
    {
        // Setup
        _mockWorkOrderRepository
            .Setup(r => r.GetOpenWorkOrdersAsync())
            .ReturnsAsync(new List<WorkOrder>());

        var kernelWithWorkOrderPlugin = CreateKernelWithWorkOrderPlugin();

        // Act
        var openWorkOrdersResult = await kernelWithWorkOrderPlugin.InvokeAsync(
            "WorkOrder", "get_open_work_orders");

        // Result
        openWorkOrdersResult.ToString().Should().Be("No open work orders found.");
    }

    [Fact]
    public async Task GetOpenWorkOrders_WhenOpenWorkOrdersExist_ReturnsFormattedWorkOrdersWithVehicleDetails()
    {
        // Setup
        var vehicleWithOpenWorkOrder = CreateTestVehicle("V-2019-0001");
        var twOpenWorkOrders = new List<WorkOrder>
        {
            new()
            {
                Id = 1, WorkOrderNumber = "WO-2026-00025",
                VehicleId = 1, Vehicle = vehicleWithOpenWorkOrder,
                Status = WorkOrderStatus.Open, Priority = Priority.Critical,
                Description = "Engine overheating under load",
                RequestedDate = new DateTime(2026, 4, 1),
                AssignedTechnician = "Mike Torres"
            },
            new()
            {
                Id = 2, WorkOrderNumber = "WO-2026-00030",
                VehicleId = 1, Vehicle = vehicleWithOpenWorkOrder,
                Status = WorkOrderStatus.InProgress, Priority = Priority.Medium,
                Description = "Replace cabin air filter",
                RequestedDate = new DateTime(2026, 4, 5),
                AssignedTechnician = "Sarah Chen"
            }
        };

        _mockWorkOrderRepository
            .Setup(r => r.GetOpenWorkOrdersAsync())
            .ReturnsAsync(twOpenWorkOrders);

        var kernelWithWorkOrderPlugin = CreateKernelWithWorkOrderPlugin();

        // Act
        var openWorkOrdersResult = await kernelWithWorkOrderPlugin.InvokeAsync(
            "WorkOrder", "get_open_work_orders");

        // Result
        var openWorkOrdersJsonResponse = openWorkOrdersResult.ToString();
        openWorkOrdersJsonResponse.Should().Contain("Found 2 open work orders");
        openWorkOrdersJsonResponse.Should().Contain("WO-2026-00025");
        openWorkOrdersJsonResponse.Should().Contain("V-2019-0001");
        openWorkOrdersJsonResponse.Should().Contain("2019 Ford F-150 XL");
        openWorkOrdersJsonResponse.Should().Contain("Critical");
        openWorkOrdersJsonResponse.Should().Contain("Mike Torres");
    }

    // ── get_work_order_details ───────────────────────────────────────

    [Fact]
    public async Task GetWorkOrderDetails_WhenWorkOrderExists_ReturnsFormattedDetailsWithVehicleInfo()
    {
        // Setup
        var vehicleAssociatedWithWorkOrder = CreateTestVehicle("V-2019-0001");
        var completedBrakeRepairWorkOrder = new WorkOrder
        {
            Id = 5, WorkOrderNumber = "WO-2026-00142",
            VehicleId = 1, Vehicle = vehicleAssociatedWithWorkOrder,
            Status = WorkOrderStatus.Completed, Priority = Priority.High,
            Description = "Front brake pad replacement",
            RequestedDate = new DateTime(2026, 3, 1),
            CompletedDate = new DateTime(2026, 3, 5),
            AssignedTechnician = "James Wilson",
            LaborHours = 2.5m, TotalCost = 385.00m,
            Notes = "Rotors within spec, pads replaced both sides"
        };

        _mockWorkOrderRepository
            .Setup(r => r.GetByWorkOrderNumberAsync("WO-2026-00142"))
            .ReturnsAsync(completedBrakeRepairWorkOrder);

        var kernelWithWorkOrderPlugin = CreateKernelWithWorkOrderPlugin();

        // Act
        var workOrderDetailsResult = await kernelWithWorkOrderPlugin.InvokeAsync(
            "WorkOrder", "get_work_order_details",
            new KernelArguments { ["workOrderNumber"] = "WO-2026-00142" });

        // Result
        var workOrderDetailsJsonResponse = workOrderDetailsResult.ToString();
        workOrderDetailsJsonResponse.Should().Contain("Work order WO-2026-00142: Front brake pad replacement");
        workOrderDetailsJsonResponse.Should().Contain("V-2019-0001");
        workOrderDetailsJsonResponse.Should().Contain("385");
        workOrderDetailsJsonResponse.Should().Contain("James Wilson");
        workOrderDetailsJsonResponse.Should().Contain("Rotors within spec");
    }

    [Fact]
    public async Task GetWorkOrderDetails_WhenWorkOrderDoesNotExist_ReturnsNotFoundMessage()
    {
        // Setup
        _mockWorkOrderRepository
            .Setup(r => r.GetByWorkOrderNumberAsync("WO-9999-99999"))
            .ReturnsAsync((WorkOrder?)null);

        var kernelWithWorkOrderPlugin = CreateKernelWithWorkOrderPlugin();

        // Act
        var workOrderDetailsResult = await kernelWithWorkOrderPlugin.InvokeAsync(
            "WorkOrder", "get_work_order_details",
            new KernelArguments { ["workOrderNumber"] = "WO-9999-99999" });

        // Result
        workOrderDetailsResult.ToString().Should().Be("No work order found with number WO-9999-99999.");
    }

    // ── get_parts_below_reorder_threshold ────────────────────────────

    [Fact]
    public async Task GetPartsBelowReorderThreshold_WhenAllPartsAboveThreshold_ReturnsNoRestockingMessage()
    {
        // Setup
        _mockPartRepository
            .Setup(r => r.GetBelowReorderThresholdAsync())
            .ReturnsAsync(new List<Part>());

        var kernelWithWorkOrderPlugin = CreateKernelWithWorkOrderPlugin();

        // Act
        var partsResult = await kernelWithWorkOrderPlugin.InvokeAsync(
            "WorkOrder", "get_parts_below_reorder_threshold");

        // Result
        partsResult.ToString().Should().Be("All parts are above their reorder thresholds. No restocking needed.");
    }

    [Fact]
    public async Task GetPartsBelowReorderThreshold_WhenPartsBelowThreshold_ReturnsFormattedPartsWithDeficit()
    {
        // Setup
        var twoPartsBelowReorderThreshold = new List<Part>
        {
            new()
            {
                Id = 1, PartNumber = "BRK-PAD-001", Name = "Brake Pads (Front)",
                Category = "Brakes", QuantityInStock = 3, ReorderThreshold = 10,
                UnitCost = 45.99m, Location = "Bin A-12"
            },
            new()
            {
                Id = 2, PartNumber = "FLT-OIL-001", Name = "Oil Filter",
                Category = "Filters", QuantityInStock = 5, ReorderThreshold = 20,
                UnitCost = 8.99m, Location = "Bin B-03"
            }
        };

        _mockPartRepository
            .Setup(r => r.GetBelowReorderThresholdAsync())
            .ReturnsAsync(twoPartsBelowReorderThreshold);

        var kernelWithWorkOrderPlugin = CreateKernelWithWorkOrderPlugin();

        // Act
        var partsResult = await kernelWithWorkOrderPlugin.InvokeAsync(
            "WorkOrder", "get_parts_below_reorder_threshold");

        // Result
        var partsBelowThresholdJsonResponse = partsResult.ToString();
        partsBelowThresholdJsonResponse.Should().Contain("Found 2 parts below reorder threshold");
        partsBelowThresholdJsonResponse.Should().Contain("BRK-PAD-001");
        partsBelowThresholdJsonResponse.Should().Contain("Brake Pads (Front)");
        partsBelowThresholdJsonResponse.Should().Contain("Bin A-12");
    }

    [Fact]
    public async Task GetPartsBelowReorderThreshold_WhenPartsBelowThreshold_CalculatesDeficitCorrectly()
    {
        // Setup
        var partWithKnownDeficit = new List<Part>
        {
            new()
            {
                Id = 1, PartNumber = "BRK-PAD-001", Name = "Brake Pads (Front)",
                Category = "Brakes", QuantityInStock = 2, ReorderThreshold = 15,
                UnitCost = 45.99m, Location = "Bin A-12"
            }
        };

        _mockPartRepository
            .Setup(r => r.GetBelowReorderThresholdAsync())
            .ReturnsAsync(partWithKnownDeficit);

        var kernelWithWorkOrderPlugin = CreateKernelWithWorkOrderPlugin();

        // Act
        var partsResult = await kernelWithWorkOrderPlugin.InvokeAsync(
            "WorkOrder", "get_parts_below_reorder_threshold");

        // Result -- Deficit should be ReorderThreshold (15) - QuantityInStock (2) = 13
        partsResult.ToString().Should().Contain("\"Deficit\": 13");
    }
}
