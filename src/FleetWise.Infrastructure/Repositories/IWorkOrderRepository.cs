using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;

namespace FleetWise.Infrastructure.Repositories;

public interface IWorkOrderRepository
{
    Task<List<WorkOrder>> GetAllAsync(WorkOrderStatus? status = null);
    Task<WorkOrder?> GetByIdAsync(int id);
    Task<WorkOrder?> GetByWorkOrderNumberAsync(string workOrderNumber);
    Task<List<WorkOrder>> GetByVehicleIdAsync(int vehicleId);
    Task<List<WorkOrder>> GetOpenWorkOrdersAsync();
}
