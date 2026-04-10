using FleetWise.Domain.Entities;
using FleetWise.Domain.Enums;

namespace FleetWise.Infrastructure.Data;

public static class SeedData
{
    public static void Initialize(FleetDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Vehicles.Any())
            return;

        // ──────────────────────────────────────────────
        //  VEHICLES (35 total)
        // ──────────────────────────────────────────────
        var vehicles = new List<Vehicle>
        {
            // --- Pickup Trucks (8) ---
            new Vehicle { Id = 1, AssetNumber = "V-2019-0001", VIN = "1FTEW1EP5KFA00001", Year = 2019, Make = "Ford", Model = "F-150 XL", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Tom Bradley", CurrentMileage = 87432, AcquisitionDate = new DateTime(2019, 3, 15), AcquisitionCost = 34500.00m, LicensePlate = "MUN-1001", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 2, AssetNumber = "V-2020-0002", VIN = "1GCUYDED3LZ100002", Year = 2020, Make = "Chevrolet", Model = "Silverado 1500", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Carlos Mendez", CurrentMileage = 72105, AcquisitionDate = new DateTime(2020, 1, 10), AcquisitionCost = 36200.00m, LicensePlate = "MUN-1002", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 3, AssetNumber = "V-2021-0003", VIN = "1FTEW1EP9MFA00003", Year = 2021, Make = "Ford", Model = "F-150 XLT", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Parks and Recreation", AssignedDriver = "Angela Rivera", CurrentMileage = 54210, AcquisitionDate = new DateTime(2021, 6, 1), AcquisitionCost = 38750.00m, LicensePlate = "MUN-1003", Location = "North Yard", Notes = null },
            new Vehicle { Id = 4, AssetNumber = "V-2018-0004", VIN = "1GCUYDED7JZ100004", Year = 2018, Make = "Chevrolet", Model = "Silverado 2500HD", FuelType = FuelType.Diesel, Status = VehicleStatus.Active, Department = "Water Department", AssignedDriver = "Ray Patterson", CurrentMileage = 112540, AcquisitionDate = new DateTime(2018, 9, 20), AcquisitionCost = 42100.00m, LicensePlate = "MUN-1004", Location = "South Facility", Notes = "Plow attachment equipped" },
            new Vehicle { Id = 5, AssetNumber = "V-2022-0005", VIN = "1FTEW1EP2NFA00005", Year = 2022, Make = "Ford", Model = "F-250 Super Duty", FuelType = FuelType.Diesel, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Mike Jensen", CurrentMileage = 41380, AcquisitionDate = new DateTime(2022, 2, 14), AcquisitionCost = 47800.00m, LicensePlate = "MUN-1005", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 6, AssetNumber = "V-2020-0006", VIN = "1GCUYDED1LZ100006", Year = 2020, Make = "Chevrolet", Model = "Silverado 1500", FuelType = FuelType.Gasoline, Status = VehicleStatus.InShop, Department = "Building Inspection", AssignedDriver = "Patricia Nguyen", CurrentMileage = 68930, AcquisitionDate = new DateTime(2020, 5, 22), AcquisitionCost = 35800.00m, LicensePlate = "MUN-1006", Location = "Central Garage", Notes = "Transmission issues reported" },
            new Vehicle { Id = 7, AssetNumber = "V-2017-0007", VIN = "1FTEW1EP6HFA00007", Year = 2017, Make = "Ford", Model = "F-150 XL", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Bill Kowalski", CurrentMileage = 142680, AcquisitionDate = new DateTime(2017, 4, 3), AcquisitionCost = 32000.00m, LicensePlate = "MUN-1007", Location = "Central Garage", Notes = "High maintenance history - lifecycle review pending" },
            new Vehicle { Id = 8, AssetNumber = "V-2023-0008", VIN = "1GCUYDED5PZ100008", Year = 2023, Make = "Chevrolet", Model = "Silverado 1500 RST", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Parks and Recreation", AssignedDriver = "Derek Simmons", CurrentMileage = 28450, AcquisitionDate = new DateTime(2023, 1, 18), AcquisitionCost = 41200.00m, LicensePlate = "MUN-1008", Location = "North Yard", Notes = null },

            // --- Vans (5) ---
            new Vehicle { Id = 9, AssetNumber = "V-2021-0009", VIN = "1FTBW2CM3MKA00009", Year = 2021, Make = "Ford", Model = "Transit 250 Cargo", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Water Department", AssignedDriver = "Helen Park", CurrentMileage = 61200, AcquisitionDate = new DateTime(2021, 8, 12), AcquisitionCost = 39500.00m, LicensePlate = "MUN-2001", Location = "South Facility", Notes = null },
            new Vehicle { Id = 10, AssetNumber = "V-2020-0010", VIN = "1FTBW2CM7LKA00010", Year = 2020, Make = "Ford", Model = "Transit 350 Cargo", FuelType = FuelType.Gasoline, Status = VehicleStatus.InShop, Department = "Public Works", AssignedDriver = "Greg Hoffman", CurrentMileage = 78430, AcquisitionDate = new DateTime(2020, 3, 5), AcquisitionCost = 41200.00m, LicensePlate = "MUN-2002", Location = "Central Garage", Notes = "Brake system overhaul in progress" },
            new Vehicle { Id = 11, AssetNumber = "V-2022-0011", VIN = "1FTBW2CM1NKA00011", Year = 2022, Make = "Ford", Model = "Transit 250 Cargo", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Building Inspection", AssignedDriver = "Maria Santos", CurrentMileage = 43560, AcquisitionDate = new DateTime(2022, 7, 19), AcquisitionCost = 40100.00m, LicensePlate = "MUN-2003", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 12, AssetNumber = "V-2019-0012", VIN = "1FTBW2CM9KKA00012", Year = 2019, Make = "Ford", Model = "Transit 150 Passenger", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Parks and Recreation", AssignedDriver = null, CurrentMileage = 52870, AcquisitionDate = new DateTime(2019, 11, 8), AcquisitionCost = 37600.00m, LicensePlate = "MUN-2004", Location = "North Yard", Notes = "Pool vehicle for crew transport" },
            new Vehicle { Id = 13, AssetNumber = "V-2023-0013", VIN = "1FTBW2CM4PKA00013", Year = 2023, Make = "Ford", Model = "Transit 250 Cargo", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Water Department", AssignedDriver = "Kevin Wu", CurrentMileage = 19840, AcquisitionDate = new DateTime(2023, 4, 25), AcquisitionCost = 42500.00m, LicensePlate = "MUN-2005", Location = "South Facility", Notes = null },

            // --- Sedans (7) ---
            new Vehicle { Id = 14, AssetNumber = "V-2021-0014", VIN = "4T1BF1FK5MU100014", Year = 2021, Make = "Toyota", Model = "Camry LE", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Administration", AssignedDriver = "Janet Collins", CurrentMileage = 38720, AcquisitionDate = new DateTime(2021, 2, 28), AcquisitionCost = 26800.00m, LicensePlate = "MUN-3001", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 15, AssetNumber = "V-2020-0015", VIN = "3FA6P0HD1LR100015", Year = 2020, Make = "Ford", Model = "Fusion SE", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Administration", AssignedDriver = "Robert Kim", CurrentMileage = 45130, AcquisitionDate = new DateTime(2020, 6, 14), AcquisitionCost = 24500.00m, LicensePlate = "MUN-3002", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 16, AssetNumber = "V-2022-0016", VIN = "4T1BF1FK3NU100016", Year = 2022, Make = "Toyota", Model = "Camry SE", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Building Inspection", AssignedDriver = "Diana Lewis", CurrentMileage = 31460, AcquisitionDate = new DateTime(2022, 9, 10), AcquisitionCost = 28200.00m, LicensePlate = "MUN-3003", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 17, AssetNumber = "V-2019-0017", VIN = "3FA6P0HD5KR100017", Year = 2019, Make = "Ford", Model = "Fusion SE", FuelType = FuelType.Gasoline, Status = VehicleStatus.OutOfService, Department = "Administration", AssignedDriver = null, CurrentMileage = 98210, AcquisitionDate = new DateTime(2019, 1, 22), AcquisitionCost = 23800.00m, LicensePlate = "MUN-3004", Location = "Central Garage", Notes = "Retired from daily use - pending disposal" },
            new Vehicle { Id = 18, AssetNumber = "V-2023-0018", VIN = "4T1BF1FK8PU100018", Year = 2023, Make = "Toyota", Model = "Camry LE", FuelType = FuelType.Hybrid, Status = VehicleStatus.Active, Department = "Administration", AssignedDriver = "Susan Wright", CurrentMileage = 22140, AcquisitionDate = new DateTime(2023, 3, 7), AcquisitionCost = 29500.00m, LicensePlate = "MUN-3005", Location = "Central Garage", Notes = "Hybrid model - fuel efficiency pilot" },
            new Vehicle { Id = 19, AssetNumber = "V-2021-0019", VIN = "3FA6P0HD9MR100019", Year = 2021, Make = "Ford", Model = "Fusion Hybrid", FuelType = FuelType.Hybrid, Status = VehicleStatus.Active, Department = "Building Inspection", AssignedDriver = "Thomas Grant", CurrentMileage = 41870, AcquisitionDate = new DateTime(2021, 10, 15), AcquisitionCost = 27600.00m, LicensePlate = "MUN-3006", Location = "Central Garage", Notes = null },
            new Vehicle { Id = 20, AssetNumber = "V-2022-0020", VIN = "4T1BF1FK1NU100020", Year = 2022, Make = "Toyota", Model = "Camry SE", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Water Department", AssignedDriver = "Nancy Chen", CurrentMileage = 29540, AcquisitionDate = new DateTime(2022, 4, 18), AcquisitionCost = 28400.00m, LicensePlate = "MUN-3007", Location = "South Facility", Notes = null },

            // --- Heavy Equipment (7) ---
            new Vehicle { Id = 21, AssetNumber = "V-2018-0021", VIN = "1FVACWDT5JHAB0021", Year = 2018, Make = "Freightliner", Model = "M2 106 Dump Truck", FuelType = FuelType.Diesel, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Frank Deluca", CurrentMileage = 65430, AcquisitionDate = new DateTime(2018, 6, 10), AcquisitionCost = 112000.00m, LicensePlate = "MUN-4001", Location = "Central Garage", Notes = "CDL required" },
            new Vehicle { Id = 22, AssetNumber = "V-2020-0022", VIN = "1FVACWDT7LHAB0022", Year = 2020, Make = "Freightliner", Model = "M2 106 Dump Truck", FuelType = FuelType.Diesel, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Victor Reyes", CurrentMileage = 48210, AcquisitionDate = new DateTime(2020, 8, 25), AcquisitionCost = 118500.00m, LicensePlate = "MUN-4002", Location = "Central Garage", Notes = "CDL required" },
            new Vehicle { Id = 23, AssetNumber = "V-2019-0023", VIN = "1FVACWDT2KHAB0023", Year = 2019, Make = "International", Model = "HV507 Dump Truck", FuelType = FuelType.Diesel, Status = VehicleStatus.InShop, Department = "Public Works", AssignedDriver = "Sam Ortiz", CurrentMileage = 71890, AcquisitionDate = new DateTime(2019, 5, 14), AcquisitionCost = 125000.00m, LicensePlate = "MUN-4003", Location = "Central Garage", Notes = "CDL required - hydraulic lift repair" },
            new Vehicle { Id = 24, AssetNumber = "V-2016-0024", VIN = "CATBHL00XGCAT0024", Year = 2016, Make = "Caterpillar", Model = "420F2 Backhoe Loader", FuelType = FuelType.Diesel, Status = VehicleStatus.Active, Department = "Water Department", AssignedDriver = "Joe Brandt", CurrentMileage = 4820, AcquisitionDate = new DateTime(2016, 11, 3), AcquisitionCost = 98500.00m, LicensePlate = "MUN-4004", Location = "South Facility", Notes = "Hour meter: 6,240 hrs" },
            new Vehicle { Id = 25, AssetNumber = "V-2021-0025", VIN = "CATBHL003MCAT0025", Year = 2021, Make = "Caterpillar", Model = "430F2 Backhoe Loader", FuelType = FuelType.Diesel, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Eddie Marshall", CurrentMileage = 2150, AcquisitionDate = new DateTime(2021, 3, 22), AcquisitionCost = 108000.00m, LicensePlate = "MUN-4005", Location = "Central Garage", Notes = "Hour meter: 3,410 hrs" },
            new Vehicle { Id = 26, AssetNumber = "V-2022-0026", VIN = "1FVACWDT4NHAB0026", Year = 2022, Make = "Freightliner", Model = "M2 106 Dump Truck", FuelType = FuelType.Diesel, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Hank Dawson", CurrentMileage = 32100, AcquisitionDate = new DateTime(2022, 10, 30), AcquisitionCost = 121000.00m, LicensePlate = "MUN-4006", Location = "North Yard", Notes = "CDL required" },
            new Vehicle { Id = 27, AssetNumber = "V-2017-0027", VIN = "1FVACWDT8HHAB0027", Year = 2017, Make = "International", Model = "HV507 Dump Truck", FuelType = FuelType.Diesel, Status = VehicleStatus.OutOfService, Department = "Public Works", AssignedDriver = null, CurrentMileage = 94520, AcquisitionDate = new DateTime(2017, 7, 19), AcquisitionCost = 115000.00m, LicensePlate = "MUN-4007", Location = "Central Garage", Notes = "CDL required - engine failure, pending replacement decision" },

            // --- CNG Vehicle (1) ---
            new Vehicle { Id = 28, AssetNumber = "V-2021-0028", VIN = "1FTEW1CNGMFA00028", Year = 2021, Make = "Ford", Model = "F-150 CNG", FuelType = FuelType.CNG, Status = VehicleStatus.Active, Department = "Public Works", AssignedDriver = "Larry Finn", CurrentMileage = 47600, AcquisitionDate = new DateTime(2021, 5, 11), AcquisitionCost = 41000.00m, LicensePlate = "MUN-5001", Location = "Central Garage", Notes = "CNG fueling at Central Garage only" },

            // --- Additional Fleet (4) ---
            new Vehicle { Id = 29, AssetNumber = "V-2023-0029", VIN = "1FTEW1EP8PFA00029", Year = 2023, Make = "Ford", Model = "F-150 XLT", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Parks and Recreation", AssignedDriver = "Amy Zhao", CurrentMileage = 18920, AcquisitionDate = new DateTime(2023, 8, 5), AcquisitionCost = 42300.00m, LicensePlate = "MUN-1009", Location = "North Yard", Notes = null },
            new Vehicle { Id = 30, AssetNumber = "V-2024-0030", VIN = "1GCUYDED2RZ100030", Year = 2024, Make = "Chevrolet", Model = "Silverado 1500 WT", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Water Department", AssignedDriver = "Paul Dixon", CurrentMileage = 12340, AcquisitionDate = new DateTime(2024, 1, 15), AcquisitionCost = 39800.00m, LicensePlate = "MUN-1010", Location = "South Facility", Notes = null },
            new Vehicle { Id = 31, AssetNumber = "V-2019-0031", VIN = "1FTBW2CM6KKA00031", Year = 2019, Make = "Ford", Model = "Transit 350 Cargo", FuelType = FuelType.Gasoline, Status = VehicleStatus.InShop, Department = "Parks and Recreation", AssignedDriver = "Jim Olsen", CurrentMileage = 83210, AcquisitionDate = new DateTime(2019, 12, 2), AcquisitionCost = 38900.00m, LicensePlate = "MUN-2006", Location = "North Yard", Notes = "Engine coolant leak under investigation" },
            new Vehicle { Id = 32, AssetNumber = "V-2020-0032", VIN = "4T1BF1FK7LU100032", Year = 2020, Make = "Toyota", Model = "Camry LE", FuelType = FuelType.Gasoline, Status = VehicleStatus.Active, Department = "Administration", AssignedDriver = "Wendy Mills", CurrentMileage = 51200, AcquisitionDate = new DateTime(2020, 10, 9), AcquisitionCost = 25900.00m, LicensePlate = "MUN-3008", Location = "Central Garage", Notes = null },

            // --- Electric Vehicles (3) ---
            new Vehicle { Id = 33, AssetNumber = "V-2023-0033", VIN = "1G1FY6S09P4100033", Year = 2023, Make = "Chevrolet", Model = "Bolt EV", FuelType = FuelType.Electric, Status = VehicleStatus.Active, Department = "Administration", AssignedDriver = "Linda Tran", CurrentMileage = 16540, AcquisitionDate = new DateTime(2023, 6, 12), AcquisitionCost = 27500.00m, LicensePlate = "MUN-6001", Location = "Central Garage", Notes = "Level 2 charger at Central Garage" },
            new Vehicle { Id = 34, AssetNumber = "V-2024-0034", VIN = "1FTBW3EVXRKA00034", Year = 2024, Make = "Ford", Model = "E-Transit 350 Cargo", FuelType = FuelType.Electric, Status = VehicleStatus.Active, Department = "Building Inspection", AssignedDriver = "Oscar Vega", CurrentMileage = 8720, AcquisitionDate = new DateTime(2024, 3, 20), AcquisitionCost = 53500.00m, LicensePlate = "MUN-6002", Location = "Central Garage", Notes = "DC fast charge capable" },
            new Vehicle { Id = 35, AssetNumber = "V-2024-0035", VIN = "5YJ3E1EA4RF100035", Year = 2024, Make = "Tesla", Model = "Model 3", FuelType = FuelType.Electric, Status = VehicleStatus.Active, Department = "Administration", AssignedDriver = "Chris Palmer", CurrentMileage = 11230, AcquisitionDate = new DateTime(2024, 5, 8), AcquisitionCost = 42000.00m, LicensePlate = "MUN-6003", Location = "Central Garage", Notes = "Supercharger network access" },
        };
        context.AddRange(vehicles);
        context.SaveChanges();

        // ──────────────────────────────────────────────
        //  PARTS (45 total, 7 below reorder threshold)
        // ──────────────────────────────────────────────
        var parts = new List<Part>
        {
            // --- Oil & Fluids ---
            new Part { Id = 1, PartNumber = "OIL-5W30-5Q", Name = "Motor Oil 5W-30 (5 qt)", Category = "Oil & Fluids", QuantityInStock = 48, ReorderThreshold = 20, UnitCost = 28.99m, Location = "Central Garage" },
            new Part { Id = 2, PartNumber = "OIL-15W40-5Q", Name = "Diesel Motor Oil 15W-40 (5 qt)", Category = "Oil & Fluids", QuantityInStock = 24, ReorderThreshold = 12, UnitCost = 34.50m, Location = "Central Garage" },
            new Part { Id = 3, PartNumber = "OIL-0W20-5Q", Name = "Synthetic Oil 0W-20 (5 qt)", Category = "Oil & Fluids", QuantityInStock = 18, ReorderThreshold = 10, UnitCost = 38.75m, Location = "Central Garage" },
            new Part { Id = 4, PartNumber = "COOL-DEXC-1G", Name = "Dex-Cool Coolant (1 gal)", Category = "Oil & Fluids", QuantityInStock = 15, ReorderThreshold = 8, UnitCost = 18.50m, Location = "Central Garage" },
            new Part { Id = 5, PartNumber = "ATF-MERC-1Q", Name = "Mercon LV ATF (1 qt)", Category = "Oil & Fluids", QuantityInStock = 3, ReorderThreshold = 10, UnitCost = 12.75m, Location = "Central Garage" }, // BELOW THRESHOLD
            new Part { Id = 6, PartNumber = "BRK-DOT4-32", Name = "DOT 4 Brake Fluid (32 oz)", Category = "Oil & Fluids", QuantityInStock = 14, ReorderThreshold = 6, UnitCost = 9.99m, Location = "Central Garage" },
            new Part { Id = 7, PartNumber = "DEF-2.5G", Name = "Diesel Exhaust Fluid (2.5 gal)", Category = "Oil & Fluids", QuantityInStock = 20, ReorderThreshold = 10, UnitCost = 14.25m, Location = "Central Garage" },
            new Part { Id = 8, PartNumber = "HYD-AW46-5G", Name = "Hydraulic Fluid AW46 (5 gal)", Category = "Oil & Fluids", QuantityInStock = 6, ReorderThreshold = 4, UnitCost = 52.00m, Location = "South Facility" },

            // --- Filters ---
            new Part { Id = 9, PartNumber = "FLT-OIL-FL500S", Name = "Oil Filter FL-500S (Ford Gas)", Category = "Filters", QuantityInStock = 30, ReorderThreshold = 15, UnitCost = 8.49m, Location = "Central Garage" },
            new Part { Id = 10, PartNumber = "FLT-OIL-PF63E", Name = "Oil Filter PF63E (Chevy Gas)", Category = "Filters", QuantityInStock = 22, ReorderThreshold = 12, UnitCost = 9.25m, Location = "Central Garage" },
            new Part { Id = 11, PartNumber = "FLT-OIL-LF16", Name = "Oil Filter LF16 (Diesel HD)", Category = "Filters", QuantityInStock = 8, ReorderThreshold = 5, UnitCost = 14.50m, Location = "Central Garage" },
            new Part { Id = 12, PartNumber = "FLT-AIR-FA1884", Name = "Air Filter FA-1884 (Ford F-150)", Category = "Filters", QuantityInStock = 12, ReorderThreshold = 6, UnitCost = 22.99m, Location = "Central Garage" },
            new Part { Id = 13, PartNumber = "FLT-AIR-A3181C", Name = "Air Filter A3181C (Chevy Silverado)", Category = "Filters", QuantityInStock = 10, ReorderThreshold = 6, UnitCost = 21.50m, Location = "Central Garage" },
            new Part { Id = 14, PartNumber = "FLT-FUEL-FD4615", Name = "Fuel Filter FD4615 (Diesel)", Category = "Filters", QuantityInStock = 2, ReorderThreshold = 4, UnitCost = 42.00m, Location = "Central Garage" }, // BELOW THRESHOLD
            new Part { Id = 15, PartNumber = "FLT-CABIN-CF1", Name = "Cabin Air Filter CF-1 (Universal)", Category = "Filters", QuantityInStock = 16, ReorderThreshold = 8, UnitCost = 15.75m, Location = "Central Garage" },

            // --- Brakes ---
            new Part { Id = 16, PartNumber = "BRK-PAD-MKD1414", Name = "Brake Pads MKD1414 (Ford F-150 Front)", Category = "Brakes", QuantityInStock = 8, ReorderThreshold = 4, UnitCost = 45.99m, Location = "Central Garage" },
            new Part { Id = 17, PartNumber = "BRK-PAD-MKD1707", Name = "Brake Pads MKD1707 (Chevy Silverado Front)", Category = "Brakes", QuantityInStock = 6, ReorderThreshold = 4, UnitCost = 48.50m, Location = "Central Garage" },
            new Part { Id = 18, PartNumber = "BRK-ROT-AR8644", Name = "Brake Rotor AR8644 (Ford F-150 Front)", Category = "Brakes", QuantityInStock = 4, ReorderThreshold = 2, UnitCost = 62.75m, Location = "Central Garage" },
            new Part { Id = 19, PartNumber = "BRK-ROT-AR8645", Name = "Brake Rotor AR8645 (Chevy Front)", Category = "Brakes", QuantityInStock = 1, ReorderThreshold = 2, UnitCost = 65.00m, Location = "Central Garage" }, // BELOW THRESHOLD
            new Part { Id = 20, PartNumber = "BRK-PAD-HD7759", Name = "Brake Pads HD7759 (Heavy Duty)", Category = "Brakes", QuantityInStock = 4, ReorderThreshold = 3, UnitCost = 89.99m, Location = "Central Garage" },

            // --- Tires ---
            new Part { Id = 21, PartNumber = "TIR-AT-27565R18", Name = "All-Terrain Tire 275/65R18", Category = "Tires", QuantityInStock = 8, ReorderThreshold = 4, UnitCost = 185.00m, Location = "Central Garage" },
            new Part { Id = 22, PartNumber = "TIR-HWY-26570R17", Name = "Highway Tire 265/70R17", Category = "Tires", QuantityInStock = 6, ReorderThreshold = 4, UnitCost = 165.00m, Location = "Central Garage" },
            new Part { Id = 23, PartNumber = "TIR-COM-24575R16", Name = "Commercial Tire 245/75R16 (Transit)", Category = "Tires", QuantityInStock = 4, ReorderThreshold = 4, UnitCost = 175.00m, Location = "Central Garage" },
            new Part { Id = 24, PartNumber = "TIR-SED-21555R17", Name = "Sedan Tire 215/55R17", Category = "Tires", QuantityInStock = 6, ReorderThreshold = 4, UnitCost = 125.00m, Location = "Central Garage" },
            new Part { Id = 25, PartNumber = "TIR-HD-11R225", Name = "Heavy Duty Tire 11R22.5", Category = "Tires", QuantityInStock = 2, ReorderThreshold = 4, UnitCost = 385.00m, Location = "Central Garage" }, // BELOW THRESHOLD

            // --- Batteries ---
            new Part { Id = 26, PartNumber = "BAT-65-750", Name = "Battery Group 65 750CCA", Category = "Batteries", QuantityInStock = 5, ReorderThreshold = 3, UnitCost = 145.00m, Location = "Central Garage" },
            new Part { Id = 27, PartNumber = "BAT-78-800", Name = "Battery Group 78 800CCA", Category = "Batteries", QuantityInStock = 4, ReorderThreshold = 3, UnitCost = 155.00m, Location = "Central Garage" },
            new Part { Id = 28, PartNumber = "BAT-HD-31-950", Name = "HD Battery Group 31 950CCA", Category = "Batteries", QuantityInStock = 3, ReorderThreshold = 2, UnitCost = 195.00m, Location = "Central Garage" },

            // --- Belts & Hoses ---
            new Part { Id = 29, PartNumber = "BLT-SERP-K060882", Name = "Serpentine Belt K060882 (Ford V8)", Category = "Belts & Hoses", QuantityInStock = 6, ReorderThreshold = 3, UnitCost = 32.50m, Location = "Central Garage" },
            new Part { Id = 30, PartNumber = "BLT-SERP-K060923", Name = "Serpentine Belt K060923 (Chevy V8)", Category = "Belts & Hoses", QuantityInStock = 5, ReorderThreshold = 3, UnitCost = 34.00m, Location = "Central Garage" },
            new Part { Id = 31, PartNumber = "HSE-RAD-UPR-F150", Name = "Upper Radiator Hose (Ford F-150)", Category = "Belts & Hoses", QuantityInStock = 3, ReorderThreshold = 2, UnitCost = 28.75m, Location = "Central Garage" },
            new Part { Id = 32, PartNumber = "HSE-RAD-LWR-F150", Name = "Lower Radiator Hose (Ford F-150)", Category = "Belts & Hoses", QuantityInStock = 3, ReorderThreshold = 2, UnitCost = 31.25m, Location = "Central Garage" },

            // --- Electrical ---
            new Part { Id = 33, PartNumber = "ELC-SPARK-SP547", Name = "Spark Plug SP-547 (Ford)", Category = "Electrical", QuantityInStock = 32, ReorderThreshold = 16, UnitCost = 7.99m, Location = "Central Garage" },
            new Part { Id = 34, PartNumber = "ELC-SPARK-41162", Name = "Spark Plug 41-162 (Chevy)", Category = "Electrical", QuantityInStock = 24, ReorderThreshold = 16, UnitCost = 8.25m, Location = "Central Garage" },
            new Part { Id = 35, PartNumber = "ELC-ALT-FD110", Name = "Alternator FD-110 (Ford F-150)", Category = "Electrical", QuantityInStock = 1, ReorderThreshold = 2, UnitCost = 225.00m, Location = "Central Garage" }, // BELOW THRESHOLD
            new Part { Id = 36, PartNumber = "ELC-STRT-FD220", Name = "Starter Motor FD-220 (Ford)", Category = "Electrical", QuantityInStock = 2, ReorderThreshold = 1, UnitCost = 198.00m, Location = "Central Garage" },
            new Part { Id = 37, PartNumber = "ELC-FUSE-KIT", Name = "Fuse Assortment Kit (Universal)", Category = "Electrical", QuantityInStock = 10, ReorderThreshold = 4, UnitCost = 18.99m, Location = "Central Garage" },

            // --- Steering & Suspension ---
            new Part { Id = 38, PartNumber = "SUS-SHOCK-FT-F150", Name = "Front Shock Absorber (Ford F-150)", Category = "Steering & Suspension", QuantityInStock = 4, ReorderThreshold = 2, UnitCost = 78.50m, Location = "Central Garage" },
            new Part { Id = 39, PartNumber = "SUS-SHOCK-RR-F150", Name = "Rear Shock Absorber (Ford F-150)", Category = "Steering & Suspension", QuantityInStock = 4, ReorderThreshold = 2, UnitCost = 68.25m, Location = "Central Garage" },
            new Part { Id = 40, PartNumber = "STR-TIEROD-OUT", Name = "Outer Tie Rod End (Universal Light)", Category = "Steering & Suspension", QuantityInStock = 6, ReorderThreshold = 3, UnitCost = 42.00m, Location = "Central Garage" },

            // --- Wipers & Lights ---
            new Part { Id = 41, PartNumber = "WPR-BLADE-22", Name = "Wiper Blade 22 inch", Category = "Wipers & Lights", QuantityInStock = 14, ReorderThreshold = 6, UnitCost = 12.99m, Location = "Central Garage" },
            new Part { Id = 42, PartNumber = "WPR-BLADE-18", Name = "Wiper Blade 18 inch", Category = "Wipers & Lights", QuantityInStock = 12, ReorderThreshold = 6, UnitCost = 11.49m, Location = "Central Garage" },
            new Part { Id = 43, PartNumber = "LGT-HEAD-H11", Name = "Headlight Bulb H11", Category = "Wipers & Lights", QuantityInStock = 8, ReorderThreshold = 4, UnitCost = 16.50m, Location = "Central Garage" },
            new Part { Id = 44, PartNumber = "LGT-TAIL-LED", Name = "LED Tail Light Assembly (Universal)", Category = "Wipers & Lights", QuantityInStock = 3, ReorderThreshold = 2, UnitCost = 45.00m, Location = "Central Garage" },

            // --- EV Parts ---
            new Part { Id = 45, PartNumber = "EV-CABIN-BOLT", Name = "Cabin Air Filter (Chevy Bolt)", Category = "EV Components", QuantityInStock = 2, ReorderThreshold = 3, UnitCost = 24.00m, Location = "Central Garage" }, // BELOW THRESHOLD
        };
        context.AddRange(parts);
        context.SaveChanges();

        // ──────────────────────────────────────────────
        //  WORK ORDERS (33+ total: 18 open/in-progress, 15+ completed)
        // ──────────────────────────────────────────────
        var workOrders = new List<WorkOrder>
        {
            // --- Completed Work Orders (15) ---
            new WorkOrder { Id = 1, WorkOrderNumber = "WO-2025-00001", VehicleId = 1, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Scheduled oil change and tire rotation at 82,000 miles", RequestedDate = new DateTime(2025, 3, 10), CompletedDate = new DateTime(2025, 3, 12), AssignedTechnician = "Mike Torres", LaborHours = 1.5m, TotalCost = 125.50m, Notes = null },
            new WorkOrder { Id = 2, WorkOrderNumber = "WO-2025-00002", VehicleId = 2, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Oil change and multi-point inspection", RequestedDate = new DateTime(2025, 4, 5), CompletedDate = new DateTime(2025, 4, 6), AssignedTechnician = "Sarah Chen", LaborHours = 1.0m, TotalCost = 95.00m, Notes = null },
            new WorkOrder { Id = 3, WorkOrderNumber = "WO-2025-00003", VehicleId = 7, Status = WorkOrderStatus.Completed, Priority = Priority.High, Description = "Transmission rebuild - slipping between gears", RequestedDate = new DateTime(2025, 1, 15), CompletedDate = new DateTime(2025, 1, 28), AssignedTechnician = "James Wilson", LaborHours = 18.0m, TotalCost = 4250.00m, Notes = "Transmission had significant internal wear" },
            new WorkOrder { Id = 4, WorkOrderNumber = "WO-2025-00004", VehicleId = 7, Status = WorkOrderStatus.Completed, Priority = Priority.High, Description = "Replace front brake rotors and pads - excessive wear", RequestedDate = new DateTime(2025, 5, 20), CompletedDate = new DateTime(2025, 5, 22), AssignedTechnician = "Mike Torres", LaborHours = 3.5m, TotalCost = 685.00m, Notes = "Rotors were below minimum thickness" },
            new WorkOrder { Id = 5, WorkOrderNumber = "WO-2025-00005", VehicleId = 21, Status = WorkOrderStatus.Completed, Priority = Priority.High, Description = "DOT annual inspection - dump truck", RequestedDate = new DateTime(2025, 6, 1), CompletedDate = new DateTime(2025, 6, 3), AssignedTechnician = "David Park", LaborHours = 4.0m, TotalCost = 520.00m, Notes = "Passed inspection, replaced marker lights" },
            new WorkOrder { Id = 6, WorkOrderNumber = "WO-2025-00006", VehicleId = 3, Status = WorkOrderStatus.Completed, Priority = Priority.Low, Description = "Oil change and air filter replacement", RequestedDate = new DateTime(2025, 7, 8), CompletedDate = new DateTime(2025, 7, 9), AssignedTechnician = "Sarah Chen", LaborHours = 1.0m, TotalCost = 110.00m, Notes = null },
            new WorkOrder { Id = 7, WorkOrderNumber = "WO-2025-00007", VehicleId = 14, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Tire rotation and brake inspection", RequestedDate = new DateTime(2025, 8, 14), CompletedDate = new DateTime(2025, 8, 15), AssignedTechnician = "Lisa Martinez", LaborHours = 1.5m, TotalCost = 85.00m, Notes = null },
            new WorkOrder { Id = 8, WorkOrderNumber = "WO-2025-00008", VehicleId = 7, Status = WorkOrderStatus.Completed, Priority = Priority.Critical, Description = "Alternator failure - vehicle stranded on route", RequestedDate = new DateTime(2025, 9, 3), CompletedDate = new DateTime(2025, 9, 5), AssignedTechnician = "James Wilson", LaborHours = 3.0m, TotalCost = 575.00m, Notes = "Towed from field location. Alternator completely failed." },
            new WorkOrder { Id = 9, WorkOrderNumber = "WO-2025-00009", VehicleId = 9, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Coolant flush and thermostat replacement", RequestedDate = new DateTime(2025, 9, 18), CompletedDate = new DateTime(2025, 9, 20), AssignedTechnician = "Mike Torres", LaborHours = 2.5m, TotalCost = 245.00m, Notes = null },
            new WorkOrder { Id = 10, WorkOrderNumber = "WO-2025-00010", VehicleId = 22, Status = WorkOrderStatus.Completed, Priority = Priority.High, Description = "DOT annual inspection - dump truck", RequestedDate = new DateTime(2025, 10, 2), CompletedDate = new DateTime(2025, 10, 4), AssignedTechnician = "David Park", LaborHours = 4.5m, TotalCost = 610.00m, Notes = "Replaced air brake components during inspection" },
            new WorkOrder { Id = 11, WorkOrderNumber = "WO-2025-00011", VehicleId = 7, Status = WorkOrderStatus.Completed, Priority = Priority.High, Description = "A/C compressor replacement", RequestedDate = new DateTime(2025, 10, 28), CompletedDate = new DateTime(2025, 11, 1), AssignedTechnician = "James Wilson", LaborHours = 5.0m, TotalCost = 1180.00m, Notes = "Compressor seized. Full A/C system recharge." },
            new WorkOrder { Id = 12, WorkOrderNumber = "WO-2025-00012", VehicleId = 5, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Oil change and DEF refill", RequestedDate = new DateTime(2025, 11, 15), CompletedDate = new DateTime(2025, 11, 16), AssignedTechnician = "Sarah Chen", LaborHours = 1.5m, TotalCost = 135.00m, Notes = null },
            new WorkOrder { Id = 13, WorkOrderNumber = "WO-2025-00013", VehicleId = 33, Status = WorkOrderStatus.Completed, Priority = Priority.Low, Description = "EV battery diagnostic and cabin air filter", RequestedDate = new DateTime(2025, 12, 5), CompletedDate = new DateTime(2025, 12, 6), AssignedTechnician = "Lisa Martinez", LaborHours = 1.5m, TotalCost = 78.00m, Notes = "Battery health at 96%. Cabin filter replaced." },
            new WorkOrder { Id = 14, WorkOrderNumber = "WO-2026-00014", VehicleId = 15, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Oil change and serpentine belt replacement", RequestedDate = new DateTime(2026, 1, 10), CompletedDate = new DateTime(2026, 1, 12), AssignedTechnician = "Mike Torres", LaborHours = 2.0m, TotalCost = 165.00m, Notes = "Belt showed visible cracking" },
            new WorkOrder { Id = 15, WorkOrderNumber = "WO-2026-00015", VehicleId = 4, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Fuel filter and air filter replacement", RequestedDate = new DateTime(2026, 2, 3), CompletedDate = new DateTime(2026, 2, 5), AssignedTechnician = "David Park", LaborHours = 2.0m, TotalCost = 185.00m, Notes = null },
            new WorkOrder { Id = 16, WorkOrderNumber = "WO-2025-00016", VehicleId = 12, Status = WorkOrderStatus.Completed, Priority = Priority.Medium, Description = "Brake pad replacement - front and rear", RequestedDate = new DateTime(2025, 11, 20), CompletedDate = new DateTime(2025, 11, 22), AssignedTechnician = "Mike Torres", LaborHours = 3.0m, TotalCost = 340.00m, Notes = null },
            new WorkOrder { Id = 17, WorkOrderNumber = "WO-2025-00017", VehicleId = 24, Status = WorkOrderStatus.Completed, Priority = Priority.High, Description = "Hydraulic cylinder reseal - boom arm", RequestedDate = new DateTime(2025, 8, 5), CompletedDate = new DateTime(2025, 8, 10), AssignedTechnician = "James Wilson", LaborHours = 8.0m, TotalCost = 1450.00m, Notes = "Seals worn, minor scoring on cylinder rod" },
            new WorkOrder { Id = 18, WorkOrderNumber = "WO-2026-00018", VehicleId = 8, Status = WorkOrderStatus.Completed, Priority = Priority.Low, Description = "Oil change and tire rotation", RequestedDate = new DateTime(2026, 1, 22), CompletedDate = new DateTime(2026, 1, 23), AssignedTechnician = "Sarah Chen", LaborHours = 1.5m, TotalCost = 115.00m, Notes = null },

            // --- Open / In-Progress Work Orders (18 total) ---
            // Critical (3)
            new WorkOrder { Id = 19, WorkOrderNumber = "WO-2026-00019", VehicleId = 23, Status = WorkOrderStatus.InProgress, Priority = Priority.Critical, Description = "Hydraulic lift cylinder failure - dump body inoperable", RequestedDate = new DateTime(2026, 4, 2), CompletedDate = null, AssignedTechnician = "James Wilson", LaborHours = null, TotalCost = null, Notes = "Truck is down. Parts ordered, ETA 4/12." },
            new WorkOrder { Id = 20, WorkOrderNumber = "WO-2026-00020", VehicleId = 27, Status = WorkOrderStatus.Open, Priority = Priority.Critical, Description = "Catastrophic engine failure - rod knock detected", RequestedDate = new DateTime(2026, 3, 18), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = "Engine replacement vs. vehicle replacement decision pending fleet manager review" },
            new WorkOrder { Id = 21, WorkOrderNumber = "WO-2026-00021", VehicleId = 7, Status = WorkOrderStatus.InProgress, Priority = Priority.Critical, Description = "Power steering pump failure and rack replacement", RequestedDate = new DateTime(2026, 4, 5), CompletedDate = null, AssignedTechnician = "James Wilson", LaborHours = null, TotalCost = null, Notes = "Fifth major repair this fiscal year. Recommend lifecycle cost review." },

            // AwaitingParts (2)
            new WorkOrder { Id = 22, WorkOrderNumber = "WO-2026-00022", VehicleId = 6, Status = WorkOrderStatus.AwaitingParts, Priority = Priority.High, Description = "Transmission control module replacement", RequestedDate = new DateTime(2026, 3, 25), CompletedDate = null, AssignedTechnician = "David Park", LaborHours = null, TotalCost = null, Notes = "TCM on backorder from GM. Expected delivery 4/18." },
            new WorkOrder { Id = 23, WorkOrderNumber = "WO-2026-00023", VehicleId = 10, Status = WorkOrderStatus.AwaitingParts, Priority = Priority.High, Description = "Complete brake system overhaul - master cylinder and calipers", RequestedDate = new DateTime(2026, 3, 28), CompletedDate = null, AssignedTechnician = "Mike Torres", LaborHours = null, TotalCost = null, Notes = "Rear calipers on order. Front work completed." },

            // InProgress (6 more)
            new WorkOrder { Id = 24, WorkOrderNumber = "WO-2026-00024", VehicleId = 31, Status = WorkOrderStatus.InProgress, Priority = Priority.High, Description = "Engine coolant leak diagnosis and repair", RequestedDate = new DateTime(2026, 4, 3), CompletedDate = null, AssignedTechnician = "Sarah Chen", LaborHours = null, TotalCost = null, Notes = "Suspected head gasket issue. Performing pressure test." },
            new WorkOrder { Id = 25, WorkOrderNumber = "WO-2026-00025", VehicleId = 1, Status = WorkOrderStatus.InProgress, Priority = Priority.Medium, Description = "Oil change, tire rotation, and brake inspection at 87,000 miles", RequestedDate = new DateTime(2026, 4, 7), CompletedDate = null, AssignedTechnician = "Mike Torres", LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 26, WorkOrderNumber = "WO-2026-00026", VehicleId = 26, Status = WorkOrderStatus.InProgress, Priority = Priority.Medium, Description = "DOT annual inspection - dump truck", RequestedDate = new DateTime(2026, 4, 8), CompletedDate = null, AssignedTechnician = "David Park", LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 27, WorkOrderNumber = "WO-2026-00027", VehicleId = 34, Status = WorkOrderStatus.InProgress, Priority = Priority.Medium, Description = "EV battery diagnostic - range degradation reported", RequestedDate = new DateTime(2026, 4, 6), CompletedDate = null, AssignedTechnician = "Lisa Martinez", LaborHours = null, TotalCost = null, Notes = "Driver reports 15% range loss. Running full diagnostic." },
            new WorkOrder { Id = 28, WorkOrderNumber = "WO-2026-00028", VehicleId = 11, Status = WorkOrderStatus.InProgress, Priority = Priority.Medium, Description = "Air filter replacement and AC system recharge", RequestedDate = new DateTime(2026, 4, 8), CompletedDate = null, AssignedTechnician = "Sarah Chen", LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 29, WorkOrderNumber = "WO-2026-00029", VehicleId = 22, Status = WorkOrderStatus.InProgress, Priority = Priority.Medium, Description = "DOT annual inspection - dump truck", RequestedDate = new DateTime(2026, 4, 9), CompletedDate = null, AssignedTechnician = "David Park", LaborHours = null, TotalCost = null, Notes = null },

            // Open (7 more)
            new WorkOrder { Id = 30, WorkOrderNumber = "WO-2026-00030", VehicleId = 2, Status = WorkOrderStatus.Open, Priority = Priority.Medium, Description = "Scheduled oil change and tire rotation at 72,000 miles", RequestedDate = new DateTime(2026, 4, 9), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 31, WorkOrderNumber = "WO-2026-00031", VehicleId = 9, Status = WorkOrderStatus.Open, Priority = Priority.Medium, Description = "Oil change and multi-point inspection", RequestedDate = new DateTime(2026, 4, 9), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 32, WorkOrderNumber = "WO-2026-00032", VehicleId = 16, Status = WorkOrderStatus.Open, Priority = Priority.Low, Description = "Wiper blade replacement and headlight adjustment", RequestedDate = new DateTime(2026, 4, 10), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 33, WorkOrderNumber = "WO-2026-00033", VehicleId = 18, Status = WorkOrderStatus.Open, Priority = Priority.Low, Description = "Hybrid battery health check and cabin filter", RequestedDate = new DateTime(2026, 4, 10), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 34, WorkOrderNumber = "WO-2026-00034", VehicleId = 29, Status = WorkOrderStatus.Open, Priority = Priority.Low, Description = "Oil change and tire rotation at 18,900 miles", RequestedDate = new DateTime(2026, 4, 10), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 35, WorkOrderNumber = "WO-2026-00035", VehicleId = 35, Status = WorkOrderStatus.Open, Priority = Priority.Low, Description = "Cabin air filter replacement and tire rotation", RequestedDate = new DateTime(2026, 4, 10), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = null },
            new WorkOrder { Id = 36, WorkOrderNumber = "WO-2026-00036", VehicleId = 25, Status = WorkOrderStatus.Open, Priority = Priority.Medium, Description = "Hydraulic fluid change and bucket teeth inspection", RequestedDate = new DateTime(2026, 4, 9), CompletedDate = null, AssignedTechnician = null, LaborHours = null, TotalCost = null, Notes = null },
        };
        context.AddRange(workOrders);
        context.SaveChanges();

        // ──────────────────────────────────────────────
        //  MAINTENANCE RECORDS (120+)
        // ──────────────────────────────────────────────
        var maintenanceRecords = new List<MaintenanceRecord>
        {
            // === Vehicle 1: Ford F-150 (2019), 87,432 mi ===
            new MaintenanceRecord { Id = 1, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 3, 15), MileageAtService = 52000, Description = "Oil change - 5W-30 synthetic blend", Cost = 65.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 2, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2023, 3, 15), MileageAtService = 52000, Description = "Tire rotation - all four tires", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 3, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 8, 20), MileageAtService = 57000, Description = "Oil change - 5W-30 synthetic blend", Cost = 65.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 4, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2023, 9, 10), MileageAtService = 57500, Description = "Annual brake inspection - pads at 60%", Cost = 45.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 5, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 1, 12), MileageAtService = 62000, Description = "Oil change - 5W-30 synthetic blend", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 6, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 1, 12), MileageAtService = 62000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 7, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.AirFilter, PerformedDate = new DateTime(2024, 5, 8), MileageAtService = 67000, Description = "Engine air filter replacement", Cost = 42.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 8, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 6, 15), MileageAtService = 67500, Description = "Oil change - 5W-30 synthetic blend", Cost = 68.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 9, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 11, 20), MileageAtService = 72500, Description = "Oil change - 5W-30 synthetic blend", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 10, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 11, 20), MileageAtService = 72500, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 11, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 12, 5), MileageAtService = 73000, Description = "Annual brake inspection - pads at 40%, plan replacement at next service", Cost = 45.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 12, VehicleId = 1, WorkOrderId = 1, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 3, 12), MileageAtService = 77500, Description = "Oil change - 5W-30 synthetic blend", Cost = 68.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 13, VehicleId = 1, WorkOrderId = 1, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2025, 3, 12), MileageAtService = 77500, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 14, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 8, 22), MileageAtService = 82500, Description = "Oil change - 5W-30 synthetic blend", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 15, VehicleId = 1, WorkOrderId = null, MaintenanceType = MaintenanceType.CoolantFlush, PerformedDate = new DateTime(2025, 12, 10), MileageAtService = 85000, Description = "Coolant system flush and refill", Cost = 125.00m, TechnicianName = "Mike Torres" },

            // === Vehicle 2: Chevy Silverado (2020), 72,105 mi ===
            new MaintenanceRecord { Id = 16, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 2, 10), MileageAtService = 42000, Description = "Oil change - 5W-30 conventional", Cost = 62.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 17, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2023, 5, 18), MileageAtService = 45000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 18, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 7, 22), MileageAtService = 47000, Description = "Oil change - 5W-30 conventional", Cost = 62.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 19, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2023, 10, 5), MileageAtService = 50000, Description = "Annual brake inspection - all good", Cost = 45.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 20, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 1, 15), MileageAtService = 52000, Description = "Oil change - 5W-30 conventional", Cost = 65.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 21, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.AirFilter, PerformedDate = new DateTime(2024, 4, 20), MileageAtService = 55000, Description = "Engine air filter replacement", Cost = 40.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 22, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 6, 30), MileageAtService = 57000, Description = "Oil change - 5W-30 synthetic blend", Cost = 68.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 23, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 6, 30), MileageAtService = 57000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 24, VehicleId = 2, WorkOrderId = 2, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 4, 6), MileageAtService = 67000, Description = "Oil change and inspection", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 25, VehicleId = 2, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2025, 10, 12), MileageAtService = 70000, Description = "Annual brake inspection - front pads at 45%", Cost = 45.00m, TechnicianName = "Mike Torres" },

            // === Vehicle 3: Ford F-150 XLT (2021), 54,210 mi ===
            new MaintenanceRecord { Id = 26, VehicleId = 3, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 4, 10), MileageAtService = 25000, Description = "Oil change - 5W-30 synthetic", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 27, VehicleId = 3, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2023, 4, 10), MileageAtService = 25000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 28, VehicleId = 3, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 9, 15), MileageAtService = 30000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 29, VehicleId = 3, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 1, 20), MileageAtService = 35000, Description = "Annual brake inspection", Cost = 45.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 30, VehicleId = 3, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 3, 5), MileageAtService = 37000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 31, VehicleId = 3, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 7, 12), MileageAtService = 42000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 32, VehicleId = 3, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 8, 18), MileageAtService = 43000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 33, VehicleId = 3, WorkOrderId = 6, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 7, 9), MileageAtService = 50000, Description = "Oil change and air filter replacement", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 34, VehicleId = 3, WorkOrderId = 6, MaintenanceType = MaintenanceType.AirFilter, PerformedDate = new DateTime(2025, 7, 9), MileageAtService = 50000, Description = "Engine air filter replacement", Cost = 38.00m, TechnicianName = "Sarah Chen" },

            // === Vehicle 4: Chevy Silverado 2500HD Diesel (2018), 112,540 mi ===
            new MaintenanceRecord { Id = 35, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 1, 18), MileageAtService = 78000, Description = "Oil change - 15W-40 diesel", Cost = 85.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 36, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2023, 4, 5), MileageAtService = 82000, Description = "Tire rotation", Cost = 40.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 37, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 6, 22), MileageAtService = 85000, Description = "Oil change - 15W-40 diesel", Cost = 85.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 38, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2023, 9, 8), MileageAtService = 88000, Description = "Annual brake inspection", Cost = 55.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 39, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 2, 14), MileageAtService = 93000, Description = "Oil change - 15W-40 diesel", Cost = 88.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 40, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.TransmissionService, PerformedDate = new DateTime(2024, 5, 20), MileageAtService = 97000, Description = "Transmission fluid and filter change", Cost = 285.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 41, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 8, 10), MileageAtService = 102000, Description = "Oil change - 15W-40 diesel", Cost = 88.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 42, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.CoolantFlush, PerformedDate = new DateTime(2025, 1, 15), MileageAtService = 107000, Description = "Coolant system flush", Cost = 135.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 43, VehicleId = 4, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 4, 22), MileageAtService = 110000, Description = "Oil change - 15W-40 diesel", Cost = 88.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 44, VehicleId = 4, WorkOrderId = 15, MaintenanceType = MaintenanceType.AirFilter, PerformedDate = new DateTime(2026, 2, 5), MileageAtService = 112000, Description = "Fuel filter and air filter replacement", Cost = 95.00m, TechnicianName = "David Park" },

            // === Vehicle 5: Ford F-250 Diesel (2022), 41,380 mi ===
            new MaintenanceRecord { Id = 45, VehicleId = 5, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 5, 10), MileageAtService = 15000, Description = "Oil change - 15W-40 diesel", Cost = 85.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 46, VehicleId = 5, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 11, 5), MileageAtService = 20000, Description = "Oil change - 15W-40 diesel", Cost = 85.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 47, VehicleId = 5, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 2, 15), MileageAtService = 25000, Description = "Tire rotation", Cost = 40.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 48, VehicleId = 5, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 5, 20), MileageAtService = 30000, Description = "Oil change - 15W-40 diesel", Cost = 88.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 49, VehicleId = 5, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 9, 12), MileageAtService = 34000, Description = "Annual brake inspection", Cost = 55.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 50, VehicleId = 5, WorkOrderId = 12, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 11, 16), MileageAtService = 40000, Description = "Oil change and DEF refill", Cost = 95.00m, TechnicianName = "Sarah Chen" },

            // === Vehicle 7: Ford F-150 "Money Pit" (2017), 142,680 mi ===
            new MaintenanceRecord { Id = 51, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 2, 5), MileageAtService = 105000, Description = "Oil change", Cost = 65.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 52, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2023, 3, 12), MileageAtService = 106000, Description = "Brake inspection - rear pads low", Cost = 45.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 53, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 5, 28), MileageAtService = 110000, Description = "Oil change", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 54, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.TransmissionService, PerformedDate = new DateTime(2023, 7, 15), MileageAtService = 112000, Description = "Transmission fluid flush - shifting rough", Cost = 320.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 55, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.CoolantFlush, PerformedDate = new DateTime(2023, 9, 20), MileageAtService = 115000, Description = "Coolant flush - overheating intermittently", Cost = 185.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 56, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 11, 10), MileageAtService = 118000, Description = "Oil change", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 57, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.BatteryCheck, PerformedDate = new DateTime(2024, 1, 8), MileageAtService = 120000, Description = "Battery replacement - failed load test", Cost = 185.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 58, VehicleId = 7, WorkOrderId = 3, MaintenanceType = MaintenanceType.TransmissionService, PerformedDate = new DateTime(2025, 1, 28), MileageAtService = 128000, Description = "Full transmission rebuild", Cost = 4250.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 59, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 3, 15), MileageAtService = 130000, Description = "Oil change", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 60, VehicleId = 7, WorkOrderId = 4, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2025, 5, 22), MileageAtService = 133000, Description = "Front brake rotor and pad replacement", Cost = 685.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 61, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 7, 10), MileageAtService = 135000, Description = "Oil change", Cost = 68.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 62, VehicleId = 7, WorkOrderId = 8, MaintenanceType = MaintenanceType.BatteryCheck, PerformedDate = new DateTime(2025, 9, 5), MileageAtService = 137000, Description = "Alternator replacement - complete failure", Cost = 575.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 63, VehicleId = 7, WorkOrderId = 11, MaintenanceType = MaintenanceType.AirFilter, PerformedDate = new DateTime(2025, 11, 1), MileageAtService = 139000, Description = "A/C compressor replacement and system recharge", Cost = 1180.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 64, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2026, 1, 20), MileageAtService = 141000, Description = "Oil change", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 65, VehicleId = 7, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2026, 1, 20), MileageAtService = 141000, Description = "Tire rotation - tires showing uneven wear", Cost = 35.00m, TechnicianName = "Sarah Chen" },

            // === Vehicle 9: Ford Transit 250 (2021), 61,200 mi ===
            new MaintenanceRecord { Id = 66, VehicleId = 9, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 6, 10), MileageAtService = 30000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 67, VehicleId = 9, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2023, 6, 10), MileageAtService = 30000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 68, VehicleId = 9, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 12, 8), MileageAtService = 35000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 69, VehicleId = 9, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 3, 15), MileageAtService = 40000, Description = "Annual brake inspection", Cost = 45.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 70, VehicleId = 9, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 6, 20), MileageAtService = 45000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 71, VehicleId = 9, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 12, 12), MileageAtService = 50000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 72, VehicleId = 9, WorkOrderId = 9, MaintenanceType = MaintenanceType.CoolantFlush, PerformedDate = new DateTime(2025, 9, 20), MileageAtService = 58000, Description = "Coolant flush and thermostat replacement", Cost = 245.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 73, VehicleId = 9, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 12, 18), MileageAtService = 60000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },

            // === Vehicle 14: Toyota Camry (2021), 38,720 mi ===
            new MaintenanceRecord { Id = 74, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 3, 20), MileageAtService = 15000, Description = "Oil change - 0W-20 synthetic", Cost = 55.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 75, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 9, 12), MileageAtService = 20000, Description = "Oil change - 0W-20 synthetic", Cost = 55.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 76, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2023, 9, 12), MileageAtService = 20000, Description = "Tire rotation", Cost = 30.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 77, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 3, 8), MileageAtService = 25000, Description = "Oil change - 0W-20 synthetic", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 78, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 6, 15), MileageAtService = 28000, Description = "Annual brake inspection", Cost = 40.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 79, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 9, 22), MileageAtService = 30000, Description = "Oil change - 0W-20 synthetic", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 80, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 9, 22), MileageAtService = 30000, Description = "Tire rotation", Cost = 30.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 81, VehicleId = 14, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 3, 18), MileageAtService = 35000, Description = "Oil change - 0W-20 synthetic", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 82, VehicleId = 14, WorkOrderId = 7, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2025, 8, 15), MileageAtService = 37500, Description = "Tire rotation and brake inspection", Cost = 30.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 83, VehicleId = 14, WorkOrderId = 7, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2025, 8, 15), MileageAtService = 37500, Description = "Annual brake inspection", Cost = 40.00m, TechnicianName = "Lisa Martinez" },

            // === Vehicle 15: Ford Fusion (2020), 45,130 mi ===
            new MaintenanceRecord { Id = 84, VehicleId = 15, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 4, 15), MileageAtService = 25000, Description = "Oil change", Cost = 55.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 85, VehicleId = 15, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 10, 20), MileageAtService = 30000, Description = "Oil change", Cost = 55.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 86, VehicleId = 15, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 2, 8), MileageAtService = 33000, Description = "Tire rotation", Cost = 30.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 87, VehicleId = 15, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 5, 15), MileageAtService = 35000, Description = "Oil change", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 88, VehicleId = 15, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 8, 22), MileageAtService = 38000, Description = "Annual brake inspection", Cost = 40.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 89, VehicleId = 15, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 1, 10), MileageAtService = 40000, Description = "Oil change", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 90, VehicleId = 15, WorkOrderId = 14, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2026, 1, 12), MileageAtService = 44000, Description = "Oil change and serpentine belt replacement", Cost = 125.00m, TechnicianName = "Mike Torres" },

            // === Vehicle 21: Freightliner Dump Truck (2018), 65,430 mi ===
            new MaintenanceRecord { Id = 91, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 3, 8), MileageAtService = 48000, Description = "Oil change - 15W-40 diesel HD", Cost = 120.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 92, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2023, 6, 15), MileageAtService = 50000, Description = "Annual DOT inspection - passed", Cost = 350.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 93, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 9, 20), MileageAtService = 53000, Description = "Oil change - 15W-40 diesel HD", Cost = 120.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 94, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 1, 12), MileageAtService = 55000, Description = "Brake inspection - air brake system check", Cost = 85.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 95, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 4, 18), MileageAtService = 57000, Description = "Oil change - 15W-40 diesel HD", Cost = 120.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 96, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2024, 6, 20), MileageAtService = 58000, Description = "Annual DOT inspection - passed, replaced brake shoes", Cost = 480.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 97, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 10, 5), MileageAtService = 61000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 98, VehicleId = 21, WorkOrderId = 5, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2025, 6, 3), MileageAtService = 64000, Description = "Annual DOT inspection - passed, replaced marker lights", Cost = 520.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 99, VehicleId = 21, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 9, 10), MileageAtService = 65000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },

            // === Vehicle 22: Freightliner Dump Truck (2020), 48,210 mi ===
            new MaintenanceRecord { Id = 100, VehicleId = 22, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 5, 15), MileageAtService = 28000, Description = "Oil change - 15W-40 diesel HD", Cost = 120.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 101, VehicleId = 22, WorkOrderId = null, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2023, 10, 5), MileageAtService = 32000, Description = "Annual DOT inspection - passed", Cost = 350.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 102, VehicleId = 22, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 1, 20), MileageAtService = 35000, Description = "Oil change - 15W-40 diesel HD", Cost = 120.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 103, VehicleId = 22, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 7, 15), MileageAtService = 40000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 104, VehicleId = 22, WorkOrderId = 10, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2025, 10, 4), MileageAtService = 47000, Description = "DOT annual inspection - replaced air brake components", Cost = 610.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 105, VehicleId = 22, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2026, 1, 8), MileageAtService = 48000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },

            // === Vehicle 24: CAT Backhoe (2016), 4,820 mi / 6,240 hrs ===
            new MaintenanceRecord { Id = 106, VehicleId = 24, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 4, 12), MileageAtService = 3800, Description = "Engine oil and hydraulic filter change (500 hr interval)", Cost = 210.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 107, VehicleId = 24, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 3, 18), MileageAtService = 4200, Description = "Engine oil and hydraulic filter change", Cost = 215.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 108, VehicleId = 24, WorkOrderId = 17, MaintenanceType = MaintenanceType.TransmissionService, PerformedDate = new DateTime(2025, 8, 10), MileageAtService = 4700, Description = "Hydraulic cylinder reseal - boom arm", Cost = 1450.00m, TechnicianName = "James Wilson" },

            // === Vehicle 33: Chevy Bolt EV (2023), 16,540 mi ===
            new MaintenanceRecord { Id = 109, VehicleId = 33, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2023, 12, 8), MileageAtService = 7500, Description = "Tire rotation", Cost = 30.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 110, VehicleId = 33, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 6, 15), MileageAtService = 12000, Description = "Tire rotation", Cost = 30.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 111, VehicleId = 33, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 10, 5), MileageAtService = 14000, Description = "Brake inspection - regenerative braking wear minimal", Cost = 35.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 112, VehicleId = 33, WorkOrderId = 13, MaintenanceType = MaintenanceType.EVBatteryDiagnostic, PerformedDate = new DateTime(2025, 12, 6), MileageAtService = 16000, Description = "EV battery diagnostic - 96% health", Cost = 48.00m, TechnicianName = "Lisa Martinez" },

            // === Vehicle 34: Ford E-Transit (2024), 8,720 mi ===
            new MaintenanceRecord { Id = 113, VehicleId = 34, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 10, 12), MileageAtService = 5000, Description = "First tire rotation", Cost = 35.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 114, VehicleId = 34, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2025, 3, 20), MileageAtService = 7500, Description = "Brake inspection", Cost = 35.00m, TechnicianName = "Lisa Martinez" },

            // === Vehicle 35: Tesla Model 3 (2024), 11,230 mi ===
            new MaintenanceRecord { Id = 115, VehicleId = 35, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 11, 18), MileageAtService = 5000, Description = "First tire rotation", Cost = 35.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 116, VehicleId = 35, WorkOrderId = null, MaintenanceType = MaintenanceType.EVBatteryDiagnostic, PerformedDate = new DateTime(2025, 5, 8), MileageAtService = 8000, Description = "EV battery health check - 99% capacity", Cost = 45.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 117, VehicleId = 35, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2025, 10, 15), MileageAtService = 10000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Lisa Martinez" },

            // === Vehicle 8: Chevy Silverado RST (2023), 28,450 mi ===
            new MaintenanceRecord { Id = 118, VehicleId = 8, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 7, 20), MileageAtService = 5000, Description = "First oil change", Cost = 62.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 119, VehicleId = 8, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 1, 15), MileageAtService = 10000, Description = "Oil change", Cost = 65.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 120, VehicleId = 8, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 5, 10), MileageAtService = 15000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 121, VehicleId = 8, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 7, 22), MileageAtService = 18000, Description = "Oil change", Cost = 65.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 122, VehicleId = 8, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 11, 8), MileageAtService = 22000, Description = "Annual brake inspection", Cost = 45.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 123, VehicleId = 8, WorkOrderId = 18, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2026, 1, 23), MileageAtService = 27000, Description = "Oil change and tire rotation", Cost = 68.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 124, VehicleId = 8, WorkOrderId = 18, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2026, 1, 23), MileageAtService = 27000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Sarah Chen" },

            // === Vehicle 10: Ford Transit 350 (2020), 78,430 mi ===
            new MaintenanceRecord { Id = 125, VehicleId = 10, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 8, 15), MileageAtService = 50000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 126, VehicleId = 10, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2024, 2, 10), MileageAtService = 58000, Description = "Brake inspection - front pads low", Cost = 45.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 127, VehicleId = 10, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 6, 18), MileageAtService = 63000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 128, VehicleId = 10, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 2, 5), MileageAtService = 72000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 129, VehicleId = 10, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2025, 6, 20), MileageAtService = 75000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Sarah Chen" },

            // === Vehicle 12: Ford Transit 150 Passenger (2019), 52,870 mi ===
            new MaintenanceRecord { Id = 130, VehicleId = 12, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 5, 10), MileageAtService = 32000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 131, VehicleId = 12, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 1, 18), MileageAtService = 38000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 132, VehicleId = 12, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 5, 12), MileageAtService = 42000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 133, VehicleId = 12, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 8, 15), MileageAtService = 45000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 134, VehicleId = 12, WorkOrderId = 16, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2025, 11, 22), MileageAtService = 52000, Description = "Brake pad replacement - front and rear", Cost = 340.00m, TechnicianName = "Mike Torres" },

            // === Vehicle 23: International Dump Truck (2019), 71,890 mi ===
            new MaintenanceRecord { Id = 135, VehicleId = 23, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 7, 10), MileageAtService = 52000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 136, VehicleId = 23, WorkOrderId = null, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2023, 11, 15), MileageAtService = 56000, Description = "Annual DOT inspection - passed", Cost = 380.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 137, VehicleId = 23, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 3, 8), MileageAtService = 60000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 138, VehicleId = 23, WorkOrderId = null, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2024, 11, 20), MileageAtService = 67000, Description = "Annual DOT inspection - passed", Cost = 390.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 139, VehicleId = 23, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 5, 12), MileageAtService = 70000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },

            // === Scattered records for remaining vehicles ===
            // Vehicle 6: Chevy Silverado InShop
            new MaintenanceRecord { Id = 140, VehicleId = 6, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 8, 5), MileageAtService = 42000, Description = "Oil change", Cost = 65.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 141, VehicleId = 6, WorkOrderId = null, MaintenanceType = MaintenanceType.TransmissionService, PerformedDate = new DateTime(2024, 6, 18), MileageAtService = 55000, Description = "Transmission fluid change", Cost = 265.00m, TechnicianName = "James Wilson" },
            new MaintenanceRecord { Id = 142, VehicleId = 6, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 3, 12), MileageAtService = 65000, Description = "Oil change", Cost = 68.00m, TechnicianName = "Sarah Chen" },

            // Vehicle 11: Ford Transit 250 Cargo
            new MaintenanceRecord { Id = 143, VehicleId = 11, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 9, 10), MileageAtService = 18000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 144, VehicleId = 11, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 4, 15), MileageAtService = 25000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 145, VehicleId = 11, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 9, 20), MileageAtService = 32000, Description = "Tire rotation", Cost = 35.00m, TechnicianName = "Mike Torres" },
            new MaintenanceRecord { Id = 146, VehicleId = 11, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 2, 10), MileageAtService = 38000, Description = "Oil change", Cost = 72.00m, TechnicianName = "Sarah Chen" },
            new MaintenanceRecord { Id = 147, VehicleId = 11, WorkOrderId = null, MaintenanceType = MaintenanceType.BrakeInspection, PerformedDate = new DateTime(2025, 8, 5), MileageAtService = 42000, Description = "Annual brake inspection", Cost = 45.00m, TechnicianName = "Mike Torres" },

            // Vehicle 16: Toyota Camry SE
            new MaintenanceRecord { Id = 148, VehicleId = 16, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 8, 18), MileageAtService = 10000, Description = "Oil change - 0W-20 synthetic", Cost = 55.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 149, VehicleId = 16, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 3, 12), MileageAtService = 15000, Description = "Oil change", Cost = 55.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 150, VehicleId = 16, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 3, 12), MileageAtService = 15000, Description = "Tire rotation", Cost = 30.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 151, VehicleId = 16, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 10, 5), MileageAtService = 22000, Description = "Oil change", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 152, VehicleId = 16, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 5, 20), MileageAtService = 28000, Description = "Oil change", Cost = 58.00m, TechnicianName = "Lisa Martinez" },

            // Vehicle 18: Toyota Camry Hybrid
            new MaintenanceRecord { Id = 153, VehicleId = 18, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 10, 15), MileageAtService = 5000, Description = "First oil change - 0W-20 synthetic", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 154, VehicleId = 18, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 5, 20), MileageAtService = 10000, Description = "Oil change", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 155, VehicleId = 18, WorkOrderId = null, MaintenanceType = MaintenanceType.TireRotation, PerformedDate = new DateTime(2024, 5, 20), MileageAtService = 10000, Description = "Tire rotation", Cost = 30.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 156, VehicleId = 18, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 1, 8), MileageAtService = 15000, Description = "Oil change", Cost = 58.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 157, VehicleId = 18, WorkOrderId = null, MaintenanceType = MaintenanceType.BatteryCheck, PerformedDate = new DateTime(2025, 6, 15), MileageAtService = 18000, Description = "Hybrid battery health check - 98%", Cost = 45.00m, TechnicianName = "Lisa Martinez" },
            new MaintenanceRecord { Id = 158, VehicleId = 18, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2025, 9, 22), MileageAtService = 21000, Description = "Oil change", Cost = 58.00m, TechnicianName = "Lisa Martinez" },

            // Vehicle 26: Freightliner Dump Truck (2022)
            new MaintenanceRecord { Id = 159, VehicleId = 26, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2023, 5, 25), MileageAtService = 10000, Description = "Oil change - 15W-40 diesel HD", Cost = 120.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 160, VehicleId = 26, WorkOrderId = null, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2023, 10, 30), MileageAtService = 15000, Description = "Annual DOT inspection - passed", Cost = 350.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 161, VehicleId = 26, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 4, 10), MileageAtService = 20000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 162, VehicleId = 26, WorkOrderId = null, MaintenanceType = MaintenanceType.OilChange, PerformedDate = new DateTime(2024, 11, 8), MileageAtService = 27000, Description = "Oil change - 15W-40 diesel HD", Cost = 125.00m, TechnicianName = "David Park" },
            new MaintenanceRecord { Id = 163, VehicleId = 26, WorkOrderId = null, MaintenanceType = MaintenanceType.DOTInspection, PerformedDate = new DateTime(2025, 4, 15), MileageAtService = 30000, Description = "Annual DOT inspection - passed", Cost = 365.00m, TechnicianName = "David Park" },
        };
        context.AddRange(maintenanceRecords);
        context.SaveChanges();

        // ──────────────────────────────────────────────
        //  MAINTENANCE SCHEDULES (per vehicle, with some overdue)
        // ──────────────────────────────────────────────
        var maintenanceSchedules = new List<MaintenanceSchedule>
        {
            // === Vehicle 1 ===
            new MaintenanceSchedule { Id = 1, VehicleId = 1, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 8, 22), LastCompletedMileage = 82500, NextDueMileage = 87500, NextDueDate = new DateTime(2026, 2, 18) }, // OVERDUE by date
            new MaintenanceSchedule { Id = 2, VehicleId = 1, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2024, 11, 20), LastCompletedMileage = 72500, NextDueMileage = 80000, NextDueDate = new DateTime(2025, 5, 19) }, // OVERDUE
            new MaintenanceSchedule { Id = 3, VehicleId = 1, MaintenanceType = MaintenanceType.BrakeInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2024, 12, 5), LastCompletedMileage = 73000, NextDueMileage = null, NextDueDate = new DateTime(2025, 12, 5) }, // OVERDUE
            new MaintenanceSchedule { Id = 4, VehicleId = 1, MaintenanceType = MaintenanceType.AirFilter, IntervalMiles = 30000, IntervalDays = null, LastCompletedDate = new DateTime(2024, 5, 8), LastCompletedMileage = 67000, NextDueMileage = 97000, NextDueDate = null },

            // === Vehicle 2 ===
            new MaintenanceSchedule { Id = 5, VehicleId = 2, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 4, 6), LastCompletedMileage = 67000, NextDueMileage = 72000, NextDueDate = new DateTime(2025, 10, 3) }, // OVERDUE
            new MaintenanceSchedule { Id = 6, VehicleId = 2, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2024, 6, 30), LastCompletedMileage = 57000, NextDueMileage = 64500, NextDueDate = new DateTime(2024, 12, 27) }, // OVERDUE
            new MaintenanceSchedule { Id = 7, VehicleId = 2, MaintenanceType = MaintenanceType.BrakeInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 10, 12), LastCompletedMileage = 70000, NextDueMileage = null, NextDueDate = new DateTime(2026, 10, 12) },

            // === Vehicle 3 ===
            new MaintenanceSchedule { Id = 8, VehicleId = 3, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 7, 9), LastCompletedMileage = 50000, NextDueMileage = 55000, NextDueDate = new DateTime(2026, 1, 5) }, // OVERDUE by date
            new MaintenanceSchedule { Id = 9, VehicleId = 3, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2024, 7, 12), LastCompletedMileage = 42000, NextDueMileage = 49500, NextDueDate = new DateTime(2025, 1, 8) }, // OVERDUE
            new MaintenanceSchedule { Id = 10, VehicleId = 3, MaintenanceType = MaintenanceType.BrakeInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2024, 1, 20), LastCompletedMileage = 35000, NextDueMileage = null, NextDueDate = new DateTime(2025, 1, 20) }, // OVERDUE

            // === Vehicle 4 ===
            new MaintenanceSchedule { Id = 11, VehicleId = 4, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 4, 22), LastCompletedMileage = 110000, NextDueMileage = 115000, NextDueDate = new DateTime(2025, 10, 19) }, // OVERDUE
            new MaintenanceSchedule { Id = 12, VehicleId = 4, MaintenanceType = MaintenanceType.BrakeInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2023, 9, 8), LastCompletedMileage = 88000, NextDueMileage = null, NextDueDate = new DateTime(2024, 9, 7) }, // OVERDUE - way overdue
            new MaintenanceSchedule { Id = 13, VehicleId = 4, MaintenanceType = MaintenanceType.TransmissionService, IntervalMiles = 60000, IntervalDays = null, LastCompletedDate = new DateTime(2024, 5, 20), LastCompletedMileage = 97000, NextDueMileage = 157000, NextDueDate = null },

            // === Vehicle 5 ===
            new MaintenanceSchedule { Id = 14, VehicleId = 5, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 11, 16), LastCompletedMileage = 40000, NextDueMileage = 45000, NextDueDate = new DateTime(2026, 5, 15) },
            new MaintenanceSchedule { Id = 15, VehicleId = 5, MaintenanceType = MaintenanceType.BrakeInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2024, 9, 12), LastCompletedMileage = 34000, NextDueMileage = null, NextDueDate = new DateTime(2025, 9, 12) }, // OVERDUE

            // === Vehicle 6 ===
            new MaintenanceSchedule { Id = 16, VehicleId = 6, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 3, 12), LastCompletedMileage = 65000, NextDueMileage = 70000, NextDueDate = new DateTime(2025, 9, 8) }, // OVERDUE

            // === Vehicle 7 - Money Pit ===
            new MaintenanceSchedule { Id = 17, VehicleId = 7, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2026, 1, 20), LastCompletedMileage = 141000, NextDueMileage = 146000, NextDueDate = new DateTime(2026, 7, 19) },
            new MaintenanceSchedule { Id = 18, VehicleId = 7, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2026, 1, 20), LastCompletedMileage = 141000, NextDueMileage = 148500, NextDueDate = new DateTime(2026, 7, 19) },
            new MaintenanceSchedule { Id = 19, VehicleId = 7, MaintenanceType = MaintenanceType.BrakeInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 5, 22), LastCompletedMileage = 133000, NextDueMileage = null, NextDueDate = new DateTime(2026, 5, 22) },

            // === Vehicle 8 ===
            new MaintenanceSchedule { Id = 20, VehicleId = 8, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2026, 1, 23), LastCompletedMileage = 27000, NextDueMileage = 32000, NextDueDate = new DateTime(2026, 7, 22) },
            new MaintenanceSchedule { Id = 21, VehicleId = 8, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2026, 1, 23), LastCompletedMileage = 27000, NextDueMileage = 34500, NextDueDate = new DateTime(2026, 7, 22) },

            // === Vehicle 9 ===
            new MaintenanceSchedule { Id = 22, VehicleId = 9, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 12, 18), LastCompletedMileage = 60000, NextDueMileage = 65000, NextDueDate = new DateTime(2026, 6, 16) },

            // === Vehicle 10 ===
            new MaintenanceSchedule { Id = 23, VehicleId = 10, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 2, 5), LastCompletedMileage = 72000, NextDueMileage = 77000, NextDueDate = new DateTime(2025, 8, 4) }, // OVERDUE

            // === Vehicle 11 ===
            new MaintenanceSchedule { Id = 24, VehicleId = 11, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 2, 10), LastCompletedMileage = 38000, NextDueMileage = 43000, NextDueDate = new DateTime(2025, 8, 9) }, // OVERDUE

            // === Vehicle 12 ===
            new MaintenanceSchedule { Id = 25, VehicleId = 12, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2024, 8, 15), LastCompletedMileage = 45000, NextDueMileage = 50000, NextDueDate = new DateTime(2025, 2, 11) }, // OVERDUE

            // === Vehicle 13 ===
            new MaintenanceSchedule { Id = 26, VehicleId = 13, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 8, 10), LastCompletedMileage = 17000, NextDueMileage = 22000, NextDueDate = new DateTime(2026, 2, 6) }, // OVERDUE by date

            // === Vehicle 14 ===
            new MaintenanceSchedule { Id = 27, VehicleId = 14, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 3, 18), LastCompletedMileage = 35000, NextDueMileage = 40000, NextDueDate = new DateTime(2025, 9, 14) }, // OVERDUE

            // === Vehicle 15 ===
            new MaintenanceSchedule { Id = 28, VehicleId = 15, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2026, 1, 12), LastCompletedMileage = 44000, NextDueMileage = 49000, NextDueDate = new DateTime(2026, 7, 11) },

            // === Vehicle 16 ===
            new MaintenanceSchedule { Id = 29, VehicleId = 16, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 5, 20), LastCompletedMileage = 28000, NextDueMileage = 33000, NextDueDate = new DateTime(2025, 11, 16) }, // OVERDUE

            // === Vehicle 17 (OutOfService - no active schedules needed) ===

            // === Vehicle 18 ===
            new MaintenanceSchedule { Id = 30, VehicleId = 18, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 9, 22), LastCompletedMileage = 21000, NextDueMileage = 26000, NextDueDate = new DateTime(2026, 3, 21) }, // OVERDUE by date

            // === Vehicle 19 ===
            new MaintenanceSchedule { Id = 31, VehicleId = 19, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 7, 15), LastCompletedMileage = 39000, NextDueMileage = 44000, NextDueDate = new DateTime(2026, 1, 11) }, // OVERDUE

            // === Vehicle 20 ===
            new MaintenanceSchedule { Id = 32, VehicleId = 20, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 10, 8), LastCompletedMileage = 27000, NextDueMileage = 32000, NextDueDate = new DateTime(2026, 4, 6) },

            // === Vehicle 21 - Dump Truck ===
            new MaintenanceSchedule { Id = 33, VehicleId = 21, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 9, 10), LastCompletedMileage = 65000, NextDueMileage = 70000, NextDueDate = new DateTime(2026, 3, 9) }, // OVERDUE
            new MaintenanceSchedule { Id = 34, VehicleId = 21, MaintenanceType = MaintenanceType.DOTInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 6, 3), LastCompletedMileage = 64000, NextDueMileage = null, NextDueDate = new DateTime(2026, 6, 3) },

            // === Vehicle 22 - Dump Truck ===
            new MaintenanceSchedule { Id = 35, VehicleId = 22, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2026, 1, 8), LastCompletedMileage = 48000, NextDueMileage = 53000, NextDueDate = new DateTime(2026, 7, 7) },
            new MaintenanceSchedule { Id = 36, VehicleId = 22, MaintenanceType = MaintenanceType.DOTInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 10, 4), LastCompletedMileage = 47000, NextDueMileage = null, NextDueDate = new DateTime(2026, 10, 4) },

            // === Vehicle 23 - Dump Truck InShop ===
            new MaintenanceSchedule { Id = 37, VehicleId = 23, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 5, 12), LastCompletedMileage = 70000, NextDueMileage = 75000, NextDueDate = new DateTime(2025, 11, 8) }, // OVERDUE
            new MaintenanceSchedule { Id = 38, VehicleId = 23, MaintenanceType = MaintenanceType.DOTInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2024, 11, 20), LastCompletedMileage = 67000, NextDueMileage = null, NextDueDate = new DateTime(2025, 11, 20) }, // OVERDUE

            // === Vehicle 24 - Backhoe ===
            new MaintenanceSchedule { Id = 39, VehicleId = 24, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2024, 3, 18), LastCompletedMileage = 4200, NextDueMileage = null, NextDueDate = new DateTime(2025, 3, 18) }, // OVERDUE

            // === Vehicle 25 - Backhoe ===
            new MaintenanceSchedule { Id = 40, VehicleId = 25, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 6, 20), LastCompletedMileage = 2000, NextDueMileage = null, NextDueDate = new DateTime(2026, 6, 20) },

            // === Vehicle 26 - Dump Truck ===
            new MaintenanceSchedule { Id = 41, VehicleId = 26, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2024, 11, 8), LastCompletedMileage = 27000, NextDueMileage = 32000, NextDueDate = new DateTime(2025, 5, 7) }, // OVERDUE
            new MaintenanceSchedule { Id = 42, VehicleId = 26, MaintenanceType = MaintenanceType.DOTInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 4, 15), LastCompletedMileage = 30000, NextDueMileage = null, NextDueDate = new DateTime(2026, 4, 15) },

            // === Vehicle 28 - CNG ===
            new MaintenanceSchedule { Id = 43, VehicleId = 28, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 11, 5), LastCompletedMileage = 45000, NextDueMileage = 50000, NextDueDate = new DateTime(2026, 5, 4) },

            // === Vehicle 29 ===
            new MaintenanceSchedule { Id = 44, VehicleId = 29, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 10, 12), LastCompletedMileage = 16000, NextDueMileage = 21000, NextDueDate = new DateTime(2026, 4, 10) },

            // === Vehicle 30 ===
            new MaintenanceSchedule { Id = 45, VehicleId = 30, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 8, 15), LastCompletedMileage = 10000, NextDueMileage = 15000, NextDueDate = new DateTime(2026, 2, 11) }, // OVERDUE by date

            // === Vehicle 31 ===
            new MaintenanceSchedule { Id = 46, VehicleId = 31, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 6, 10), LastCompletedMileage = 78000, NextDueMileage = 83000, NextDueDate = new DateTime(2025, 12, 7) }, // OVERDUE

            // === Vehicle 32 ===
            new MaintenanceSchedule { Id = 47, VehicleId = 32, MaintenanceType = MaintenanceType.OilChange, IntervalMiles = 5000, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 11, 18), LastCompletedMileage = 49000, NextDueMileage = 54000, NextDueDate = new DateTime(2026, 5, 17) },

            // === Vehicle 33 - EV ===
            new MaintenanceSchedule { Id = 48, VehicleId = 33, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2024, 6, 15), LastCompletedMileage = 12000, NextDueMileage = 19500, NextDueDate = new DateTime(2024, 12, 12) }, // OVERDUE
            new MaintenanceSchedule { Id = 49, VehicleId = 33, MaintenanceType = MaintenanceType.EVBatteryDiagnostic, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 12, 6), LastCompletedMileage = 16000, NextDueMileage = null, NextDueDate = new DateTime(2026, 12, 6) },
            new MaintenanceSchedule { Id = 50, VehicleId = 33, MaintenanceType = MaintenanceType.BrakeInspection, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2024, 10, 5), LastCompletedMileage = 14000, NextDueMileage = null, NextDueDate = new DateTime(2025, 10, 5) }, // OVERDUE

            // === Vehicle 34 - EV ===
            new MaintenanceSchedule { Id = 51, VehicleId = 34, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2024, 10, 12), LastCompletedMileage = 5000, NextDueMileage = 12500, NextDueDate = new DateTime(2025, 4, 10) }, // OVERDUE
            new MaintenanceSchedule { Id = 52, VehicleId = 34, MaintenanceType = MaintenanceType.EVBatteryDiagnostic, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = null, LastCompletedMileage = null, NextDueMileage = null, NextDueDate = new DateTime(2025, 3, 20) }, // OVERDUE - never done

            // === Vehicle 35 - EV ===
            new MaintenanceSchedule { Id = 53, VehicleId = 35, MaintenanceType = MaintenanceType.TireRotation, IntervalMiles = 7500, IntervalDays = 180, LastCompletedDate = new DateTime(2025, 10, 15), LastCompletedMileage = 10000, NextDueMileage = 17500, NextDueDate = new DateTime(2026, 4, 13) },
            new MaintenanceSchedule { Id = 54, VehicleId = 35, MaintenanceType = MaintenanceType.EVBatteryDiagnostic, IntervalMiles = null, IntervalDays = 365, LastCompletedDate = new DateTime(2025, 5, 8), LastCompletedMileage = 8000, NextDueMileage = null, NextDueDate = new DateTime(2026, 5, 8) },
        };
        context.AddRange(maintenanceSchedules);
        context.SaveChanges();
    }
}
