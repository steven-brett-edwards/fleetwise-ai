import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { WorkOrderService } from '../../../app/core/services/work-order.service';
import { ApiService } from '../../../app/core/services/api.service';
import { createMockWorkOrder } from '../../helpers/mock-data.factory';

describe('WorkOrderService', () => {
  let service: WorkOrderService;
  let mockApiService: jasmine.SpyObj<ApiService>;

  beforeEach(() => {
    mockApiService = jasmine.createSpyObj('ApiService', ['get', 'post']);

    TestBed.configureTestingModule({
      providers: [
        WorkOrderService,
        { provide: ApiService, useValue: mockApiService },
      ],
    });

    service = TestBed.inject(WorkOrderService);
  });

  it('getAll_WithNoStatus_CallsApiGetWithNoParams', () => {
    // Setup
    mockApiService.get.and.returnValue(of([]));

    // Act
    service.getAll().subscribe();

    // Result
    expect(mockApiService.get).toHaveBeenCalledWith('/work-orders', {});
  });

  it('getAll_WithStatus_CallsApiGetWithStatusParam', () => {
    // Setup
    mockApiService.get.and.returnValue(of([]));

    // Act
    service.getAll('Open').subscribe();

    // Result
    expect(mockApiService.get).toHaveBeenCalledWith('/work-orders', { status: 'Open' });
  });

  it('getAll_WhenCalled_ReturnsWorkOrders', () => {
    // Setup
    const expectedOpenWorkOrder = createMockWorkOrder({ status: 'Open', workOrderNumber: 'WO-001' });
    const expectedCompletedWorkOrder = createMockWorkOrder({ id: 2, status: 'Completed', workOrderNumber: 'WO-002' });
    mockApiService.get.and.returnValue(of([expectedOpenWorkOrder, expectedCompletedWorkOrder]));
    let actualWorkOrders: any[] = [];

    // Act
    service.getAll().subscribe(wo => actualWorkOrders = wo);

    // Result
    expect(actualWorkOrders.length).toBe(2);
    // First work order
    expect(actualWorkOrders[0].workOrderNumber).toBe('WO-001');
    expect(actualWorkOrders[0].status).toBe('Open');
    // Second work order
    expect(actualWorkOrders[1].workOrderNumber).toBe('WO-002');
    expect(actualWorkOrders[1].status).toBe('Completed');
  });

  it('getById_WithId_CallsApiGetWithCorrectPath', () => {
    // Setup
    mockApiService.get.and.returnValue(of(createMockWorkOrder()));

    // Act
    service.getById(42).subscribe();

    // Result
    expect(mockApiService.get).toHaveBeenCalledWith('/work-orders/42');
  });

  it('getById_WhenCalled_ReturnsSingleWorkOrder', () => {
    // Setup
    const expectedWorkOrder = createMockWorkOrder({ id: 42, workOrderNumber: 'WO-042' });
    mockApiService.get.and.returnValue(of(expectedWorkOrder));
    let actualWorkOrder: any;

    // Act
    service.getById(42).subscribe(wo => actualWorkOrder = wo);

    // Result
    expect(actualWorkOrder.id).toBe(42);
    expect(actualWorkOrder.workOrderNumber).toBe('WO-042');
  });
});
