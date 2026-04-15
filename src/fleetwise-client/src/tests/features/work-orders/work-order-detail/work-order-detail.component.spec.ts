import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { WorkOrderDetailComponent } from '../../../../app/features/work-orders/work-order-detail/work-order-detail.component';
import { WorkOrderService } from '../../../../app/core/services/work-order.service';
import { createMockWorkOrder } from '../../../helpers/mock-data.factory';

describe('WorkOrderDetailComponent', () => {
  let component: WorkOrderDetailComponent;
  let mockWorkOrderService: jasmine.SpyObj<WorkOrderService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockWorkOrderService = jasmine.createSpyObj('WorkOrderService', ['getById']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [WorkOrderDetailComponent],
      providers: [
        { provide: WorkOrderService, useValue: mockWorkOrderService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({ id: '42' }) } } },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    });
  });

  function createComponent(): void {
    const fixture = TestBed.createComponent(WorkOrderDetailComponent);
    component = fixture.componentInstance;
  }

  it('ngOnInit_WhenRouteParamProvided_CallsGetByIdWithParsedId', () => {
    // Setup
    mockWorkOrderService.getById.and.returnValue(of(createMockWorkOrder({ id: 42 })));

    // Act
    createComponent();
    component.ngOnInit();

    // Result
    expect(mockWorkOrderService.getById).toHaveBeenCalledWith(42);
  });

  it('ngOnInit_WhenServiceReturnsData_SetsWorkOrder', () => {
    // Setup
    const expectedWorkOrder = createMockWorkOrder({ id: 42, workOrderNumber: 'WO-042' });
    mockWorkOrderService.getById.and.returnValue(of(expectedWorkOrder));

    // Act
    createComponent();
    component.ngOnInit();

    // Result
    expect(component.workOrder!.id).toBe(42);
    expect(component.workOrder!.workOrderNumber).toBe('WO-042');
  });

  it('ngOnInit_WhenServiceReturnsData_SetsLoadingToFalse', () => {
    // Setup
    mockWorkOrderService.getById.and.returnValue(of(createMockWorkOrder()));

    // Act
    createComponent();
    component.ngOnInit();

    // Result
    expect(component.loading).toBeFalse();
  });

  it('ngOnInit_WhenServiceErrors_SetsLoadingToFalse', () => {
    // Setup
    mockWorkOrderService.getById.and.returnValue(throwError(() => new Error('API error')));

    // Act
    createComponent();
    component.ngOnInit();

    // Result
    expect(component.loading).toBeFalse();
  });

  it('ngOnInit_WhenServiceErrors_LeavesWorkOrderNull', () => {
    // Setup
    mockWorkOrderService.getById.and.returnValue(throwError(() => new Error('API error')));

    // Act
    createComponent();
    component.ngOnInit();

    // Result
    expect(component.workOrder).toBeNull();
  });

  it('goBack_WhenCalled_NavigatesToWorkOrdersList', () => {
    // Setup
    mockWorkOrderService.getById.and.returnValue(of(createMockWorkOrder()));
    createComponent();

    // Act
    component.goBack();

    // Result
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/work-orders']);
  });

  it('goToVehicle_WhenWorkOrderExists_NavigatesToVehicleDetail', () => {
    // Setup
    mockWorkOrderService.getById.and.returnValue(of(createMockWorkOrder({ vehicleId: 7 })));
    createComponent();
    component.ngOnInit();

    // Act
    component.goToVehicle();

    // Result
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/vehicles', 7]);
  });

  it('goToVehicle_WhenWorkOrderIsNull_DoesNotNavigate', () => {
    // Setup
    mockWorkOrderService.getById.and.returnValue(throwError(() => new Error()));
    createComponent();
    component.ngOnInit();

    // Act
    component.goToVehicle();

    // Result
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  });
});
