import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { MaintenanceService } from '../../../app/core/services/maintenance.service';
import { ApiService } from '../../../app/core/services/api.service';
import { createMockMaintenanceSchedule } from '../../helpers/mock-data.factory';

describe('MaintenanceService', () => {
  let service: MaintenanceService;
  let mockApiService: jasmine.SpyObj<ApiService>;

  beforeEach(() => {
    mockApiService = jasmine.createSpyObj('ApiService', ['get', 'post']);

    TestBed.configureTestingModule({
      providers: [
        MaintenanceService,
        { provide: ApiService, useValue: mockApiService },
      ],
    });

    service = TestBed.inject(MaintenanceService);
  });

  it('getOverdue_WhenCalled_CallsApiGetWithCorrectPath', () => {
    // Setup
    mockApiService.get.and.returnValue(of([]));

    // Act
    service.getOverdue().subscribe();

    // Result
    expect(mockApiService.get).toHaveBeenCalledWith('/maintenance/overdue');
  });

  it('getOverdue_WhenCalled_ReturnsOverdueSchedules', () => {
    // Setup
    const expectedOverdueSchedule = createMockMaintenanceSchedule({ maintenanceType: 'BrakeInspection' });
    mockApiService.get.and.returnValue(of([expectedOverdueSchedule]));
    let actualSchedules: any[] = [];

    // Act
    service.getOverdue().subscribe(s => actualSchedules = s);

    // Result
    expect(actualSchedules.length).toBe(1);
    expect(actualSchedules[0].maintenanceType).toBe('BrakeInspection');
  });

  it('getUpcoming_WithDefaults_CallsApiGetWithDefaultDaysAndMiles', () => {
    // Setup
    mockApiService.get.and.returnValue(of([]));

    // Act
    service.getUpcoming().subscribe();

    // Result
    expect(mockApiService.get).toHaveBeenCalledWith('/maintenance/upcoming', {
      days: '30',
      miles: '5000',
    });
  });

  it('getUpcoming_WithCustomValues_CallsApiGetWithProvidedDaysAndMiles', () => {
    // Setup
    mockApiService.get.and.returnValue(of([]));
    const customDays = 60;
    const customMiles = 10000;

    // Act
    service.getUpcoming(customDays, customMiles).subscribe();

    // Result
    expect(mockApiService.get).toHaveBeenCalledWith('/maintenance/upcoming', {
      days: '60',
      miles: '10000',
    });
  });

  it('getUpcoming_WhenCalled_ReturnsUpcomingSchedules', () => {
    // Setup
    const expectedUpcomingSchedule = createMockMaintenanceSchedule({ maintenanceType: 'TireRotation' });
    mockApiService.get.and.returnValue(of([expectedUpcomingSchedule]));
    let actualSchedules: any[] = [];

    // Act
    service.getUpcoming().subscribe(s => actualSchedules = s);

    // Result
    expect(actualSchedules.length).toBe(1);
    expect(actualSchedules[0].maintenanceType).toBe('TireRotation');
  });
});
