export interface MaintenanceRecord {
  id: number;
  vehicleId: number;
  workOrderId: number | null;
  maintenanceType: string;
  performedDate: string;
  mileageAtService: number;
  description: string;
  cost: number;
  technicianName: string;
}

export interface MaintenanceSchedule {
  id: number;
  vehicleId: number;
  vehicleAssetNumber: string;
  vehicleDescription: string;
  maintenanceType: string;
  nextDueDate: string | null;
  nextDueMileage: number | null;
  currentMileage: number;
  lastCompletedDate: string | null;
  lastCompletedMileage: number | null;
}
