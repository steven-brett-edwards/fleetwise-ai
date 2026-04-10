using FleetWise.Domain.Entities;

namespace FleetWise.Infrastructure.Repositories;

public interface IMaintenanceRepository
{
    Task<List<MaintenanceRecord>> GetByVehicleIdAsync(int vehicleId);
    Task<List<MaintenanceSchedule>> GetOverdueSchedulesAsync();
    Task<List<MaintenanceSchedule>> GetUpcomingSchedulesAsync(int withinDays = 30, int withinMiles = 5000);
    Task<List<MaintenanceCostGroup>> GetCostSummaryAsync(string groupBy = "vehicle");
}

public record MaintenanceCostGroup(string GroupKey, decimal TotalCost, int RecordCount);
