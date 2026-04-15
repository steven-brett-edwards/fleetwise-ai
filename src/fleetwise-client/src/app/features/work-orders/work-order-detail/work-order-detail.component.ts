import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { WorkOrderService } from '../../../core/services/work-order.service';
import { WorkOrder } from '../../../core/models/work-order.model';

@Component({
  selector: 'app-work-order-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule,
  ],
  templateUrl: './work-order-detail.component.html',
  styleUrl: './work-order-detail.component.scss',
})
export class WorkOrderDetailComponent implements OnInit {
  private workOrderService = inject(WorkOrderService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = true;
  workOrder: WorkOrder | null = null;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.workOrderService.getById(id).subscribe({
      next: wo => {
        this.workOrder = wo;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/work-orders']);
  }

  goToVehicle(): void {
    if (this.workOrder) {
      this.router.navigate(['/vehicles', this.workOrder.vehicleId]);
    }
  }
}
