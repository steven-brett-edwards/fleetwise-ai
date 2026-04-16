export interface Vehicle {
    id: number;
    assetNumber: string;
    vin: string;
    year: number;
    make: string;
    model: string;
    fuelType: string;
    status: string;
    department: string;
    assignedDriver: string | null;
    currentMileage: number;
    acquisitionDate: string;
    acquisitionCost: number;
    licensePlate: string;
    location: string;
    notes: string | null;
}

export interface FleetSummary {
    totalVehicles: number;
    byStatus: Record<string, number>;
    byFuelType: Record<string, number>;
    byDepartment: Record<string, number>;
}

export interface VehicleMaintenanceCost {
    vehicleId: number;
    assetNumber: string;
    year: number;
    make: string;
    model: string;
    totalMaintenanceCost: number;
    recordCount: number;
}
