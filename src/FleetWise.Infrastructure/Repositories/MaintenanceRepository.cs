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
        // SQLite's EF provider can't translate GroupBy + Sum(decimal) + OrderByDescending,
        // so we pull the minimal projection into memory and group client-side. Dataset is
        // small (hundreds of records); in-memory aggregation is the pragmatic fix.
        var rows = await context.MaintenanceRecords
            .Select(mr => new
            {
                mr.Cost,
                mr.PerformedDate,
                MaintenanceType = mr.MaintenanceType.ToString(),
                mr.Vehicle.AssetNumber
            })
            .ToListAsync();

        return groupBy.ToLower() switch
        {
            "vehicle" => [.. rows
                .GroupBy(r => r.AssetNumber)
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(r => r.Cost), g.Count()))
                .OrderByDescending(x => x.TotalCost)],

            "type" => [.. rows
                .GroupBy(r => r.MaintenanceType)
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(r => r.Cost), g.Count()))
                .OrderByDescending(x => x.TotalCost)],

            "month" => [.. rows
                .GroupBy(r => r.PerformedDate.Year + "-" + r.PerformedDate.Month.ToString().PadLeft(2, '0'))
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(r => r.Cost), g.Count()))
                .OrderByDescending(x => x.GroupKey)],

            _ => [.. rows
                .GroupBy(r => r.AssetNumber)
                .Select(g => new MaintenanceCostGroup(g.Key, g.Sum(r => r.Cost), g.Count()))
                .OrderByDescending(x => x.TotalCost)]
        };
    }
}
