import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { WorkOrder } from '../models/work-order.model';

@Injectable({ providedIn: 'root' })
export class WorkOrderService {
    private api = inject(ApiService);

    getAll(status?: string): Observable<WorkOrder[]> {
        const params: Record<string, string> = {};
        if (status) params['status'] = status;
        return this.api.get<WorkOrder[]>('/work-orders', params);
    }

    getById(id: number): Observable<WorkOrder> {
        return this.api.get<WorkOrder>(`/work-orders/${id}`);
    }
}
