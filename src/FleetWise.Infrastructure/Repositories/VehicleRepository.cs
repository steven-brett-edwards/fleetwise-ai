using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.Infrastructure.Repositories;

public class VehicleRepository(FleetDbContext context) : IVehicleRepository
{
    public async Task<List<Vehicle>> GetAllAsync(VehicleStatus? status = null, string? department = null, FuelType? fuelType = null)
    {
        var query = context.Vehicles.AsQueryable();

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(department))
            query = query.Where(v => v.Department == department);
        if (fuelType.HasValue)
            query = query.Where(v => v.FuelType == fuelType.Value);

        return await query.OrderBy(v => v.AssetNumber).ToListAsync();
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        return await context.Vehicles
            .Include(v => v.MaintenanceSchedules)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Vehicle?> GetByAssetNumberAsync(string assetNumber)
    {
        return await context.Vehicles
            .Include(v => v.MaintenanceSchedules)
            .FirstOrDefaultAsync(v => v.AssetNumber == assetNumber);
    }

    public async Task<List<Vehicle>> SearchAsync(
        string? make = null, string? model = null, string? department = null,
        VehicleStatus? status = null, FuelType? fuelType = null)
    {
        var query = context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(make))
            query = query.Where(v => v.Make.ToLower().Contains(make.ToLower()));
        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(v => v.Model.ToLower().Contains(model.ToLower()));
        if (!string.IsNullOrWhiteSpace(department))
            query = query.Where(v => v.Department.ToLower().Contains(department.ToLower()));
        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);
        if (fuelType.HasValue)
            query = query.Where(v => v.FuelType == fuelType.Value);

        return await query.OrderBy(v => v.AssetNumber).ToListAsync();
    }

    public async Task<FleetSummary> GetFleetSummaryAsync()
    {
        var vehicles = await context.Vehicles.ToListAsync();

        var byStatus = vehicles
            .GroupBy(v => v.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var byFuelType = vehicles
            .GroupBy(v => v.FuelType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var byDepartment = vehicles
            .GroupBy(v => v.Department)
            .ToDictionary(g => g.Key, g => g.Count());

        return new FleetSummary(vehicles.Count, byStatus, byFuelType, byDepartment);
    }

    public async Task<List<VehicleMaintenanceCost>> GetVehiclesByMaintenanceCostAsync(int topN = 10)
    {
        // SQLite's EF provider can't translate GroupBy + Sum(decimal) + OrderByDescending,
        // so materialize the minimal projection first and aggregate in memory. The record
        // count is small (hundreds, not millions) so client-side grouping is fine here.
        var rows = await context.MaintenanceRecords
            .Select(mr => new { mr.VehicleId, mr.Cost })
            .ToListAsync();

        var topCosts = rows
            .GroupBy(r => r.VehicleId)
            .Select(g => new
            {
                VehicleId = g.Key,
                TotalCost = g.Sum(r => r.Cost),
                RecordCount = g.Count()
            })
            .OrderByDescending(x => x.TotalCost)
            .Take(topN)
            .ToList();

        if (topCosts.Count == 0)
            return new List<VehicleMaintenanceCost>();

        var vehicleIds = topCosts.Select(x => x.VehicleId).ToList();
        var vehicles = await context.Vehicles
            .Where(v => vehicleIds.Contains(v.Id))
            .ToDictionaryAsync(v => v.Id);

        return topCosts
            .Where(x => vehicles.ContainsKey(x.VehicleId))
            .Select(x =>
            {
                var v = vehicles[x.VehicleId];
                return new VehicleMaintenanceCost(
                    v.Id,
                    v.AssetNumber,
                    v.Year,
                    v.Make,
                    v.Model,
                    x.TotalCost,
                    x.RecordCount);
            })
            .ToList();
    }
}
