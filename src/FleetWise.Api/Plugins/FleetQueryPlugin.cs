using System.ComponentModel;
using System.Text.Json;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using Microsoft.SemanticKernel;

namespace FleetWise.Api.Plugins;

/// <summary>
/// Semantic Kernel plugin that exposes fleet vehicle queries as kernel functions.
/// The LLM reads the [Description] attributes to decide which function to call
/// based on the user's natural language question.
/// </summary>
public class FleetQueryPlugin(IVehicleRepository vehicleRepo)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [KernelFunction("get_fleet_summary")]
    [Description("Returns a summary of the fleet including total vehicle count and breakdowns by status, fuel type, and department. Use this when the user asks about fleet size, composition, or general fleet statistics.")]
    public async Task<string> GetFleetSummary()
    {
        var summary = await vehicleRepo.GetFleetSummaryAsync();
        var json = JsonSerializer.Serialize(summary, JsonOptions);
        return $"Fleet summary: {summary.TotalVehicles} total vehicles\n{json}";
    }

    [KernelFunction("get_vehicle_by_asset_number")]
    [Description("Looks up a specific vehicle by its asset number. Asset numbers follow the format V-YYYY-NNNN (e.g., V-2019-0042). Use this when the user references a specific vehicle.")]
    public async Task<string> GetVehicleByAssetNumber(
        [Description("The asset number to look up (format: V-YYYY-NNNN)")] string assetNumber)
    {
        var vehicle = await vehicleRepo.GetByAssetNumberAsync(assetNumber);
        if (vehicle is null)
            return $"No vehicle found with asset number {assetNumber}.";

        var projection = new
        {
            vehicle.Id,
            vehicle.AssetNumber,
            vehicle.VIN,
            vehicle.Year,
            vehicle.Make,
            vehicle.Model,
            FuelType = vehicle.FuelType.ToString(),
            Status = vehicle.Status.ToString(),
            vehicle.Department,
            vehicle.AssignedDriver,
            vehicle.CurrentMileage,
            vehicle.AcquisitionDate,
            vehicle.AcquisitionCost,
            vehicle.LicensePlate,
            vehicle.Location,
            vehicle.Notes
        };

        var json = JsonSerializer.Serialize(projection, JsonOptions);
        return $"Vehicle {vehicle.AssetNumber}: {vehicle.Year} {vehicle.Make} {vehicle.Model}\n{json}";
    }

    [KernelFunction("search_vehicles")]
    [Description("Search vehicles by make, model, department, status (Active, InShop, OutOfService, Retired), or fuel type (Gasoline, Diesel, Electric, Hybrid, CNG). All filters are optional -- combine them to narrow results.")]
    public async Task<string> SearchVehicles(
        [Description("Filter by make (e.g., Ford, Chevrolet)")] string? make = null,
        [Description("Filter by model (e.g., F-150, Silverado)")] string? model = null,
        [Description("Filter by department (e.g., Public Works, Parks and Recreation)")] string? department = null,
        [Description("Filter by status: Active, InShop, OutOfService, Retired")] string? status = null,
        [Description("Filter by fuel type: Gasoline, Diesel, Electric, Hybrid, CNG")] string? fuelType = null)
    {
        VehicleStatus? parsedStatus = null;
        if (status is not null)
        {
            if (!Enum.TryParse<VehicleStatus>(status, ignoreCase: true, out var s))
                return $"Invalid status '{status}'. Valid values: Active, InShop, OutOfService, Retired";
            parsedStatus = s;
        }

        FuelType? parsedFuelType = null;
        if (fuelType is not null)
        {
            if (!Enum.TryParse<FuelType>(fuelType, ignoreCase: true, out var f))
                return $"Invalid fuel type '{fuelType}'. Valid values: Gasoline, Diesel, Electric, Hybrid, CNG";
            parsedFuelType = f;
        }

        var vehicles = await vehicleRepo.SearchAsync(make, model, department, parsedStatus, parsedFuelType);

        if (vehicles.Count == 0)
            return "No vehicles found matching the specified criteria.";

        var projections = vehicles.Select(v => new
        {
            v.AssetNumber,
            v.Year,
            v.Make,
            v.Model,
            FuelType = v.FuelType.ToString(),
            Status = v.Status.ToString(),
            v.Department,
            v.CurrentMileage
        });

        var json = JsonSerializer.Serialize(projections, JsonOptions);
        return $"Found {vehicles.Count} vehicles matching criteria\n{json}";
    }

    [KernelFunction("get_vehicles_by_high_maintenance_cost")]
    [Description("Returns the vehicles with the highest total maintenance costs, ranked from most to least expensive. Use this when the user asks about costly vehicles, maintenance spending, or which vehicles cost the most to maintain.")]
    public async Task<string> GetVehiclesByHighMaintenanceCost(
        [Description("Number of vehicles to return (default: 10)")] int topN = 10)
    {
        var vehicles = await vehicleRepo.GetVehiclesByMaintenanceCostAsync(topN);

        if (vehicles.Count == 0)
            return "No maintenance cost data available.";

        var json = JsonSerializer.Serialize(vehicles, JsonOptions);
        return $"Top {vehicles.Count} vehicles by maintenance cost\n{json}";
    }
}
