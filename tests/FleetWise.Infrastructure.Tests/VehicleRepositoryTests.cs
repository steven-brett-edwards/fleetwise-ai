using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;

namespace FleetWise.Infrastructure.Tests;

public class VehicleRepositoryTests : SqliteRepositoryTestBase
{
    private readonly VehicleRepository _repository;

    public VehicleRepositoryTests()
    {
        Context.Vehicles.AddRange(
            NewVehicle(1, "V-001", VehicleStatus.Active,  "Public Works",       FuelType.Gasoline),
            NewVehicle(2, "V-002", VehicleStatus.InShop,  "Public Works",       FuelType.Gasoline),
            NewVehicle(3, "V-003", VehicleStatus.Active,  "Parks and Recreation", FuelType.Electric)
        );
        Context.SaveChanges();

        _repository = new VehicleRepository(Context);
    }

    // ── GetAllAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WithNoFilter_ReturnsAllVehiclesOrderedByAssetNumber()
    {
        // Act
        var allVehicles = await _repository.GetAllAsync();

        // Result
        allVehicles.Should().HaveCount(3);
        allVehicles.Select(v => v.AssetNumber).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ReturnsOnlyMatchingVehicles()
    {
        // Act
        var activeVehicles = await _repository.GetAllAsync(status: VehicleStatus.Active);

        // Result
        activeVehicles.Should().HaveCount(2);
        activeVehicles.Should().AllSatisfy(v => v.Status.Should().Be(VehicleStatus.Active));
    }

    [Fact]
    public async Task GetAllAsync_WithDepartmentFilter_ReturnsOnlyMatchingVehicles()
    {
        // Act
        var parksVehicles = await _repository.GetAllAsync(department: "Parks and Recreation");

        // Result
        parksVehicles.Should().HaveCount(1);
        parksVehicles[0].AssetNumber.Should().Be("V-003");
    }

    [Fact]
    public async Task GetAllAsync_WithFuelTypeFilter_ReturnsOnlyMatchingVehicles()
    {
        // Act
        var electricVehicles = await _repository.GetAllAsync(fuelType: FuelType.Electric);

        // Result
        electricVehicles.Should().HaveCount(1);
        electricVehicles[0].AssetNumber.Should().Be("V-003");
    }

    // ── GetByIdAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenVehicleExists_ReturnsVehicleWithMaintenanceSchedules()
    {
        // Setup
        Context.MaintenanceSchedules.Add(NewMaintenanceSchedule(id: 1, vehicleId: 1, nextDueDate: DateTime.UtcNow.AddDays(10)));
        Context.SaveChanges();

        // Act
        var vehicleWithSchedules = await _repository.GetByIdAsync(1);

        // Result
        Assert.NotNull(vehicleWithSchedules);
        vehicleWithSchedules.AssetNumber.Should().Be("V-001");
        vehicleWithSchedules.MaintenanceSchedules.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenVehicleDoesNotExist_ReturnsNull()
    {
        // Act
        var missingVehicle = await _repository.GetByIdAsync(999);

        // Result
        Assert.Null(missingVehicle);
    }

    // ── GetByAssetNumberAsync ──────────────────────────────────────

    [Fact]
    public async Task GetByAssetNumberAsync_WhenFound_ReturnsMatchingVehicle()
    {
        // Act
        var vehicle = await _repository.GetByAssetNumberAsync("V-002");

        // Result
        Assert.NotNull(vehicle);
        vehicle.Id.Should().Be(2);
        vehicle.Status.Should().Be(VehicleStatus.InShop);
    }

    [Fact]
    public async Task GetByAssetNumberAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var missingVehicle = await _repository.GetByAssetNumberAsync("V-DOES-NOT-EXIST");

        // Result
        Assert.Null(missingVehicle);
    }

    // ── SearchAsync ────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_WithMakeFilter_ReturnsCaseInsensitiveMatches()
    {
        // Setup (all 3 seeded vehicles have Make = "Ford")

        // Act
        var fordVehicles = await _repository.SearchAsync(make: "ford");

        // Result
        fordVehicles.Should().HaveCount(3);
    }

    [Fact]
    public async Task SearchAsync_WithModelFilter_ReturnsCaseInsensitiveMatches()
    {
        // Setup (all 3 seeded vehicles have Model = "F-150")

        // Act
        var vehicles = await _repository.SearchAsync(model: "f-150");

        // Result
        vehicles.Should().HaveCount(3);
    }

    [Fact]
    public async Task SearchAsync_WithMultipleFilters_ReturnsOnlyMatchingVehicles()
    {
        // Act
        var activePublicWorksVehicles = await _repository.SearchAsync(
            department: "public works",
            status: VehicleStatus.Active);

        // Result
        activePublicWorksVehicles.Should().HaveCount(1);
        activePublicWorksVehicles[0].AssetNumber.Should().Be("V-001");
    }

    // ── GetFleetSummaryAsync ───────────────────────────────────────

    [Fact]
    public async Task GetFleetSummaryAsync_ReturnsTotalCountAndGroupedBreakdowns()
    {
        // Setup (3 vehicles: 2 Active/1 InShop, 2 Gasoline/1 Electric, 2 PublicWorks/1 Parks)

        // Act
        var summary = await _repository.GetFleetSummaryAsync();

        // Result
        summary.TotalVehicles.Should().Be(3);
        summary.ByStatus["Active"].Should().Be(2);
        summary.ByStatus["InShop"].Should().Be(1);
        summary.ByFuelType["Gasoline"].Should().Be(2);
        summary.ByFuelType["Electric"].Should().Be(1);
        summary.ByDepartment["Public Works"].Should().Be(2);
        summary.ByDepartment["Parks and Recreation"].Should().Be(1);
    }

    // ── GetVehiclesByMaintenanceCostAsync ──────────────────────────

    [Fact]
    public async Task GetVehiclesByMaintenanceCostAsync_WhenNoMaintenanceRecords_ReturnsEmptyList()
    {
        // Setup (3 vehicles seeded in constructor, zero maintenance records)

        // Act
        var costList = await _repository.GetVehiclesByMaintenanceCostAsync();

        // Result
        costList.Should().BeEmpty();
    }
}
