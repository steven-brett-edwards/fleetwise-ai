using FleetWise.Domain.Entities;

namespace FleetWise.Infrastructure.Repositories;

public interface IPartRepository
{
    Task<List<Part>> GetAllAsync();
    Task<List<Part>> GetBelowReorderThresholdAsync();
}
