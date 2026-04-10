using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;

namespace FleetWise.Infrastructure.Repositories;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllAsync(VehicleStatus? status = null, string? department = null, FuelType? fuelType = null);
    Task<Vehicle?> GetByIdAsync(int id);
    Task<Vehicle?> GetByAssetNumberAsync(string assetNumber);
    Task<List<Vehicle>> SearchAsync(string? make = null, string? model = null, string? department = null, VehicleStatus? status = null, FuelType? fuelType = null);
    Task<FleetSummary> GetFleetSummaryAsync();
    Task<List<VehicleMaintenanceCost>> GetVehiclesByMaintenanceCostAsync(int topN = 10);
}

public record FleetSummary(
    int TotalVehicles,
    Dictionary<string, int> ByStatus,
    Dictionary<string, int> ByFuelType,
    Dictionary<string, int> ByDepartment);

public record VehicleMaintenanceCost(
    int VehicleId,
    string AssetNumber,
    int Year,
    string Make,
    string Model,
    decimal TotalMaintenanceCost,
    int RecordCount);
