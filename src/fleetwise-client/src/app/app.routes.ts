import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'chat', loadComponent: () => import('./features/chat/chat.component').then(m => m.ChatComponent) },
      { path: 'vehicles', loadComponent: () => import('./features/vehicles/vehicle-list/vehicle-list.component').then(m => m.VehicleListComponent) },
      { path: 'vehicles/:id', loadComponent: () => import('./features/vehicles/vehicle-detail/vehicle-detail.component').then(m => m.VehicleDetailComponent) },
      { path: 'work-orders', loadComponent: () => import('./features/work-orders/work-order-list/work-order-list.component').then(m => m.WorkOrderListComponent) },
      { path: 'work-orders/:id', loadComponent: () => import('./features/work-orders/work-order-detail/work-order-detail.component').then(m => m.WorkOrderDetailComponent) },
    ],
  },
  { path: '**', redirectTo: '' },
];
