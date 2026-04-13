using FleetWise.Api.Plugins;
using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;
using FleetWise.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.SemanticKernel;
using Moq;

namespace FleetWise.Api.Tests.Plugins;

/// <summary>
/// Tests for FleetQueryPlugin invoked through the Semantic Kernel -- the same way
/// the LLM calls these functions at runtime via Kernel.InvokeAsync.
/// </summary>
public class FleetQueryPluginTests
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository = new();

    private Kernel CreateKernelWithFleetQueryPlugin()
    {
        var kernel = Kernel.CreateBuilder().Build();
        kernel.ImportPluginFromObject(
            new FleetQueryPlugin(_mockVehicleRepository.Object), "FleetQuery");
        return kernel;
    }

    // ── get_fleet_summary ────────────────────────────────────────────

    [Fact]
    public async Task GetFleetSummary_WhenRepositoryReturnsSummary_ReturnsFormattedJsonWithTotalVehicleCount()
    {
        // Setup
        var fleetSummaryWithThirtyFiveVehicles = new FleetSummary(
            TotalVehicles: 35,
            ByStatus: new Dictionary<string, int> { ["Active"] = 29, ["InShop"] = 4, ["OutOfService"] = 2 },
            ByFuelType: new Dictionary<string, int> { ["Gasoline"] = 20, ["Diesel"] = 9 },
            ByDepartment: new Dictionary<string, int> { ["Public Works"] = 12 });

        _mockVehicleRepository
            .Setup(r => r.GetFleetSummaryAsync())
            .ReturnsAsync(fleetSummaryWithThirtyFiveVehicles);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var fleetSummaryFunctionResult = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "get_fleet_summary");

        // Result
        var fleetSummaryJsonResponse = fleetSummaryFunctionResult.ToString();
        fleetSummaryJsonResponse.Should().Contain("35 total vehicles");
        fleetSummaryJsonResponse.Should().Contain("Active");
        fleetSummaryJsonResponse.Should().Contain("Public Works");
    }

    // ── get_vehicle_by_asset_number ──────────────────────────────────

    [Fact]
    public async Task GetVehicleByAssetNumber_WhenVehicleExists_ReturnsFormattedVehicleDetailsAsJson()
    {
        // Setup
        var activeFordF150InPublicWorks = new Vehicle
        {
            Id = 1,
            AssetNumber = "V-2019-0042",
            VIN = "1FTFW1E50KFA00042",
            Year = 2019,
            Make = "Ford",
            Model = "F-150 XL",
            FuelType = FuelType.Gasoline,
            Status = VehicleStatus.Active,
            Department = "Public Works",
            AssignedDriver = "John Smith",
            CurrentMileage = 87432,
            AcquisitionDate = new DateTime(2019, 3, 15),
            AcquisitionCost = 35000m,
            LicensePlate = "GOV-1234",
            Location = "Main Garage"
        };

        _mockVehicleRepository
            .Setup(r => r.GetByAssetNumberAsync("V-2019-0042"))
            .ReturnsAsync(activeFordF150InPublicWorks);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var vehicleLookupResult = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "get_vehicle_by_asset_number",
            new KernelArguments { ["assetNumber"] = "V-2019-0042" });

        // Result
        var vehicleDetailsJsonResponse = vehicleLookupResult.ToString();
        vehicleDetailsJsonResponse.Should().Contain("V-2019-0042");
        vehicleDetailsJsonResponse.Should().Contain("2019 Ford F-150 XL");
        vehicleDetailsJsonResponse.Should().Contain("Public Works");
        vehicleDetailsJsonResponse.Should().Contain("Gasoline");
        vehicleDetailsJsonResponse.Should().Contain("87432");
    }

    [Fact]
    public async Task GetVehicleByAssetNumber_WhenVehicleDoesNotExist_ReturnsNotFoundMessage()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetByAssetNumberAsync("V-9999-0000"))
            .ReturnsAsync((Vehicle?)null);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var vehicleLookupResult = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "get_vehicle_by_asset_number",
            new KernelArguments { ["assetNumber"] = "V-9999-0000" });

        // Result
        vehicleLookupResult.ToString().Should().Be("No vehicle found with asset number V-9999-0000.");
    }

    // ── search_vehicles ─────────────────────────────────────────────

    [Fact]
    public async Task SearchVehicles_WhenInvalidStatusProvided_ReturnsErrorWithValidStatusValues()
    {
        // Setup
        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var searchResultWithInvalidStatus = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "search_vehicles",
            new KernelArguments { ["status"] = "Exploded" });

        // Result
        var invalidStatusErrorMessage = searchResultWithInvalidStatus.ToString();
        invalidStatusErrorMessage.Should().Contain("Invalid status 'Exploded'");
        invalidStatusErrorMessage.Should().Contain("Active, InShop, OutOfService, Retired");
    }

    [Fact]
    public async Task SearchVehicles_WhenInvalidFuelTypeProvided_ReturnsErrorWithValidFuelTypeValues()
    {
        // Setup
        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var searchResultWithInvalidFuelType = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "search_vehicles",
            new KernelArguments { ["fuelType"] = "Nuclear" });

        // Result
        var invalidFuelTypeErrorMessage = searchResultWithInvalidFuelType.ToString();
        invalidFuelTypeErrorMessage.Should().Contain("Invalid fuel type 'Nuclear'");
        invalidFuelTypeErrorMessage.Should().Contain("Gasoline, Diesel, Electric, Hybrid, CNG");
    }

    [Fact]
    public async Task SearchVehicles_WhenNoVehiclesMatchCriteria_ReturnsNoVehiclesFoundMessage()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.SearchAsync("Lamborghini", null, null, null, null))
            .ReturnsAsync(new List<Vehicle>());

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var searchResultForLamborghini = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "search_vehicles",
            new KernelArguments { ["make"] = "Lamborghini" });

        // Result
        searchResultForLamborghini.ToString().Should().Be("No vehicles found matching the specified criteria.");
    }

    [Fact]
    public async Task SearchVehicles_WhenVehiclesMatchMakeAndStatusFilter_ReturnsFormattedResultsWithCount()
    {
        // Setup
        var twoActiveFordVehicles = new List<Vehicle>
        {
            new()
            {
                Id = 1, AssetNumber = "V-2019-0001", VIN = "1FTFW1E50KFA00001",
                Year = 2019, Make = "Ford", Model = "F-150 XL",
                FuelType = FuelType.Gasoline, Status = VehicleStatus.Active,
                Department = "Public Works", CurrentMileage = 87432,
                AcquisitionDate = DateTime.Now, AcquisitionCost = 35000m,
                LicensePlate = "GOV-0001", Location = "Main Garage"
            },
            new()
            {
                Id = 2, AssetNumber = "V-2020-0005", VIN = "1FTFW1E50LFA00005",
                Year = 2020, Make = "Ford", Model = "Transit 150",
                FuelType = FuelType.Gasoline, Status = VehicleStatus.Active,
                Department = "Parks and Recreation", CurrentMileage = 52870,
                AcquisitionDate = DateTime.Now, AcquisitionCost = 32000m,
                LicensePlate = "GOV-0005", Location = "East Lot"
            }
        };

        _mockVehicleRepository
            .Setup(r => r.SearchAsync("Ford", null, null, VehicleStatus.Active, null))
            .ReturnsAsync(twoActiveFordVehicles);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var searchResultForActiveFords = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "search_vehicles",
            new KernelArguments { ["make"] = "Ford", ["status"] = "Active" });

        // Result
        var activeFordSearchJsonResponse = searchResultForActiveFords.ToString();
        activeFordSearchJsonResponse.Should().Contain("Found 2 vehicles matching criteria");
        activeFordSearchJsonResponse.Should().Contain("V-2019-0001");
        activeFordSearchJsonResponse.Should().Contain("V-2020-0005");
    }

    [Fact]
    public async Task SearchVehicles_WhenStatusProvidedInMixedCase_ParsesEnumCaseInsensitively()
    {
        // Setup
        var oneVehicleInShop = new List<Vehicle>
        {
            new()
            {
                Id = 3, AssetNumber = "V-2018-0003", VIN = "1GCGG25K681000003",
                Year = 2018, Make = "Chevrolet", Model = "Express 2500",
                FuelType = FuelType.Gasoline, Status = VehicleStatus.InShop,
                Department = "Water Department", CurrentMileage = 110500,
                AcquisitionDate = DateTime.Now, AcquisitionCost = 28000m,
                LicensePlate = "GOV-0003", Location = "Main Garage"
            }
        };

        _mockVehicleRepository
            .Setup(r => r.SearchAsync(null, null, null, VehicleStatus.InShop, null))
            .ReturnsAsync(oneVehicleInShop);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var searchResultWithMixedCaseStatus = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "search_vehicles",
            new KernelArguments { ["status"] = "inshop" });

        // Result
        searchResultWithMixedCaseStatus.ToString().Should().Contain("Found 1 vehicles matching criteria");
    }

    [Fact]
    public async Task SearchVehicles_WhenFuelTypeProvidedInLowerCase_ParsesEnumCaseInsensitively()
    {
        // Setup
        var oneElectricVehicle = new List<Vehicle>
        {
            new()
            {
                Id = 4, AssetNumber = "V-2023-0010", VIN = "5YJ3E1EA1NF000010",
                Year = 2023, Make = "Tesla", Model = "Model 3",
                FuelType = FuelType.Electric, Status = VehicleStatus.Active,
                Department = "Administration", CurrentMileage = 12000,
                AcquisitionDate = DateTime.Now, AcquisitionCost = 45000m,
                LicensePlate = "GOV-0010", Location = "City Hall"
            }
        };

        _mockVehicleRepository
            .Setup(r => r.SearchAsync(null, null, null, null, FuelType.Electric))
            .ReturnsAsync(oneElectricVehicle);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var searchResultWithLowerCaseFuelType = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "search_vehicles",
            new KernelArguments { ["fuelType"] = "electric" });

        // Result
        searchResultWithLowerCaseFuelType.ToString().Should().Contain("Found 1 vehicles matching criteria");
    }

    [Fact]
    public async Task SearchVehicles_WhenNoFiltersProvided_SearchesWithAllNullParameters()
    {
        // Setup
        var threeVehiclesFromUnfilteredSearch = new List<Vehicle>
        {
            new()
            {
                Id = 1, AssetNumber = "V-2019-0001", VIN = "1FTFW1E50KFA00001",
                Year = 2019, Make = "Ford", Model = "F-150 XL",
                FuelType = FuelType.Gasoline, Status = VehicleStatus.Active,
                Department = "Public Works", CurrentMileage = 87432,
                AcquisitionDate = DateTime.Now, AcquisitionCost = 35000m,
                LicensePlate = "GOV-0001", Location = "Main Garage"
            },
            new()
            {
                Id = 2, AssetNumber = "V-2020-0002", VIN = "1GCGG25K681000002",
                Year = 2020, Make = "Chevrolet", Model = "Silverado",
                FuelType = FuelType.Diesel, Status = VehicleStatus.Active,
                Department = "Water Department", CurrentMileage = 65000,
                AcquisitionDate = DateTime.Now, AcquisitionCost = 40000m,
                LicensePlate = "GOV-0002", Location = "South Yard"
            },
            new()
            {
                Id = 3, AssetNumber = "V-2021-0003", VIN = "5YJ3E1EA1NF000003",
                Year = 2021, Make = "Tesla", Model = "Model Y",
                FuelType = FuelType.Electric, Status = VehicleStatus.Active,
                Department = "Administration", CurrentMileage = 20000,
                AcquisitionDate = DateTime.Now, AcquisitionCost = 50000m,
                LicensePlate = "GOV-0003", Location = "City Hall"
            }
        };

        _mockVehicleRepository
            .Setup(r => r.SearchAsync(null, null, null, null, null))
            .ReturnsAsync(threeVehiclesFromUnfilteredSearch);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var unfilteredSearchResult = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "search_vehicles");

        // Result
        unfilteredSearchResult.ToString().Should().Contain("Found 3 vehicles matching criteria");
        _mockVehicleRepository.Verify(r => r.SearchAsync(null, null, null, null, null), Times.Once);
    }

    // ── get_vehicles_by_high_maintenance_cost ────────────────────────

    [Fact]
    public async Task GetVehiclesByHighMaintenanceCost_WhenNoDataAvailable_ReturnsNoDataMessage()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetVehiclesByMaintenanceCostAsync(10))
            .ReturnsAsync(new List<VehicleMaintenanceCost>());

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var maintenanceCostResult = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "get_vehicles_by_high_maintenance_cost");

        // Result
        maintenanceCostResult.ToString().Should().Be("No maintenance cost data available.");
    }

    [Fact]
    public async Task GetVehiclesByHighMaintenanceCost_WhenDataExists_ReturnsFormattedCostRanking()
    {
        // Setup
        var topThreeVehiclesByMaintenanceCost = new List<VehicleMaintenanceCost>
        {
            new(VehicleId: 7, AssetNumber: "V-2017-0007", Year: 2017, Make: "Ford", Model: "F-150 XL", TotalMaintenanceCost: 8500.00m, RecordCount: 12),
            new(VehicleId: 1, AssetNumber: "V-2019-0001", Year: 2019, Make: "Ford", Model: "F-150 XL", TotalMaintenanceCost: 5200.00m, RecordCount: 8),
            new(VehicleId: 3, AssetNumber: "V-2018-0003", Year: 2018, Make: "Chevrolet", Model: "Express 2500", TotalMaintenanceCost: 3100.00m, RecordCount: 5)
        };

        _mockVehicleRepository
            .Setup(r => r.GetVehiclesByMaintenanceCostAsync(5))
            .ReturnsAsync(topThreeVehiclesByMaintenanceCost);

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        var maintenanceCostResult = await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "get_vehicles_by_high_maintenance_cost",
            new KernelArguments { ["topN"] = 5 });

        // Result
        var costRankingJsonResponse = maintenanceCostResult.ToString();
        costRankingJsonResponse.Should().Contain("Top 3 vehicles by maintenance cost");
        costRankingJsonResponse.Should().Contain("V-2017-0007");
        costRankingJsonResponse.Should().Contain("8500");
    }

    [Fact]
    public async Task GetVehiclesByHighMaintenanceCost_WhenCustomTopNProvided_PassesValueToRepository()
    {
        // Setup
        _mockVehicleRepository
            .Setup(r => r.GetVehiclesByMaintenanceCostAsync(3))
            .ReturnsAsync(new List<VehicleMaintenanceCost>
            {
                new(VehicleId: 7, AssetNumber: "V-2017-0007", Year: 2017, Make: "Ford", Model: "F-150 XL", TotalMaintenanceCost: 8500.00m, RecordCount: 12)
            });

        var kernelWithFleetQueryPlugin = CreateKernelWithFleetQueryPlugin();

        // Act
        await kernelWithFleetQueryPlugin.InvokeAsync(
            "FleetQuery", "get_vehicles_by_high_maintenance_cost",
            new KernelArguments { ["topN"] = 3 });

        // Result
        _mockVehicleRepository.Verify(r => r.GetVehiclesByMaintenanceCostAsync(3), Times.Once);
    }
}
