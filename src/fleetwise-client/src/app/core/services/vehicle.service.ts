import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Vehicle, FleetSummary } from '../models/vehicle.model';
import { MaintenanceRecord } from '../models/maintenance.model';
import { WorkOrder } from '../models/work-order.model';

@Injectable({ providedIn: 'root' })
export class VehicleService {
  constructor(private api: ApiService) {}

  getAll(filters?: { status?: string; department?: string; fuelType?: string }): Observable<Vehicle[]> {
    const params: Record<string, string> = {};
    if (filters?.status) params['status'] = filters.status;
    if (filters?.department) params['department'] = filters.department;
    if (filters?.fuelType) params['fuelType'] = filters.fuelType;
    return this.api.get<Vehicle[]>('/vehicles', params);
  }

  getById(id: number): Observable<Vehicle> {
    return this.api.get<Vehicle>(`/vehicles/${id}`);
  }

  getMaintenanceHistory(id: number): Observable<MaintenanceRecord[]> {
    return this.api.get<MaintenanceRecord[]>(`/vehicles/${id}/maintenance`);
  }

  getWorkOrders(id: number): Observable<WorkOrder[]> {
    return this.api.get<WorkOrder[]>(`/vehicles/${id}/work-orders`);
  }

  getSummary(): Observable<FleetSummary> {
    return this.api.get<FleetSummary>('/vehicles/summary');
  }
}
