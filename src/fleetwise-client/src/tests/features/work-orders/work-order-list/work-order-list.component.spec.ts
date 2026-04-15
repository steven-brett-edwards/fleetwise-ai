import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { BehaviorSubject, of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { WorkOrderListComponent } from '../../../../app/features/work-orders/work-order-list/work-order-list.component';
import { WorkOrderService } from '../../../../app/core/services/work-order.service';
import { createMockWorkOrder, createMockVehicle } from '../../../helpers/mock-data.factory';

describe('WorkOrderListComponent', () => {
  let component: WorkOrderListComponent;
  let mockWorkOrderService: jasmine.SpyObj<WorkOrderService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let queryParamsSubject: BehaviorSubject<Params>;

  beforeEach(() => {
    mockWorkOrderService = jasmine.createSpyObj('WorkOrderService', ['getAll']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    queryParamsSubject = new BehaviorSubject<Params>({});

    mockWorkOrderService.getAll.and.returnValue(of([]));

    TestBed.configureTestingModule({
      imports: [WorkOrderListComponent],
      providers: [
        { provide: WorkOrderService, useValue: mockWorkOrderService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { queryParams: queryParamsSubject.asObservable() } },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    });

    const fixture = TestBed.createComponent(WorkOrderListComponent);
    component = fixture.componentInstance;
  });

  it('ngOnInit_WhenNoQueryParams_SetsEmptyStatusFilter', () => {
    // Act
    component.ngOnInit();

    // Result
    expect(component.statusFilter).toBe('');
  });

  it('ngOnInit_WhenStatusQueryParamExists_SetsStatusFilter', () => {
    // Setup
    queryParamsSubject.next({ status: 'Open' });

    // Act
    component.ngOnInit();

    // Result
    expect(component.statusFilter).toBe('Open');
  });

  it('ngOnInit_WhenInitialized_CallsLoadWorkOrders', () => {
    // Act
    component.ngOnInit();

    // Result
    expect(mockWorkOrderService.getAll).toHaveBeenCalled();
  });

  it('loadWorkOrders_WhenNoFilter_CallsServiceWithUndefined', () => {
    // Setup
    component.statusFilter = '';

    // Act
    component.loadWorkOrders();

    // Result
    expect(mockWorkOrderService.getAll).toHaveBeenCalledWith(undefined);
  });

  it('loadWorkOrders_WhenFilterSet_CallsServiceWithFilter', () => {
    // Setup
    component.statusFilter = 'Open';

    // Act
    component.loadWorkOrders();

    // Result
    expect(mockWorkOrderService.getAll).toHaveBeenCalledWith('Open');
  });

  it('loadWorkOrders_WhenServiceReturns_SetsWorkOrdersAndLoading', () => {
    // Setup
    const expectedOpenWorkOrder = createMockWorkOrder({ id: 1, workOrderNumber: 'WO-001' });
    const expectedCompletedWorkOrder = createMockWorkOrder({ id: 2, workOrderNumber: 'WO-002' });
    mockWorkOrderService.getAll.and.returnValue(of([expectedOpenWorkOrder, expectedCompletedWorkOrder]));

    // Act
    component.loadWorkOrders();

    // Result
    expect(component.workOrders.length).toBe(2);
    // First work order
    expect(component.workOrders[0].workOrderNumber).toBe('WO-001');
    // Second work order
    expect(component.workOrders[1].workOrderNumber).toBe('WO-002');
    expect(component.loading).toBeFalse();
  });

  it('loadWorkOrders_WhenServiceErrors_SetsLoadingFalse', () => {
    // Setup
    mockWorkOrderService.getAll.and.returnValue(throwError(() => new Error('API error')));

    // Act
    component.loadWorkOrders();

    // Result
    expect(component.loading).toBeFalse();
  });

  it('onFilterChange_WhenCalled_NavigatesWithQueryParams', () => {
    // Setup
    component.statusFilter = 'InProgress';

    // Act
    component.onFilterChange();

    // Result
    expect(mockRouter.navigate).toHaveBeenCalledWith([], {
      queryParams: { status: 'InProgress' },
      queryParamsHandling: 'merge',
    });
  });

  it('onFilterChange_WhenStatusFilterEmpty_NavigatesWithNullStatus', () => {
    // Setup
    component.statusFilter = '';

    // Act
    component.onFilterChange();

    // Result
    expect(mockRouter.navigate).toHaveBeenCalledWith([], {
      queryParams: { status: null },
      queryParamsHandling: 'merge',
    });
  });

  it('clearFilters_WhenCalled_ResetsStatusFilterAndNavigates', () => {
    // Setup
    component.statusFilter = 'Open';

    // Act
    component.clearFilters();

    // Result
    expect(component.statusFilter).toBe('');
    expect(mockRouter.navigate).toHaveBeenCalledWith([], { queryParams: {} });
  });

  it('onRowClick_WhenCalled_NavigatesToWorkOrderDetail', () => {
    // Setup
    const clickedWorkOrder = createMockWorkOrder({ id: 5 });

    // Act
    component.onRowClick(clickedWorkOrder);

    // Result
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/work-orders', 5]);
  });

  it('getVehicleDescription_WhenVehicleExists_ReturnsFullDescription', () => {
    // Setup
    const workOrderWithVehicle = createMockWorkOrder({
      vehicle: createMockVehicle({ assetNumber: 'V-007', year: 2022, make: 'Ford', model: 'F-150' }),
    });

    // Act
    const actualDescription = component.getVehicleDescription(workOrderWithVehicle);

    // Result
    expect(actualDescription).toBe('V-007 — 2022 Ford F-150');
  });

  it('getVehicleDescription_WhenVehicleIsUndefined_ReturnsFallbackWithId', () => {
    // Setup
    const workOrderWithoutVehicle = createMockWorkOrder({ vehicleId: 3 });

    // Act
    const actualDescription = component.getVehicleDescription(workOrderWithoutVehicle);

    // Result
    expect(actualDescription).toBe('Vehicle #3');
  });
});
