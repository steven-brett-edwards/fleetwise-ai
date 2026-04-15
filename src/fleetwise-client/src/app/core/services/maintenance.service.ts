import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { MaintenanceSchedule } from '../models/maintenance.model';

@Injectable({ providedIn: 'root' })
export class MaintenanceService {
  private api = inject(ApiService);


  getOverdue(): Observable<MaintenanceSchedule[]> {
    return this.api.get<MaintenanceSchedule[]>('/maintenance/overdue');
  }

  getUpcoming(days = 30, miles = 5000): Observable<MaintenanceSchedule[]> {
    return this.api.get<MaintenanceSchedule[]>('/maintenance/upcoming', {
      days: days.toString(),
      miles: miles.toString()
    });
  }
}
