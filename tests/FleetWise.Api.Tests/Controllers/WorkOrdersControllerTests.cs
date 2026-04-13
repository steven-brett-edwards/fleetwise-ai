using FleetWise.Api.Controllers;
using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FleetWise.Api.Tests.Controllers;

/// <summary>
/// Tests for WorkOrdersController constructed with a mocked IWorkOrderRepository --
/// the same way DI builds it in production. This is a thin CRUD controller with
/// two endpoints: list (with optional status filter) and get by ID.
/// </summary>
public class WorkOrdersControllerTests
{
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository = new();

    private WorkOrdersController CreateWorkOrdersControllerWithMockedRepository()
    {
        return new WorkOrdersController(_mockWorkOrderRepository.Object);
    }

    private static WorkOrder CreateTestWorkOrder(int id = 1, WorkOrderStatus status = WorkOrderStatus.Open) => new()
    {
        Id = id,
        WorkOrderNumber = $"WO-2026-{id:D5}",
        VehicleId = 1,
        Vehicle = new Vehicle
        {
            Id = 1, AssetNumber = "V-2019-0001", VIN = "1FTFW1E50KFA00001",
            Year = 2019, Make = "Ford", Model = "F-150 XL",
            FuelType = FuelType.Gasoline, Status = VehicleStatus.Active,
            Department = "Public Works", CurrentMileage = 87432,
            AcquisitionDate = new DateTime(2019, 3, 15), AcquisitionCost = 35000m,
            LicensePlate = "GOV-0001", Location = "Main Garage"
        },
        Status = status,
        Priority = Priority.High,
        Description = "Brake inspection",
        RequestedDate = new DateTime(2026, 4, 1),
        AssignedTechnician = "Mike Torres"
    };

    // ── GetAll ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_WhenNoStatusFilter_ReturnsOkWithAllWorkOrders()
    {
        // Setup
        var twoWorkOrders = new List<WorkOrder>
        {
            CreateTestWorkOrder(1, WorkOrderStatus.Open),
            CreateTestWorkOrder(2, WorkOrderStatus.InProgress)
        };

        _mockWorkOrderRepository
            .Setup(r => r.GetAllAsync(null))
            .ReturnsAsync(twoWorkOrders);

        var workOrdersControllerWithMockedRepository = CreateWorkOrdersControllerWithMockedRepository();

        // Act
        var actionResult = await workOrdersControllerWithMockedRepository.GetAll();

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedWorkOrders = Assert.IsType<List<WorkOrder>>(okResult.Value);
        returnedWorkOrders.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_WhenStatusFilterProvided_PassesFilterToRepository()
    {
        // Setup
        _mockWorkOrderRepository
            .Setup(r => r.GetAllAsync(WorkOrderStatus.Open))
            .ReturnsAsync(new List<WorkOrder>());

        var workOrdersControllerWithMockedRepository = CreateWorkOrdersControllerWithMockedRepository();

        // Act
        await workOrdersControllerWithMockedRepository.GetAll(status: WorkOrderStatus.Open);

        // Result
        _mockWorkOrderRepository.Verify(
            r => r.GetAllAsync(WorkOrderStatus.Open), Times.Once);
    }

    // ── GetById ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_WhenWorkOrderExists_ReturnsOkWithWorkOrder()
    {
        // Setup
        var existingWorkOrder = CreateTestWorkOrder(1);
        _mockWorkOrderRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingWorkOrder);

        var workOrdersControllerWithMockedRepository = CreateWorkOrdersControllerWithMockedRepository();

        // Act
        var actionResult = await workOrdersControllerWithMockedRepository.GetById(1);

        // Result
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var returnedWorkOrder = Assert.IsType<WorkOrder>(okResult.Value);
        returnedWorkOrder.WorkOrderNumber.Should().Be("WO-2026-00001");
    }

    [Fact]
    public async Task GetById_WhenWorkOrderDoesNotExist_ReturnsNotFound()
    {
        // Setup
        _mockWorkOrderRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((WorkOrder?)null);

        var workOrdersControllerWithMockedRepository = CreateWorkOrdersControllerWithMockedRepository();

        // Act
        var actionResult = await workOrdersControllerWithMockedRepository.GetById(999);

        // Result
        Assert.IsType<NotFoundResult>(actionResult);
    }
}
