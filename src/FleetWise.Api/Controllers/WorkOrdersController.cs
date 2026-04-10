using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.Api.Controllers;

[ApiController]
[Route("api/work-orders")]
public class WorkOrdersController(IWorkOrderRepository workOrderRepo) : ControllerBase
{
    /// <summary>List work orders with optional status filtering.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] WorkOrderStatus? status = null)
    {
        var workOrders = await workOrderRepo.GetAllAsync(status);
        return Ok(workOrders);
    }

    /// <summary>Get a work order by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var workOrder = await workOrderRepo.GetByIdAsync(id);
        if (workOrder is null) return NotFound();
        return Ok(workOrder);
    }
}
