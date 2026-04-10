using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController(IVehicleRepository vehicleRepo, IWorkOrderRepository workOrderRepo, IMaintenanceRepository maintenanceRepo) : ControllerBase
{
    /// <summary>List all vehicles with optional filtering.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] VehicleStatus? status = null,
        [FromQuery] string? department = null,
        [FromQuery] FuelType? fuelType = null)
    {
        var vehicles = await vehicleRepo.GetAllAsync(status, department, fuelType);
        return Ok(vehicles);
    }

    /// <summary>Get a single vehicle by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vehicle = await vehicleRepo.GetByIdAsync(id);
        if (vehicle is null) return NotFound();
        return Ok(vehicle);
    }

    /// <summary>Get maintenance history for a specific vehicle.</summary>
    [HttpGet("{id:int}/maintenance")]
    public async Task<IActionResult> GetMaintenanceHistory(int id)
    {
        var vehicle = await vehicleRepo.GetByIdAsync(id);
        if (vehicle is null) return NotFound();

        var records = await maintenanceRepo.GetByVehicleIdAsync(id);
        return Ok(records);
    }

    /// <summary>Get work orders for a specific vehicle.</summary>
    [HttpGet("{id:int}/work-orders")]
    public async Task<IActionResult> GetWorkOrders(int id)
    {
        var vehicle = await vehicleRepo.GetByIdAsync(id);
        if (vehicle is null) return NotFound();

        var workOrders = await workOrderRepo.GetByVehicleIdAsync(id);
        return Ok(workOrders);
    }

    /// <summary>Get fleet summary statistics.</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await vehicleRepo.GetFleetSummaryAsync();
        return Ok(summary);
    }
}
