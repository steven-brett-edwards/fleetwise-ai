import { Vehicle } from './vehicle.model';

export interface WorkOrder {
  id: number;
  workOrderNumber: string;
  vehicleId: number;
  status: string;
  priority: string;
  description: string;
  requestedDate: string;
  completedDate: string | null;
  assignedTechnician: string | null;
  laborHours: number | null;
  totalCost: number | null;
  notes: string | null;
  vehicle?: Vehicle;
}
