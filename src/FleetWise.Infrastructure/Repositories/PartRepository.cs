using FleetWise.Domain.Entities;
using FleetWise.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.Infrastructure.Repositories;

public class PartRepository(FleetDbContext context) : IPartRepository
{
    public async Task<List<Part>> GetAllAsync()
    {
        return await context.Parts.OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync();
    }

    public async Task<List<Part>> GetBelowReorderThresholdAsync()
    {
        return await context.Parts
            .Where(p => p.QuantityInStock <= p.ReorderThreshold)
            .OrderBy(p => p.QuantityInStock)
            .ToListAsync();
    }
}
