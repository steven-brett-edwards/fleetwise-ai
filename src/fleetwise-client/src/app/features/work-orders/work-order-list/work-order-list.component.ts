import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { WorkOrderService } from '../../../core/services/work-order.service';
import { WorkOrder } from '../../../core/models/work-order.model';

@Component({
  selector: 'app-work-order-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatTableModule, MatSelectModule, MatFormFieldModule,
    MatIconModule, MatButtonModule, MatProgressSpinnerModule,
  ],
  templateUrl: './work-order-list.component.html',
  styleUrl: './work-order-list.component.scss',
})
export class WorkOrderListComponent implements OnInit {
  private workOrderService = inject(WorkOrderService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = true;
  workOrders: WorkOrder[] = [];
  displayedColumns = ['workOrderNumber', 'vehicle', 'status', 'priority', 'description', 'requestedDate'];

  statusFilter = '';
  statusOptions = ['Open', 'InProgress', 'Completed', 'Cancelled'];

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.statusFilter = params['status'] || '';
      this.loadWorkOrders();
    });
  }

  loadWorkOrders(): void {
    this.loading = true;
    this.workOrderService.getAll(this.statusFilter || undefined).subscribe({
      next: workOrders => {
        this.workOrders = workOrders;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  onFilterChange(): void {
    this.router.navigate([], {
      queryParams: { status: this.statusFilter || null },
      queryParamsHandling: 'merge',
    });
  }

  clearFilters(): void {
    this.statusFilter = '';
    this.router.navigate([], { queryParams: {} });
  }

  onRowClick(wo: WorkOrder): void {
    this.router.navigate(['/work-orders', wo.id]);
  }

  getVehicleDescription(wo: WorkOrder): string {
    if (wo.vehicle) {
      return `${wo.vehicle.assetNumber} — ${wo.vehicle.year} ${wo.vehicle.make} ${wo.vehicle.model}`;
    }
    return `Vehicle #${wo.vehicleId}`;
  }
}
