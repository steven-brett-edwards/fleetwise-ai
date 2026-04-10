using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.Infrastructure.Data;

public class FleetDbContext : DbContext
{
    public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<MaintenanceSchedule> MaintenanceSchedules => Set<MaintenanceSchedule>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<MaintenanceRecord> MaintenanceRecords => Set<MaintenanceRecord>();
    public DbSet<Part> Parts => Set<Part>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasIndex(v => v.AssetNumber).IsUnique();
            entity.HasIndex(v => v.VIN).IsUnique();
            entity.Property(v => v.FuelType).HasConversion<string>();
            entity.Property(v => v.Status).HasConversion<string>();
            entity.Property(v => v.AcquisitionCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<MaintenanceSchedule>(entity =>
        {
            entity.HasOne(ms => ms.Vehicle)
                .WithMany(v => v.MaintenanceSchedules)
                .HasForeignKey(ms => ms.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(ms => ms.MaintenanceType).HasConversion<string>();
            entity.Ignore(ms => ms.IsOverdue);
        });

        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.HasIndex(wo => wo.WorkOrderNumber).IsUnique();
            entity.HasOne(wo => wo.Vehicle)
                .WithMany(v => v.WorkOrders)
                .HasForeignKey(wo => wo.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(wo => wo.Status).HasConversion<string>();
            entity.Property(wo => wo.Priority).HasConversion<string>();
            entity.Property(wo => wo.LaborHours).HasPrecision(8, 2);
            entity.Property(wo => wo.TotalCost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasOne(mr => mr.Vehicle)
                .WithMany(v => v.MaintenanceRecords)
                .HasForeignKey(mr => mr.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(mr => mr.WorkOrder)
                .WithMany()
                .HasForeignKey(mr => mr.WorkOrderId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.Property(mr => mr.MaintenanceType).HasConversion<string>();
            entity.Property(mr => mr.Cost).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasIndex(p => p.PartNumber).IsUnique();
            entity.Property(p => p.UnitCost).HasPrecision(18, 2);
        });
    }
}
