using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FleetWise.Functions;

public class FleetDigestFunction(
    IMaintenanceRepository maintenanceRepo,
    IWorkOrderRepository workOrderRepo,
    ILogger<FleetDigestFunction> logger)
{
    private readonly IMaintenanceRepository _maintenanceRepo = maintenanceRepo;
    private readonly IWorkOrderRepository _workOrderRepo = workOrderRepo;
    private readonly ILogger<FleetDigestFunction> _logger = logger;

    [Function("FleetDailyDigest")]
    public async Task Run([TimerTrigger("0 0 7 * * *")] TimerInfo timerInfo)
    {
        var overdueSchedules = await _maintenanceRepo.GetOverdueSchedulesAsync();
        var openWorkOrders = await _workOrderRepo.GetOpenWorkOrdersAsync();
        var criticalWorkOrders = openWorkOrders
            .Where(wo => wo.Priority == Priority.Critical)
            .ToList();

        _logger.LogInformation(
            "[FleetDigest] {Date} -- {OverdueCount} overdue vehicle schedule(s), {CriticalCount} critical work order(s)",
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            overdueSchedules.Count,
            criticalWorkOrders.Count);

        foreach (var schedule in overdueSchedules)
        {
            _logger.LogWarning(
                "[FleetDigest] OVERDUE: {AssetNumber} {Year} {Make} {Model} ({Department}) -- {MaintenanceType} past due",
                schedule.Vehicle.AssetNumber,
                schedule.Vehicle.Year,
                schedule.Vehicle.Make,
                schedule.Vehicle.Model,
                schedule.Vehicle.Department,
                schedule.MaintenanceType);
        }

        foreach (var wo in criticalWorkOrders)
        {
            _logger.LogWarning(
                "[FleetDigest] CRITICAL WO: {WorkOrderNumber} -- {Description} ({AssetNumber})",
                wo.WorkOrderNumber,
                wo.Description,
                wo.Vehicle.AssetNumber);
        }
    }
}
