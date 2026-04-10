using FleetWise.Domain.Entities;
using FleetWise.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.Infrastructure.Repositories;

public class MaintenanceRepository(FleetDbContext context) : IMaintenanceRepository
{
    public async Task<List<MaintenanceRecord>> GetByVehicleIdAsync(int vehicleId)
    {
        return await context.MaintenanceRecords
            .Where(mr => mr.VehicleId == vehicleId)
            .OrderByDescending(mr => mr.PerformedDate)
            .ToListAsync();
    }

    public async Task<List<MaintenanceSchedule>> GetOverdueSchedulesAsync()
    {
        var now = DateTime.UtcNow;

        var schedules = await context.MaintenanceSchedules
            .Include(ms => ms.Vehicle)
            .Where(ms =>
                (ms.NextDueDate.HasValue && ms.NextDueDate.Value < now) ||
                (ms.NextDueMileage.HasValue && ms.Vehicle.CurrentMileage >= ms.NextDueMileage.Value))
            .OrderBy(ms => ms.NextDueDate)
            .ToListAsync();

        return schedules;
    }

    public async Task<List<MaintenanceSchedule>> GetUpcomingSchedulesAsync(int withinDays = 30, int withinMiles = 5000)
    {
        var now = DateTime.UtcNow;
        var cutoffDate = now.AddDays(withinDays);

        var schedules = await context.MaintenanceSchedules
            .Include(ms => ms.Vehicle)
            .Where(ms =>
                (ms.NextDueDate.HasValue && ms.NextDueDate.Value >= now && ms.NextDueDate.Value <= cutoffDate) ||
                (ms.NextDueMileage.HasValue && ms.Vehicle.CurrentMileage >= ms.NextDueMileage.Value - withinMiles && ms.Vehicle.CurrentMileage < ms.NextDueMileage.Value))
            .OrderBy(ms => ms.NextDueDate)
            .ToListAsync();

        return schedules;
    }

    public async Task<List<MaintenanceCostGroup>> GetCostSummaryAsync(string groupBy = "vehicle")
    {
        var query = context.MaintenanceRecords.Include(mr => mr.Vehicle).AsQueryable();

        return groupBy.ToLower() switch
        {
            "vehicle" => await query
                .GroupBy(mr => mr.Vehicle.AssetNumber)
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(mr => mr.Cost), g.Count()))
                .OrderByDescending(x => x.TotalCost)
                .ToListAsync(),

            "type" => await query
                .GroupBy(mr => mr.MaintenanceType.ToString())
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(mr => mr.Cost), g.Count()))
                .OrderByDescending(x => x.TotalCost)
                .ToListAsync(),

            "month" => await query
                .GroupBy(mr => mr.PerformedDate.Year + "-" + mr.PerformedDate.Month.ToString().PadLeft(2, '0'))
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(mr => mr.Cost), g.Count()))
                .OrderByDescending(x => x.GroupKey)
                .ToListAsync(),

            _ => await query
                .GroupBy(mr => mr.Vehicle.AssetNumber)
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(mr => mr.Cost), g.Count()))
                .OrderByDescending(x => x.TotalCost)
                .ToListAsync()
        };
    }
}
