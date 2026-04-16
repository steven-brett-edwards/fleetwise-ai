import { Vehicle, FleetSummary } from '../../app/core/models/vehicle.model';
import { WorkOrder } from '../../app/core/models/work-order.model';
import { MaintenanceRecord, MaintenanceSchedule } from '../../app/core/models/maintenance.model';
import { ChatMessage } from '../../app/core/models/chat-message.model';

export function createMockVehicle(overrides?: Partial<Vehicle>): Vehicle {
    return {
        id: 1,
        assetNumber: '2022-001',
        vin: '1HGBH41JXMN109186',
        year: 2022,
        make: 'Ford',
        model: 'F-150',
        fuelType: 'Gasoline',
        status: 'Active',
        department: 'Operations',
        assignedDriver: 'John Smith',
        currentMileage: 45000,
        acquisitionDate: '2022-01-15',
        acquisitionCost: 35000,
        licensePlate: 'ABC-1234',
        location: 'Main Yard',
        notes: null,
        ...overrides,
    };
}

export function createMockWorkOrder(overrides?: Partial<WorkOrder>): WorkOrder {
    return {
        id: 1,
        workOrderNumber: 'WO-2024-001',
        vehicleId: 1,
        status: 'Open',
        priority: 'High',
        description: 'Brake pad replacement',
        requestedDate: '2024-03-15',
        completedDate: null,
        assignedTechnician: 'Mike Johnson',
        laborHours: null,
        totalCost: null,
        notes: null,
        ...overrides,
    };
}

export function createMockMaintenanceRecord(
    overrides?: Partial<MaintenanceRecord>
): MaintenanceRecord {
    return {
        id: 1,
        vehicleId: 1,
        workOrderId: 1,
        maintenanceType: 'OilChange',
        performedDate: '2024-02-10',
        mileageAtService: 43000,
        description: 'Regular oil change',
        cost: 75.5,
        technicianName: 'Mike Johnson',
        ...overrides,
    };
}

export function createMockMaintenanceSchedule(
    overrides?: Partial<MaintenanceSchedule>
): MaintenanceSchedule {
    return {
        id: 1,
        vehicleId: 1,
        vehicleAssetNumber: '2022-001',
        vehicleDescription: '2022 Ford F-150',
        maintenanceType: 'OilChange',
        nextDueDate: '2024-06-15',
        nextDueMileage: 48000,
        currentMileage: 45000,
        lastCompletedDate: '2024-02-10',
        lastCompletedMileage: 43000,
        ...overrides,
    };
}

export function createMockFleetSummary(overrides?: Partial<FleetSummary>): FleetSummary {
    return {
        totalVehicles: 35,
        byStatus: { Active: 29, InShop: 4, OutOfService: 2 },
        byFuelType: { Gasoline: 20, Diesel: 10, Electric: 5 },
        byDepartment: { Operations: 15, Maintenance: 10, Administration: 10 },
        ...overrides,
    };
}

export function createMockChatMessage(overrides?: Partial<ChatMessage>): ChatMessage {
    return {
        role: 'user',
        content: 'How many vehicles are in the fleet?',
        timestamp: new Date('2024-03-15T10:00:00'),
        ...overrides,
    };
}

export function createMockSSEResponse(chunks: string[]): Response {
    const encoder = new TextEncoder();
    const stream = new ReadableStream({
        start(controller) {
            for (const chunk of chunks) {
                controller.enqueue(encoder.encode(chunk));
            }
            controller.close();
        },
    });
    return new Response(stream);
}
