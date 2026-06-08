using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;

namespace FleetWise.Infrastructure.Tests;

public class WorkOrderRepositoryTests : SqliteRepositoryTestBase
{
    private readonly WorkOrderRepository _repository;

    public WorkOrderRepositoryTests()
    {
        Context.Vehicles.AddRange(
            NewVehicle(1, "V-001"),
            NewVehicle(2, "V-002")
        );
        Context.WorkOrders.AddRange(
            NewWorkOrder(1, vehicleId: 1, "WO-0001", WorkOrderStatus.Open,        Priority.Medium,   new DateTime(2026, 1,  1)),
            NewWorkOrder(2, vehicleId: 1, "WO-0002", WorkOrderStatus.InProgress,  Priority.High,     new DateTime(2026, 1, 15)),
            NewWorkOrder(3, vehicleId: 2, "WO-0003", WorkOrderStatus.Completed,   Priority.Low,      new DateTime(2025, 12, 1)),
            NewWorkOrder(4, vehicleId: 2, "WO-0004", WorkOrderStatus.Cancelled,   Priority.Low,      new DateTime(2025, 11, 1)),
            NewWorkOrder(5, vehicleId: 1, "WO-0005", WorkOrderStatus.AwaitingParts, Priority.Critical, new DateTime(2026, 2, 1))
        );
        Context.SaveChanges();

        _repository = new WorkOrderRepository(Context);
    }

    // ── GetAllAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WithNoFilter_ReturnsAllWorkOrdersOrderedByDateDescending()
    {
        // Act
        var allWorkOrders = await _repository.GetAllAsync();

        // Result
        allWorkOrders.Should().HaveCount(5);
        allWorkOrders.Select(wo => wo.RequestedDate).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ReturnsOnlyMatchingWorkOrders()
    {
        // Act
        var completedWorkOrders = await _repository.GetAllAsync(WorkOrderStatus.Completed);

        // Result
        completedWorkOrders.Should().HaveCount(1);
        completedWorkOrders[0].WorkOrderNumber.Should().Be("WO-0003");
    }

    [Fact]
    public async Task GetAllAsync_IncludesVehicleNavigationProperty()
    {
        // Act
        var workOrders = await _repository.GetAllAsync();

        // Result -- Vehicle navigation property is loaded for every returned row
        workOrders.Should().AllSatisfy(wo => wo.Vehicle.AssetNumber.Should().NotBeNullOrWhiteSpace());
    }

    // ── GetByIdAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenFound_ReturnsWorkOrderWithVehicle()
    {
        // Act
        var workOrderWithVehicle = await _repository.GetByIdAsync(2);

        // Result
        Assert.NotNull(workOrderWithVehicle);
        workOrderWithVehicle.WorkOrderNumber.Should().Be("WO-0002");
        workOrderWithVehicle.Vehicle.AssetNumber.Should().Be("V-001");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var missingWorkOrder = await _repository.GetByIdAsync(999);

        // Result
        Assert.Null(missingWorkOrder);
    }

    // ── GetByWorkOrderNumberAsync ──────────────────────────────────

    [Fact]
    public async Task GetByWorkOrderNumberAsync_WhenFound_ReturnsMatchingWorkOrder()
    {
        // Act
        var workOrder = await _repository.GetByWorkOrderNumberAsync("WO-0005");

        // Result
        Assert.NotNull(workOrder);
        workOrder.Id.Should().Be(5);
        workOrder.Priority.Should().Be(Priority.Critical);
    }

    [Fact]
    public async Task GetByWorkOrderNumberAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var missingWorkOrder = await _repository.GetByWorkOrderNumberAsync("WO-DOES-NOT-EXIST");

        // Result
        Assert.Null(missingWorkOrder);
    }

    // ── GetByVehicleIdAsync ────────────────────────────────────────

    [Fact]
    public async Task GetByVehicleIdAsync_ReturnsOnlyWorkOrdersForThatVehicle()
    {
        // Act
        var vehicleOneWorkOrders = await _repository.GetByVehicleIdAsync(vehicleId: 1);

        // Result
        vehicleOneWorkOrders.Should().HaveCount(3);
        vehicleOneWorkOrders.Should().AllSatisfy(wo => wo.VehicleId.Should().Be(1));
        vehicleOneWorkOrders.Select(wo => wo.RequestedDate).Should().BeInDescendingOrder();
    }

    // ── GetOpenWorkOrdersAsync ─────────────────────────────────────

    [Fact]
    public async Task GetOpenWorkOrdersAsync_ExcludesCompletedAndCancelledWorkOrders()
    {
        // Setup (seeded: 3 open-ish (Open/InProgress/AwaitingParts), 1 Completed, 1 Cancelled)

        // Act
        var openWorkOrders = await _repository.GetOpenWorkOrdersAsync();

        // Result
        openWorkOrders.Should().HaveCount(3);
        openWorkOrders.Should().NotContain(wo => wo.Status == WorkOrderStatus.Completed);
        openWorkOrders.Should().NotContain(wo => wo.Status == WorkOrderStatus.Cancelled);
    }

    [Fact]
    public async Task GetOpenWorkOrdersAsync_IncludesOpenInProgressAndAwaitingPartsStatuses()
    {
        // Act
        var openWorkOrders = await _repository.GetOpenWorkOrdersAsync();

        // Result
        openWorkOrders.Select(wo => wo.Status).Should().Contain(WorkOrderStatus.Open);
        openWorkOrders.Select(wo => wo.Status).Should().Contain(WorkOrderStatus.InProgress);
        openWorkOrders.Select(wo => wo.Status).Should().Contain(WorkOrderStatus.AwaitingParts);
    }
}
