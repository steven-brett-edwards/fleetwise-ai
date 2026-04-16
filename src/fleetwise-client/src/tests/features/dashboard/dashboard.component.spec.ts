import { TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of, throwError } from 'rxjs';
import { DashboardComponent } from '../../../app/features/dashboard/dashboard.component';
import { VehicleService } from '../../../app/core/services/vehicle.service';
import { MaintenanceService } from '../../../app/core/services/maintenance.service';
import { WorkOrderService } from '../../../app/core/services/work-order.service';
import {
    createMockFleetSummary,
    createMockMaintenanceSchedule,
    createMockWorkOrder,
} from '../../helpers/mock-data.factory';

describe('DashboardComponent', () => {
    let component: DashboardComponent;
    let mockVehicleService: jasmine.SpyObj<VehicleService>;
    let mockMaintenanceService: jasmine.SpyObj<MaintenanceService>;
    let mockWorkOrderService: jasmine.SpyObj<WorkOrderService>;

    beforeEach(() => {
        mockVehicleService = jasmine.createSpyObj('VehicleService', ['getSummary']);
        mockMaintenanceService = jasmine.createSpyObj('MaintenanceService', [
            'getOverdue',
            'getUpcoming',
        ]);
        mockWorkOrderService = jasmine.createSpyObj('WorkOrderService', ['getAll']);

        TestBed.configureTestingModule({
            imports: [DashboardComponent],
            providers: [
                { provide: VehicleService, useValue: mockVehicleService },
                { provide: MaintenanceService, useValue: mockMaintenanceService },
                { provide: WorkOrderService, useValue: mockWorkOrderService },
            ],
            schemas: [NO_ERRORS_SCHEMA],
        });
    });

    function setupDefaultMocks(): void {
        mockVehicleService.getSummary.and.returnValue(of(createMockFleetSummary()));
        mockMaintenanceService.getOverdue.and.returnValue(of([]));
        mockMaintenanceService.getUpcoming.and.returnValue(of([]));
        mockWorkOrderService.getAll.and.returnValue(of([]));
    }

    function createComponent(): DashboardComponent {
        const fixture = TestBed.createComponent(DashboardComponent);
        return fixture.componentInstance;
    }

    it('ngOnInit_WhenInitialized_CallsVehicleServiceGetSummary', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(mockVehicleService.getSummary).toHaveBeenCalledOnceWith();
    });

    it('ngOnInit_WhenInitialized_CallsMaintenanceServiceGetOverdue', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(mockMaintenanceService.getOverdue).toHaveBeenCalledOnceWith();
    });

    it('ngOnInit_WhenInitialized_CallsMaintenanceServiceGetUpcoming', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(mockMaintenanceService.getUpcoming).toHaveBeenCalledOnceWith();
    });

    it('ngOnInit_WhenInitialized_CallsWorkOrderServiceGetAllWithOpenStatus', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(mockWorkOrderService.getAll).toHaveBeenCalledOnceWith('Open');
    });

    it('ngOnInit_WhenForkJoinCompletes_SetsSummary', () => {
        // Setup
        const expectedSummary = createMockFleetSummary({ totalVehicles: 35 });
        mockVehicleService.getSummary.and.returnValue(of(expectedSummary));
        mockMaintenanceService.getOverdue.and.returnValue(of([]));
        mockMaintenanceService.getUpcoming.and.returnValue(of([]));
        mockWorkOrderService.getAll.and.returnValue(of([]));
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(component.summary!.totalVehicles).toBe(35);
    });

    it('ngOnInit_WhenForkJoinCompletes_SetsOverdueSchedules', () => {
        // Setup
        const expectedBrakeSchedule = createMockMaintenanceSchedule({
            id: 1,
            maintenanceType: 'BrakeInspection',
        });
        const expectedOilSchedule = createMockMaintenanceSchedule({
            id: 2,
            maintenanceType: 'OilChange',
        });
        mockVehicleService.getSummary.and.returnValue(of(createMockFleetSummary()));
        mockMaintenanceService.getOverdue.and.returnValue(
            of([expectedBrakeSchedule, expectedOilSchedule])
        );
        mockMaintenanceService.getUpcoming.and.returnValue(of([]));
        mockWorkOrderService.getAll.and.returnValue(of([]));
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(component.overdueSchedules.length).toBe(2);
        // First schedule
        expect(component.overdueSchedules[0].maintenanceType).toBe('BrakeInspection');
        // Second schedule
        expect(component.overdueSchedules[1].maintenanceType).toBe('OilChange');
    });

    it('ngOnInit_WhenForkJoinCompletes_SetsUpcomingSchedules', () => {
        // Setup
        const expectedTireSchedule = createMockMaintenanceSchedule({
            maintenanceType: 'TireRotation',
        });
        mockVehicleService.getSummary.and.returnValue(of(createMockFleetSummary()));
        mockMaintenanceService.getOverdue.and.returnValue(of([]));
        mockMaintenanceService.getUpcoming.and.returnValue(of([expectedTireSchedule]));
        mockWorkOrderService.getAll.and.returnValue(of([]));
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(component.upcomingSchedules.length).toBe(1);
        expect(component.upcomingSchedules[0].maintenanceType).toBe('TireRotation');
    });

    it('ngOnInit_WhenForkJoinCompletes_SetsOpenWorkOrderCount', () => {
        // Setup
        const openWorkOrder1 = createMockWorkOrder({ id: 1 });
        const openWorkOrder2 = createMockWorkOrder({ id: 2 });
        const openWorkOrder3 = createMockWorkOrder({ id: 3 });
        mockVehicleService.getSummary.and.returnValue(of(createMockFleetSummary()));
        mockMaintenanceService.getOverdue.and.returnValue(of([]));
        mockMaintenanceService.getUpcoming.and.returnValue(of([]));
        mockWorkOrderService.getAll.and.returnValue(
            of([openWorkOrder1, openWorkOrder2, openWorkOrder3])
        );
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(component.openWorkOrderCount).toBe(3);
    });

    it('ngOnInit_WhenForkJoinCompletes_SetsLoadingToFalse', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(component.loading).toBeFalse();
    });

    it('ngOnInit_WhenForkJoinErrors_SetsLoadingToFalse', () => {
        // Setup
        mockVehicleService.getSummary.and.returnValue(throwError(() => new Error('API error')));
        mockMaintenanceService.getOverdue.and.returnValue(of([]));
        mockMaintenanceService.getUpcoming.and.returnValue(of([]));
        mockWorkOrderService.getAll.and.returnValue(of([]));
        component = createComponent();

        // Act
        component.ngOnInit();

        // Result
        expect(component.loading).toBeFalse();
    });

    it('getStatusCount_WhenStatusExistsInSummary_ReturnsCount', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();
        component.ngOnInit();

        // Act
        const actualActiveCount = component.getStatusCount('Active');

        // Result
        expect(actualActiveCount).toBe(29);
    });

    it('getStatusCount_WhenStatusDoesNotExist_ReturnsZero', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();
        component.ngOnInit();

        // Act
        const actualNonexistentCount = component.getStatusCount('Nonexistent');

        // Result
        expect(actualNonexistentCount).toBe(0);
    });

    it('getStatusCount_WhenSummaryIsNull_ReturnsZero', () => {
        // Setup
        setupDefaultMocks();
        component = createComponent();
        component.summary = null;

        // Act
        const actualCount = component.getStatusCount('Active');

        // Result
        expect(actualCount).toBe(0);
    });
});
