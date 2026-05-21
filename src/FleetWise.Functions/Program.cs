using FleetWise.Infrastructure.Data;
using FleetWise.Infrastructure.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<FleetDbContext>(options =>
            options.UseSqlite(
                context.Configuration["ConnectionStrings:DefaultConnection"]
                ?? "Data Source=fleetwise.db"));

        services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
    })
    .Build();

await host.RunAsync();
