import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { forkJoin } from 'rxjs';
import { VehicleService } from '../../core/services/vehicle.service';
import { MaintenanceService } from '../../core/services/maintenance.service';
import { WorkOrderService } from '../../core/services/work-order.service';
import { FleetSummary } from '../../core/models/vehicle.model';
import { MaintenanceSchedule } from '../../core/models/maintenance.model';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [
        CommonModule,
        RouterLink,
        MatCardModule,
        MatTableModule,
        MatIconModule,
        MatProgressSpinnerModule,
    ],
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
    private vehicleService = inject(VehicleService);
    private maintenanceService = inject(MaintenanceService);
    private workOrderService = inject(WorkOrderService);

    loading = true;
    summary: FleetSummary | null = null;
    overdueSchedules: MaintenanceSchedule[] = [];
    upcomingSchedules: MaintenanceSchedule[] = [];
    openWorkOrderCount = 0;

    overdueColumns = ['vehicleAssetNumber', 'vehicleDescription', 'maintenanceType', 'nextDueDate'];
    upcomingColumns = [
        'vehicleAssetNumber',
        'vehicleDescription',
        'maintenanceType',
        'nextDueDate',
        'nextDueMileage',
    ];

    ngOnInit(): void {
        forkJoin({
            summary: this.vehicleService.getSummary(),
            overdue: this.maintenanceService.getOverdue(),
            upcoming: this.maintenanceService.getUpcoming(),
            openWorkOrders: this.workOrderService.getAll('Open'),
        }).subscribe({
            next: ({ summary, overdue, upcoming, openWorkOrders }) => {
                this.summary = summary;
                this.overdueSchedules = overdue;
                this.upcomingSchedules = upcoming;
                this.openWorkOrderCount = openWorkOrders.length;
                this.loading = false;
            },
            error: () => {
                this.loading = false;
            },
        });
    }

    getStatusCount(status: string): number {
        return this.summary?.byStatus[status] ?? 0;
    }
}
