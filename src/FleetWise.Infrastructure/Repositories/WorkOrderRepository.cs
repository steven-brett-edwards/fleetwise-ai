using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.Infrastructure.Repositories;

public class WorkOrderRepository(FleetDbContext context) : IWorkOrderRepository
{
    public async Task<List<WorkOrder>> GetAllAsync(WorkOrderStatus? status = null)
    {
        var query = context.WorkOrders.Include(wo => wo.Vehicle).AsQueryable();

        if (status.HasValue)
            query = query.Where(wo => wo.Status == status.Value);

        return await query.OrderByDescending(wo => wo.RequestedDate).ToListAsync();
    }

    public async Task<WorkOrder?> GetByIdAsync(int id)
    {
        return await context.WorkOrders
            .Include(wo => wo.Vehicle)
            .FirstOrDefaultAsync(wo => wo.Id == id);
    }

    public async Task<WorkOrder?> GetByWorkOrderNumberAsync(string workOrderNumber)
    {
        return await context.WorkOrders
            .Include(wo => wo.Vehicle)
            .FirstOrDefaultAsync(wo => wo.WorkOrderNumber == workOrderNumber);
    }

    public async Task<List<WorkOrder>> GetByVehicleIdAsync(int vehicleId)
    {
        return await context.WorkOrders
            .Include(wo => wo.Vehicle)
            .Where(wo => wo.VehicleId == vehicleId)
            .OrderByDescending(wo => wo.RequestedDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetOpenWorkOrdersAsync()
    {
        return await context.WorkOrders
            .Include(wo => wo.Vehicle)
            .Where(wo => wo.Status != WorkOrderStatus.Completed && wo.Status != WorkOrderStatus.Cancelled)
            .OrderByDescending(wo => wo.Priority)
            .ThenByDescending(wo => wo.RequestedDate)
            .ToListAsync();
    }
}
