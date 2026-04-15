import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { forkJoin } from 'rxjs';
import { VehicleService } from '../../../core/services/vehicle.service';
import { Vehicle } from '../../../core/models/vehicle.model';
import { MaintenanceRecord } from '../../../core/models/maintenance.model';
import { WorkOrder } from '../../../core/models/work-order.model';

@Component({
  selector: 'app-vehicle-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule, MatTabsModule, MatTableModule,
    MatIconModule, MatButtonModule, MatProgressSpinnerModule,
  ],
  templateUrl: './vehicle-detail.component.html',
  styleUrl: './vehicle-detail.component.scss',
})
export class VehicleDetailComponent implements OnInit {
  private vehicleService = inject(VehicleService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = true;
  vehicle: Vehicle | null = null;
  maintenanceRecords: MaintenanceRecord[] = [];
  workOrders: WorkOrder[] = [];

  maintenanceColumns = ['maintenanceType', 'performedDate', 'mileageAtService', 'cost', 'technicianName'];
  workOrderColumns = ['workOrderNumber', 'status', 'priority', 'description', 'requestedDate'];

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    forkJoin({
      vehicle: this.vehicleService.getById(id),
      maintenance: this.vehicleService.getMaintenanceHistory(id),
      workOrders: this.vehicleService.getWorkOrders(id),
    }).subscribe({
      next: ({ vehicle, maintenance, workOrders }) => {
        this.vehicle = vehicle;
        this.maintenanceRecords = maintenance;
        this.workOrders = workOrders;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/vehicles']);
  }

  getDescription(): string {
    const v = this.vehicle;
    return v ? `${v.year} ${v.make} ${v.model}` : '';
  }

  onWorkOrderClick(wo: WorkOrder): void {
    this.router.navigate(['/work-orders', wo.id]);
  }
}
