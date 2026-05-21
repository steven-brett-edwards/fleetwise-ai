using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Functions;
using FleetWise.Infrastructure.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;

namespace FleetWise.Functions.Tests;

public class FleetDigestFunctionTests
{
    private readonly Mock<IMaintenanceRepository> _maintenanceRepoMock = new();
    private readonly Mock<IWorkOrderRepository> _workOrderRepoMock = new();
    private readonly Mock<ILogger<FleetDigestFunction>> _loggerMock = new();

    private FleetDigestFunction CreateFunction() =>
        new(_maintenanceRepoMock.Object, _workOrderRepoMock.Object, _loggerMock.Object);

    private static Vehicle MakeVehicle() => new()
    {
        AssetNumber = "V-2024-0001",
        Year = 2024,
        Make = "Ford",
        Model = "F-150",
        Department = "Public Works",
        VIN = "1FTEW1E56KFA00001",
        LicensePlate = "ABC-1234",
        Location = "Yard A"
    };

    private static MaintenanceSchedule MakeOverdueSchedule(Vehicle vehicle) => new()
    {
        Vehicle = vehicle,
        VehicleId = vehicle.Id,
        MaintenanceType = MaintenanceType.OilChange,
        NextDueDate = DateTime.UtcNow.AddDays(-30)
    };

    private static WorkOrder MakeCriticalWorkOrder(Vehicle vehicle) => new()
    {
        WorkOrderNumber = "WO-2026-00001",
        Description = "Hydraulic lift cylinder failure",
        Priority = Priority.Critical,
        Status = WorkOrderStatus.Open,
        RequestedDate = DateTime.UtcNow.AddDays(-3),
        Vehicle = vehicle,
        VehicleId = vehicle.Id
    };

    private static WorkOrder MakeLowPriorityWorkOrder(Vehicle vehicle) => new()
    {
        WorkOrderNumber = "WO-2026-00002",
        Description = "Wiper blade replacement",
        Priority = Priority.Low,
        Status = WorkOrderStatus.Open,
        RequestedDate = DateTime.UtcNow,
        Vehicle = vehicle,
        VehicleId = vehicle.Id
    };

    [Fact]
    public async Task Run_WithOverdueSchedulesAndCriticalWorkOrders_CallsBothReposAndCompletes()
    {
        var vehicle = MakeVehicle();
        _maintenanceRepoMock
            .Setup(r => r.GetOverdueSchedulesAsync())
            .ReturnsAsync([MakeOverdueSchedule(vehicle)]);
        _workOrderRepoMock
            .Setup(r => r.GetOpenWorkOrdersAsync())
            .ReturnsAsync([MakeCriticalWorkOrder(vehicle), MakeLowPriorityWorkOrder(vehicle)]);

        await CreateFunction().Run(new TimerInfo());

        _maintenanceRepoMock.Verify(r => r.GetOverdueSchedulesAsync(), Times.Once);
        _workOrderRepoMock.Verify(r => r.GetOpenWorkOrdersAsync(), Times.Once);
    }

    [Fact]
    public async Task Run_WithEmptyResults_CallsBothReposAndCompletes()
    {
        _maintenanceRepoMock
            .Setup(r => r.GetOverdueSchedulesAsync())
            .ReturnsAsync([]);
        _workOrderRepoMock
            .Setup(r => r.GetOpenWorkOrdersAsync())
            .ReturnsAsync([]);

        await CreateFunction().Run(new TimerInfo());

        _maintenanceRepoMock.Verify(r => r.GetOverdueSchedulesAsync(), Times.Once);
        _workOrderRepoMock.Verify(r => r.GetOpenWorkOrdersAsync(), Times.Once);
    }

    [Fact]
    public async Task Run_WithNonCriticalWorkOrdersOnly_FiltersThemOut()
    {
        var vehicle = MakeVehicle();
        _maintenanceRepoMock
            .Setup(r => r.GetOverdueSchedulesAsync())
            .ReturnsAsync([]);
        _workOrderRepoMock
            .Setup(r => r.GetOpenWorkOrdersAsync())
            .ReturnsAsync([MakeLowPriorityWorkOrder(vehicle)]);

        await CreateFunction().Run(new TimerInfo());

        _maintenanceRepoMock.Verify(r => r.GetOverdueSchedulesAsync(), Times.Once);
        _workOrderRepoMock.Verify(r => r.GetOpenWorkOrdersAsync(), Times.Once);
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}
