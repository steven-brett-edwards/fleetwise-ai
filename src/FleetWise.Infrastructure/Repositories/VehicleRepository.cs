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
        return await context.MaintenanceRecords
            .GroupBy(mr => mr.VehicleId)
            .Select(g => new
            {
                VehicleId = g.Key,
                TotalCost = g.Sum(mr => mr.Cost),
                RecordCount = g.Count()
            })
            .OrderByDescending(x => x.TotalCost)
            .Take(topN)
            .Join(context.Vehicles,
                cost => cost.VehicleId,
                vehicle => vehicle.Id,
                (cost, vehicle) => new VehicleMaintenanceCost(
                    vehicle.Id,
                    vehicle.AssetNumber,
                    vehicle.Year,
                    vehicle.Make,
                    vehicle.Model,
                    cost.TotalCost,
                    cost.RecordCount))
            .ToListAsync();
    }
}
