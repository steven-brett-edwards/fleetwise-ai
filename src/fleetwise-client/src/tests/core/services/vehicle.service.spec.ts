import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { VehicleService } from '../../../app/core/services/vehicle.service';
import { ApiService } from '../../../app/core/services/api.service';
import {
    createMockVehicle,
    createMockFleetSummary,
    createMockMaintenanceRecord,
} from '../../helpers/mock-data.factory';
import { Vehicle } from '../../../app/core/models/vehicle.model';
import { MaintenanceRecord } from '../../../app/core/models/maintenance.model';

describe('VehicleService', () => {
    let service: VehicleService;
    let mockApiService: jasmine.SpyObj<ApiService>;

    beforeEach(() => {
        mockApiService = jasmine.createSpyObj('ApiService', ['get', 'post']);

        TestBed.configureTestingModule({
            providers: [VehicleService, { provide: ApiService, useValue: mockApiService }],
        });

        service = TestBed.inject(VehicleService);
    });

    it('getAll_WithNoFilters_CallsApiGetWithEmptyParams', () => {
        // Setup
        mockApiService.get.and.returnValue(of([]));

        // Act
        service.getAll().subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles', {});
    });

    it('getAll_WithAllFilters_CallsApiGetWithAllFilterParams', () => {
        // Setup
        mockApiService.get.and.returnValue(of([]));
        const allFilters = { status: 'Active', department: 'Operations', fuelType: 'Diesel' };

        // Act
        service.getAll(allFilters).subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles', {
            status: 'Active',
            department: 'Operations',
            fuelType: 'Diesel',
        });
    });

    it('getAll_WithPartialFilters_CallsApiGetWithOnlyProvidedParams', () => {
        // Setup
        mockApiService.get.and.returnValue(of([]));
        const statusOnlyFilter = { status: 'InShop' };

        // Act
        service.getAll(statusOnlyFilter).subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles', { status: 'InShop' });
    });

    it('getAll_WithUndefinedFilterValues_OmitsUndefinedParams', () => {
        // Setup
        mockApiService.get.and.returnValue(of([]));
        const filterWithUndefined = {
            status: undefined,
            department: 'Operations',
            fuelType: undefined,
        } as { status?: string; department?: string; fuelType?: string };

        // Act
        service.getAll(filterWithUndefined).subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles', { department: 'Operations' });
    });

    it('getAll_WhenCalled_ReturnsVehicleArray', () => {
        // Setup
        const expectedActiveVehicle = createMockVehicle({
            id: 1,
            assetNumber: 'V-001',
            status: 'Active',
        });
        const expectedInShopVehicle = createMockVehicle({
            id: 2,
            assetNumber: 'V-002',
            status: 'InShop',
        });
        mockApiService.get.and.returnValue(of([expectedActiveVehicle, expectedInShopVehicle]));
        let actualVehicles: Vehicle[] = [];

        // Act
        service.getAll().subscribe((v) => (actualVehicles = v));

        // Result
        expect(actualVehicles.length).toBe(2);
        // First vehicle
        expect(actualVehicles[0].assetNumber).toBe('V-001');
        expect(actualVehicles[0].status).toBe('Active');
        // Second vehicle
        expect(actualVehicles[1].assetNumber).toBe('V-002');
        expect(actualVehicles[1].status).toBe('InShop');
    });

    it('getById_WithId_CallsApiGetWithCorrectPath', () => {
        // Setup
        mockApiService.get.and.returnValue(of(createMockVehicle()));

        // Act
        service.getById(7).subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles/7');
    });

    it('getById_WhenCalled_ReturnsSingleVehicle', () => {
        // Setup
        const expectedVehicle = createMockVehicle({ id: 7, assetNumber: 'V-007' });
        mockApiService.get.and.returnValue(of(expectedVehicle));
        let actualVehicle!: Vehicle;

        // Act
        service.getById(7).subscribe((v) => (actualVehicle = v));

        // Result
        expect(actualVehicle.id).toBe(7);
        expect(actualVehicle.assetNumber).toBe('V-007');
    });

    it('getMaintenanceHistory_WithId_CallsApiGetWithCorrectPath', () => {
        // Setup
        mockApiService.get.and.returnValue(of([]));

        // Act
        service.getMaintenanceHistory(7).subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles/7/maintenance');
    });

    it('getMaintenanceHistory_WhenCalled_ReturnsMaintenanceRecords', () => {
        // Setup
        const expectedOilChangeRecord = createMockMaintenanceRecord({
            maintenanceType: 'OilChange',
        });
        mockApiService.get.and.returnValue(of([expectedOilChangeRecord]));
        let actualRecords: MaintenanceRecord[] = [];

        // Act
        service.getMaintenanceHistory(7).subscribe((r) => (actualRecords = r));

        // Result
        expect(actualRecords.length).toBe(1);
        expect(actualRecords[0].maintenanceType).toBe('OilChange');
    });

    it('getWorkOrders_WithId_CallsApiGetWithCorrectPath', () => {
        // Setup
        mockApiService.get.and.returnValue(of([]));

        // Act
        service.getWorkOrders(7).subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles/7/work-orders');
    });

    it('getSummary_WhenCalled_CallsApiGetWithCorrectPath', () => {
        // Setup
        mockApiService.get.and.returnValue(of(createMockFleetSummary()));

        // Act
        service.getSummary().subscribe();

        // Result
        expect(mockApiService.get).toHaveBeenCalledWith('/vehicles/summary');
    });
});
