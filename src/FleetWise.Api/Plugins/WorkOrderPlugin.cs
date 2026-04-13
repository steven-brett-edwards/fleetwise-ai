using System.ComponentModel;
using System.Text.Json;
using FleetWise.Infrastructure.Repositories;
using Microsoft.SemanticKernel;

namespace FleetWise.Api.Plugins;

/// <summary>
/// Semantic Kernel plugin for work order queries and parts inventory checks.
/// </summary>
public class WorkOrderPlugin(IWorkOrderRepository workOrderRepo, IPartRepository partRepo)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [KernelFunction("get_open_work_orders")]
    [Description("Returns all open and in-progress work orders with their priority, assigned technician, and associated vehicle. Use this when the user asks about current work, open tickets, or what repairs are in progress.")]
    public async Task<string> GetOpenWorkOrders()
    {
        var workOrders = await workOrderRepo.GetOpenWorkOrdersAsync();

        if (workOrders.Count == 0)
            return "No open work orders found.";

        var projections = workOrders.Select(wo => new
        {
            wo.WorkOrderNumber,
            wo.Vehicle.AssetNumber,
            VehicleDescription = $"{wo.Vehicle.Year} {wo.Vehicle.Make} {wo.Vehicle.Model}",
            Status = wo.Status.ToString(),
            Priority = wo.Priority.ToString(),
            wo.Description,
            wo.RequestedDate,
            wo.AssignedTechnician
        });

        var json = JsonSerializer.Serialize(projections, JsonOptions);
        return $"Found {workOrders.Count} open work orders\n{json}";
    }

    [KernelFunction("get_work_order_details")]
    [Description("Returns full details of a specific work order by its work order number. Work order numbers follow the format WO-YYYY-NNNNN (e.g., WO-2026-00142).")]
    public async Task<string> GetWorkOrderDetails(
        [Description("The work order number to look up (format: WO-YYYY-NNNNN)")] string workOrderNumber)
    {
        var workOrder = await workOrderRepo.GetByWorkOrderNumberAsync(workOrderNumber);

        if (workOrder is null)
            return $"No work order found with number {workOrderNumber}.";

        var projection = new
        {
            workOrder.WorkOrderNumber,
            workOrder.Vehicle.AssetNumber,
            VehicleDescription = $"{workOrder.Vehicle.Year} {workOrder.Vehicle.Make} {workOrder.Vehicle.Model}",
            Status = workOrder.Status.ToString(),
            Priority = workOrder.Priority.ToString(),
            workOrder.Description,
            workOrder.RequestedDate,
            workOrder.CompletedDate,
            workOrder.AssignedTechnician,
            workOrder.LaborHours,
            workOrder.TotalCost,
            workOrder.Notes
        };

        var json = JsonSerializer.Serialize(projection, JsonOptions);
        return $"Work order {workOrder.WorkOrderNumber}: {workOrder.Description}\n{json}";
    }

    [KernelFunction("get_parts_below_reorder_threshold")]
    [Description("Returns parts inventory items that are below their reorder threshold and need to be restocked. Use this when the user asks about low stock, parts inventory, or supply issues.")]
    public async Task<string> GetPartsBelowReorderThreshold()
    {
        var parts = await partRepo.GetBelowReorderThresholdAsync();

        if (parts.Count == 0)
            return "All parts are above their reorder thresholds. No restocking needed.";

        var projections = parts.Select(p => new
        {
            p.PartNumber,
            p.Name,
            p.Category,
            p.QuantityInStock,
            p.ReorderThreshold,
            Deficit = p.ReorderThreshold - p.QuantityInStock,
            p.UnitCost,
            p.Location
        });

        var json = JsonSerializer.Serialize(projections, JsonOptions);
        return $"Found {parts.Count} parts below reorder threshold\n{json}";
    }
}
