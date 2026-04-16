import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { BehaviorSubject, of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { VehicleListComponent } from '../../../../app/features/vehicles/vehicle-list/vehicle-list.component';
import { VehicleService } from '../../../../app/core/services/vehicle.service';
import { createMockVehicle } from '../../../helpers/mock-data.factory';

describe('VehicleListComponent', () => {
    let component: VehicleListComponent;
    let mockVehicleService: jasmine.SpyObj<VehicleService>;
    let mockRouter: jasmine.SpyObj<Router>;
    let queryParamsSubject: BehaviorSubject<Params>;

    beforeEach(() => {
        mockVehicleService = jasmine.createSpyObj('VehicleService', ['getAll']);
        mockRouter = jasmine.createSpyObj('Router', ['navigate']);
        queryParamsSubject = new BehaviorSubject<Params>({});

        mockVehicleService.getAll.and.returnValue(of([]));

        TestBed.configureTestingModule({
            imports: [VehicleListComponent],
            providers: [
                { provide: VehicleService, useValue: mockVehicleService },
                { provide: Router, useValue: mockRouter },
                {
                    provide: ActivatedRoute,
                    useValue: { queryParams: queryParamsSubject.asObservable() },
                },
            ],
            schemas: [NO_ERRORS_SCHEMA],
        });

        const fixture = TestBed.createComponent(VehicleListComponent);
        component = fixture.componentInstance;
    });

    it('ngOnInit_WhenNoQueryParams_SetsAllFiltersToEmpty', () => {
        // Act
        component.ngOnInit();

        // Result
        expect(component.statusFilter).toBe('');
        expect(component.departmentFilter).toBe('');
        expect(component.fuelTypeFilter).toBe('');
    });

    it('ngOnInit_WhenQueryParamsExist_SetsFiltersFromParams', () => {
        // Setup
        queryParamsSubject.next({ status: 'Active', department: 'Operations', fuelType: 'Diesel' });

        // Act
        component.ngOnInit();

        // Result
        expect(component.statusFilter).toBe('Active');
        expect(component.departmentFilter).toBe('Operations');
        expect(component.fuelTypeFilter).toBe('Diesel');
    });

    it('ngOnInit_WhenInitialized_CallsLoadVehicles', () => {
        // Act
        component.ngOnInit();

        // Result
        expect(mockVehicleService.getAll).toHaveBeenCalled();
    });

    it('loadVehicles_WhenNoFilters_CallsServiceWithEmptyObject', () => {
        // Setup
        component.statusFilter = '';
        component.departmentFilter = '';
        component.fuelTypeFilter = '';

        // Act
        component.loadVehicles();

        // Result
        expect(mockVehicleService.getAll).toHaveBeenCalledWith({});
    });

    it('loadVehicles_WhenAllFiltersSet_CallsServiceWithAllFilters', () => {
        // Setup
        component.statusFilter = 'Active';
        component.departmentFilter = 'Operations';
        component.fuelTypeFilter = 'Diesel';

        // Act
        component.loadVehicles();

        // Result
        expect(mockVehicleService.getAll).toHaveBeenCalledWith({
            status: 'Active',
            department: 'Operations',
            fuelType: 'Diesel',
        });
    });

    it('loadVehicles_WhenPartialFilters_CallsServiceWithOnlySetFilters', () => {
        // Setup
        component.statusFilter = 'InShop';
        component.departmentFilter = '';
        component.fuelTypeFilter = '';

        // Act
        component.loadVehicles();

        // Result
        expect(mockVehicleService.getAll).toHaveBeenCalledWith({ status: 'InShop' });
    });

    it('loadVehicles_WhenServiceReturns_SetsVehiclesAndLoading', () => {
        // Setup
        const expectedActiveVehicle = createMockVehicle({ id: 1, assetNumber: 'V-001' });
        const expectedInShopVehicle = createMockVehicle({ id: 2, assetNumber: 'V-002' });
        mockVehicleService.getAll.and.returnValue(
            of([expectedActiveVehicle, expectedInShopVehicle])
        );

        // Act
        component.loadVehicles();

        // Result
        expect(component.vehicles.length).toBe(2);
        // First vehicle
        expect(component.vehicles[0].assetNumber).toBe('V-001');
        // Second vehicle
        expect(component.vehicles[1].assetNumber).toBe('V-002');
        expect(component.loading).toBeFalse();
    });

    it('loadVehicles_WhenServiceErrors_SetsLoadingFalse', () => {
        // Setup
        mockVehicleService.getAll.and.returnValue(throwError(() => new Error('API error')));

        // Act
        component.loadVehicles();

        // Result
        expect(component.loading).toBeFalse();
    });

    it('extractFilterOptions_WhenNoStatusFilter_ExtractsSortedStatusOptions', () => {
        // Setup
        component.statusFilter = '';
        const vehicleWithActiveStatus = createMockVehicle({
            status: 'Active',
            department: 'Ops',
            fuelType: 'Gas',
        });
        const vehicleWithInShopStatus = createMockVehicle({
            status: 'InShop',
            department: 'Ops',
            fuelType: 'Gas',
        });
        mockVehicleService.getAll.and.returnValue(
            of([vehicleWithInShopStatus, vehicleWithActiveStatus])
        );

        // Act
        component.loadVehicles();

        // Result
        expect(component.statusOptions).toEqual(['Active', 'InShop']);
    });

    it('extractFilterOptions_WhenStatusFilterActive_DoesNotOverrideStatusOptions', () => {
        // Setup
        component.statusFilter = 'Active';
        const originalStatusOptions = [...component.statusOptions];
        mockVehicleService.getAll.and.returnValue(of([createMockVehicle({ status: 'Active' })]));

        // Act
        component.loadVehicles();

        // Result
        expect(component.statusOptions).toEqual(originalStatusOptions);
    });

    it('extractFilterOptions_WhenCalled_ExtractsSortedDepartmentOptions', () => {
        // Setup
        const vehicleInMaintenance = createMockVehicle({ department: 'Maintenance' });
        const vehicleInAdmin = createMockVehicle({ department: 'Administration' });
        const vehicleInMaintenanceDuplicate = createMockVehicle({ department: 'Maintenance' });
        mockVehicleService.getAll.and.returnValue(
            of([vehicleInMaintenance, vehicleInAdmin, vehicleInMaintenanceDuplicate])
        );

        // Act
        component.loadVehicles();

        // Result
        expect(component.departmentOptions).toEqual(['Administration', 'Maintenance']);
    });

    it('extractFilterOptions_WhenCalled_ExtractsSortedFuelTypeOptions', () => {
        // Setup
        const dieselVehicle = createMockVehicle({ fuelType: 'Diesel' });
        const electricVehicle = createMockVehicle({ fuelType: 'Electric' });
        const anotherDieselVehicle = createMockVehicle({ fuelType: 'Diesel' });
        mockVehicleService.getAll.and.returnValue(
            of([dieselVehicle, electricVehicle, anotherDieselVehicle])
        );

        // Act
        component.loadVehicles();

        // Result
        expect(component.fuelTypeOptions).toEqual(['Diesel', 'Electric']);
    });

    it('onFilterChange_WhenCalled_NavigatesWithQueryParams', () => {
        // Setup
        component.statusFilter = 'Active';
        component.departmentFilter = '';
        component.fuelTypeFilter = 'Diesel';

        // Act
        component.onFilterChange();

        // Result
        expect(mockRouter.navigate).toHaveBeenCalledWith([], {
            queryParams: { status: 'Active', department: null, fuelType: 'Diesel' },
            queryParamsHandling: 'merge',
        });
    });

    it('onFilterChange_WhenAllFiltersEmpty_NavigatesWithAllNullParams', () => {
        // Setup
        component.statusFilter = '';
        component.departmentFilter = '';
        component.fuelTypeFilter = '';

        // Act
        component.onFilterChange();

        // Result
        expect(mockRouter.navigate).toHaveBeenCalledWith([], {
            queryParams: { status: null, department: null, fuelType: null },
            queryParamsHandling: 'merge',
        });
    });

    it('clearFilters_WhenCalled_ResetsAllFiltersAndNavigates', () => {
        // Setup
        component.statusFilter = 'Active';
        component.departmentFilter = 'Operations';
        component.fuelTypeFilter = 'Diesel';

        // Act
        component.clearFilters();

        // Result
        expect(component.statusFilter).toBe('');
        expect(component.departmentFilter).toBe('');
        expect(component.fuelTypeFilter).toBe('');
        expect(mockRouter.navigate).toHaveBeenCalledWith([], { queryParams: {} });
    });

    it('onRowClick_WhenCalled_NavigatesToVehicleDetail', () => {
        // Setup
        const clickedVehicle = createMockVehicle({ id: 1 });

        // Act
        component.onRowClick(clickedVehicle);

        // Result
        expect(mockRouter.navigate).toHaveBeenCalledWith(['/vehicles', 1]);
    });

    it('getDescription_WhenCalled_ReturnsFormattedYearMakeModel', () => {
        // Setup
        const testVehicle = createMockVehicle({ year: 2022, make: 'Ford', model: 'F-150' });

        // Act
        const actualDescription = component.getDescription(testVehicle);

        // Result
        expect(actualDescription).toBe('2022 Ford F-150');
    });
});
