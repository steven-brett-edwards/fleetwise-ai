import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, convertToParamMap } from '@angular/router';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { VehicleDetailComponent } from '../../../../app/features/vehicles/vehicle-detail/vehicle-detail.component';
import { VehicleService } from '../../../../app/core/services/vehicle.service';
import { createMockVehicle, createMockMaintenanceRecord, createMockWorkOrder } from '../../../helpers/mock-data.factory';

describe('VehicleDetailComponent', () => {
  let component: VehicleDetailComponent;
  let mockVehicleService: jasmine.SpyObj<VehicleService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockVehicleService = jasmine.createSpyObj('VehicleService', ['getById', 'getMaintenanceHistory', 'getWorkOrders']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [VehicleDetailComponent],
      providers: [
        { provide: VehicleService, useValue: mockVehicleService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: convertToParamMap({ id: '7' }) } } },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    });
  });

  function createComponentWithData(): void {
    mockVehicleService.getById.and.returnValue(of(createMockVehicle({ id: 7 })));
    mockVehicleService.getMaintenanceHistory.and.returnValue(of([]));
    mockVehicleService.getWorkOrders.and.returnValue(of([]));
    const fixture = TestBed.createComponent(VehicleDetailComponent);
    component = fixture.componentInstance;
  }

  it('ngOnInit_WhenRouteParamProvided_CallsGetByIdWithParsedId', () => {
    // Setup
    createComponentWithData();

    // Act
    component.ngOnInit();

    // Result
    expect(mockVehicleService.getById).toHaveBeenCalledWith(7);
  });

  it('ngOnInit_WhenRouteParamProvided_CallsGetMaintenanceHistoryWithId', () => {
    // Setup
    createComponentWithData();

    // Act
    component.ngOnInit();

    // Result
    expect(mockVehicleService.getMaintenanceHistory).toHaveBeenCalledWith(7);
  });

  it('ngOnInit_WhenRouteParamProvided_CallsGetWorkOrdersWithId', () => {
    // Setup
    createComponentWithData();

    // Act
    component.ngOnInit();

    // Result
    expect(mockVehicleService.getWorkOrders).toHaveBeenCalledWith(7);
  });

  it('ngOnInit_WhenForkJoinCompletes_SetsVehicle', () => {
    // Setup
    const expectedVehicle = createMockVehicle({ id: 7, assetNumber: 'V-007' });
    mockVehicleService.getById.and.returnValue(of(expectedVehicle));
    mockVehicleService.getMaintenanceHistory.and.returnValue(of([]));
    mockVehicleService.getWorkOrders.and.returnValue(of([]));
    const fixture = TestBed.createComponent(VehicleDetailComponent);
    component = fixture.componentInstance;

    // Act
    component.ngOnInit();

    // Result
    expect(component.vehicle!.id).toBe(7);
    expect(component.vehicle!.assetNumber).toBe('V-007');
  });

  it('ngOnInit_WhenForkJoinCompletes_SetsMaintenanceRecords', () => {
    // Setup
    const expectedOilChangeRecord = createMockMaintenanceRecord({ id: 1, maintenanceType: 'OilChange' });
    const expectedBrakeRecord = createMockMaintenanceRecord({ id: 2, maintenanceType: 'BrakeInspection' });
    mockVehicleService.getById.and.returnValue(of(createMockVehicle()));
    mockVehicleService.getMaintenanceHistory.and.returnValue(of([expectedOilChangeRecord, expectedBrakeRecord]));
    mockVehicleService.getWorkOrders.and.returnValue(of([]));
    const fixture = TestBed.createComponent(VehicleDetailComponent);
    component = fixture.componentInstance;

    // Act
    component.ngOnInit();

    // Result
    expect(component.maintenanceRecords.length).toBe(2);
    // First record
    expect(component.maintenanceRecords[0].maintenanceType).toBe('OilChange');
    // Second record
    expect(component.maintenanceRecords[1].maintenanceType).toBe('BrakeInspection');
  });

  it('ngOnInit_WhenForkJoinCompletes_SetsWorkOrders', () => {
    // Setup
    const expectedWorkOrder = createMockWorkOrder({ id: 10, workOrderNumber: 'WO-010' });
    mockVehicleService.getById.and.returnValue(of(createMockVehicle()));
    mockVehicleService.getMaintenanceHistory.and.returnValue(of([]));
    mockVehicleService.getWorkOrders.and.returnValue(of([expectedWorkOrder]));
    const fixture = TestBed.createComponent(VehicleDetailComponent);
    component = fixture.componentInstance;

    // Act
    component.ngOnInit();

    // Result
    expect(component.workOrders.length).toBe(1);
    expect(component.workOrders[0].workOrderNumber).toBe('WO-010');
  });

  it('ngOnInit_WhenForkJoinCompletes_SetsLoadingToFalse', () => {
    // Setup
    createComponentWithData();

    // Act
    component.ngOnInit();

    // Result
    expect(component.loading).toBeFalse();
  });

  it('ngOnInit_WhenForkJoinErrors_SetsLoadingToFalse', () => {
    // Setup
    mockVehicleService.getById.and.returnValue(throwError(() => new Error('API error')));
    mockVehicleService.getMaintenanceHistory.and.returnValue(of([]));
    mockVehicleService.getWorkOrders.and.returnValue(of([]));
    const fixture = TestBed.createComponent(VehicleDetailComponent);
    component = fixture.componentInstance;

    // Act
    component.ngOnInit();

    // Result
    expect(component.loading).toBeFalse();
  });

  it('goBack_WhenCalled_NavigatesToVehiclesList', () => {
    // Setup
    createComponentWithData();

    // Act
    component.goBack();

    // Result
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/vehicles']);
  });

  it('getDescription_WhenVehicleExists_ReturnsFormattedDescription', () => {
    // Setup
    createComponentWithData();
    component.ngOnInit();
    component.vehicle = createMockVehicle({ year: 2022, make: 'Ford', model: 'F-150' });

    // Act
    const actualDescription = component.getDescription();

    // Result
    expect(actualDescription).toBe('2022 Ford F-150');
  });

  it('getDescription_WhenVehicleIsNull_ReturnsEmptyString', () => {
    // Setup
    createComponentWithData();
    component.vehicle = null;

    // Act
    const actualDescription = component.getDescription();

    // Result
    expect(actualDescription).toBe('');
  });

  it('onWorkOrderClick_WhenCalled_NavigatesToWorkOrderDetail', () => {
    // Setup
    createComponentWithData();
    const clickedWorkOrder = createMockWorkOrder({ id: 5 });

    // Act
    component.onWorkOrderClick(clickedWorkOrder);

    // Result
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/work-orders', 5]);
  });
});
