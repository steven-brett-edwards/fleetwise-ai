using FleetWise.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController(IMaintenanceRepository maintenanceRepo) : ControllerBase
{
    /// <summary>Get all overdue preventive maintenance items.</summary>
    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        var overdue = await maintenanceRepo.GetOverdueSchedulesAsync();
        var result = overdue.Select(ms => new
        {
            ms.Id,
            ms.VehicleId,
            VehicleAssetNumber = ms.Vehicle.AssetNumber,
            VehicleDescription = $"{ms.Vehicle.Year} {ms.Vehicle.Make} {ms.Vehicle.Model}",
            MaintenanceType = ms.MaintenanceType.ToString(),
            ms.NextDueDate,
            ms.NextDueMileage,
            CurrentMileage = ms.Vehicle.CurrentMileage,
            ms.LastCompletedDate,
            ms.LastCompletedMileage
        });
        return Ok(result);
    }

    /// <summary>Get upcoming preventive maintenance items within specified thresholds.</summary>
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming(
        [FromQuery] int days = 30,
        [FromQuery] int miles = 5000)
    {
        var upcoming = await maintenanceRepo.GetUpcomingSchedulesAsync(days, miles);
        var result = upcoming.Select(ms => new
        {
            ms.Id,
            ms.VehicleId,
            VehicleAssetNumber = ms.Vehicle.AssetNumber,
            VehicleDescription = $"{ms.Vehicle.Year} {ms.Vehicle.Make} {ms.Vehicle.Model}",
            MaintenanceType = ms.MaintenanceType.ToString(),
            ms.NextDueDate,
            ms.NextDueMileage,
            CurrentMileage = ms.Vehicle.CurrentMileage
        });
        return Ok(result);
    }
}
