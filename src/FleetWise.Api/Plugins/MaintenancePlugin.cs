using System.ComponentModel;
using System.Text.Json;
using FleetWise.Infrastructure.Repositories;
using Microsoft.SemanticKernel;

namespace FleetWise.Api.Plugins;

/// <summary>
/// Semantic Kernel plugin for maintenance schedules, history, and cost analysis.
/// </summary>
public class MaintenancePlugin(IMaintenanceRepository maintenanceRepo)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [KernelFunction("get_overdue_maintenance")]
    [Description("Returns all maintenance schedules that are past their due date or due mileage. Use this when the user asks about overdue maintenance, missed services, or vehicles needing immediate attention.")]
    public async Task<string> GetOverdueMaintenance()
    {
        var schedules = await maintenanceRepo.GetOverdueSchedulesAsync();

        if (schedules.Count == 0)
            return "No overdue maintenance schedules found.";

        var projections = schedules.Select(s => new
        {
            s.Vehicle.AssetNumber,
            VehicleDescription = $"{s.Vehicle.Year} {s.Vehicle.Make} {s.Vehicle.Model}",
            MaintenanceType = s.MaintenanceType.ToString(),
            s.NextDueDate,
            s.NextDueMileage,
            s.Vehicle.CurrentMileage,
            s.LastCompletedDate
        });

        var json = JsonSerializer.Serialize(projections, JsonOptions);
        return $"Found {schedules.Count} overdue maintenance schedules\n{json}";
    }

    [KernelFunction("get_upcoming_maintenance")]
    [Description("Returns maintenance schedules coming due soon. Filters by days until due and miles until due. Use this to plan ahead or check what maintenance is needed soon.")]
    public async Task<string> GetUpcomingMaintenance(
        [Description("Number of days to look ahead (default: 30)")] int withinDays = 30,
        [Description("Mileage threshold to look ahead (default: 5000)")] int withinMiles = 5000)
    {
        var schedules = await maintenanceRepo.GetUpcomingSchedulesAsync(withinDays, withinMiles);

        if (schedules.Count == 0)
            return $"No maintenance scheduled within the next {withinDays} days or {withinMiles} miles.";

        var projections = schedules.Select(s => new
        {
            s.Vehicle.AssetNumber,
            VehicleDescription = $"{s.Vehicle.Year} {s.Vehicle.Make} {s.Vehicle.Model}",
            MaintenanceType = s.MaintenanceType.ToString(),
            s.NextDueDate,
            s.NextDueMileage,
            s.Vehicle.CurrentMileage
        });

        var json = JsonSerializer.Serialize(projections, JsonOptions);
        return $"Found {schedules.Count} upcoming maintenance schedules\n{json}";
    }

    [KernelFunction("get_vehicle_maintenance_history")]
    [Description("Returns the maintenance history for a specific vehicle by its database ID. Use get_vehicle_by_asset_number first to get the vehicle ID if you only have an asset number.")]
    public async Task<string> GetVehicleMaintenanceHistory(
        [Description("The vehicle's database ID (integer)")] int vehicleId)
    {
        var records = await maintenanceRepo.GetByVehicleIdAsync(vehicleId);

        if (records.Count == 0)
            return $"No maintenance records found for vehicle ID {vehicleId}.";

        var projections = records.Select(r => new
        {
            MaintenanceType = r.MaintenanceType.ToString(),
            r.PerformedDate,
            r.MileageAtService,
            r.Description,
            r.Cost,
            r.TechnicianName
        });

        var json = JsonSerializer.Serialize(projections, JsonOptions);
        return $"Found {records.Count} maintenance records for vehicle ID {vehicleId}\n{json}";
    }

    [KernelFunction("get_maintenance_cost_summary")]
    [Description("Returns a summary of maintenance costs grouped by a specified category. Valid groupBy values: 'vehicle' (costs per vehicle), 'type' (costs per maintenance type), 'month' (costs per month).")]
    public async Task<string> GetMaintenanceCostSummary(
        [Description("How to group costs: 'vehicle', 'type', or 'month'")] string groupBy = "vehicle")
    {
        var validValues = new[] { "vehicle", "type", "month" };
        if (!validValues.Contains(groupBy, StringComparer.OrdinalIgnoreCase))
            return $"Invalid groupBy value '{groupBy}'. Valid values: vehicle, type, month";

        var costGroups = await maintenanceRepo.GetCostSummaryAsync(groupBy);

        if (costGroups.Count == 0)
            return "No maintenance cost data available.";

        var json = JsonSerializer.Serialize(costGroups, JsonOptions);
        return $"Maintenance costs grouped by {groupBy} ({costGroups.Count} groups)\n{json}";
    }
}
