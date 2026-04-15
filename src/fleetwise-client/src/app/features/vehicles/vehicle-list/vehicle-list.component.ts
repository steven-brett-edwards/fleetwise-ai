import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { VehicleService } from '../../../core/services/vehicle.service';
import { Vehicle } from '../../../core/models/vehicle.model';

@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatTableModule, MatSortModule, MatSelectModule, MatFormFieldModule,
    MatIconModule, MatButtonModule, MatProgressSpinnerModule,
  ],
  templateUrl: './vehicle-list.component.html',
  styleUrl: './vehicle-list.component.scss',
})
export class VehicleListComponent implements OnInit {
  private vehicleService = inject(VehicleService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = true;
  vehicles: Vehicle[] = [];
  displayedColumns = ['assetNumber', 'description', 'status', 'department', 'fuelType', 'currentMileage', 'location'];

  statusFilter = '';
  departmentFilter = '';
  fuelTypeFilter = '';

  statusOptions = ['Active', 'InShop', 'OutOfService', 'Disposed'];
  departmentOptions: string[] = [];
  fuelTypeOptions: string[] = [];

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.statusFilter = params['status'] || '';
      this.departmentFilter = params['department'] || '';
      this.fuelTypeFilter = params['fuelType'] || '';
      this.loadVehicles();
    });
  }

  loadVehicles(): void {
    this.loading = true;
    const filters: Record<string, string> = {};
    if (this.statusFilter) filters['status'] = this.statusFilter;
    if (this.departmentFilter) filters['department'] = this.departmentFilter;
    if (this.fuelTypeFilter) filters['fuelType'] = this.fuelTypeFilter;

    this.vehicleService.getAll(filters).subscribe({
      next: vehicles => {
        this.vehicles = vehicles;
        this.extractFilterOptions(vehicles);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  onFilterChange(): void {
    const queryParams: Record<string, string | null> = {
      status: this.statusFilter || null,
      department: this.departmentFilter || null,
      fuelType: this.fuelTypeFilter || null,
    };
    this.router.navigate([], { queryParams, queryParamsHandling: 'merge' });
  }

  clearFilters(): void {
    this.statusFilter = '';
    this.departmentFilter = '';
    this.fuelTypeFilter = '';
    this.router.navigate([], { queryParams: {} });
  }

  onRowClick(vehicle: Vehicle): void {
    this.router.navigate(['/vehicles', vehicle.id]);
  }

  getDescription(v: Vehicle): string {
    return `${v.year} ${v.make} ${v.model}`;
  }

  private extractFilterOptions(vehicles: Vehicle[]): void {
    if (!this.statusFilter) {
      this.statusOptions = [...new Set(vehicles.map(v => v.status))].sort();
    }
    this.departmentOptions = [...new Set(vehicles.map(v => v.department))].sort();
    this.fuelTypeOptions = [...new Set(vehicles.map(v => v.fuelType))].sort();
  }
}
