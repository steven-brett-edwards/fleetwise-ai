import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ApiService } from '../../../app/core/services/api.service';
import { environment } from '../../../environments/environment';

describe('ApiService', () => {
    let service: ApiService;
    let httpTestingController: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [ApiService, provideHttpClient(), provideHttpClientTesting()],
        });

        service = TestBed.inject(ApiService);
        httpTestingController = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpTestingController.verify();
    });

    it('get_WithNoParams_SendsGetRequestToCorrectUrl', () => {
        // Setup
        const expectedPath = '/vehicles';

        // Act
        service.get(expectedPath).subscribe();

        // Result
        const req = httpTestingController.expectOne(`${environment.apiUrl}/vehicles`);
        expect(req.request.method).toBe('GET');
        expect(req.request.params.keys().length).toBe(0);
        req.flush([]);
    });

    it('get_WithParams_AppendsQueryParamsToRequest', () => {
        // Setup
        const expectedParams = { status: 'Active', department: 'Operations' };

        // Act
        service.get('/vehicles', expectedParams).subscribe();

        // Result
        const req = httpTestingController.expectOne(
            (r) => r.url === `${environment.apiUrl}/vehicles`
        );
        expect(req.request.params.get('status')).toBe('Active');
        expect(req.request.params.get('department')).toBe('Operations');
        req.flush([]);
    });

    it('get_WithFalsyParamValues_OmitsFalsyParams', () => {
        // Setup
        const paramsWithEmptyValue = { status: '', department: 'Operations' };

        // Act
        service.get('/vehicles', paramsWithEmptyValue).subscribe();

        // Result
        const req = httpTestingController.expectOne(
            (r) => r.url === `${environment.apiUrl}/vehicles`
        );
        expect(req.request.params.has('status')).toBeFalse();
        expect(req.request.params.get('department')).toBe('Operations');
        req.flush([]);
    });

    it('get_WhenResponseReturned_EmitsTypedResponse', () => {
        // Setup
        const expectedVehicleName = 'Ford F-150';
        let actualResponse: { name: string } | undefined;

        // Act
        service.get<{ name: string }>('/vehicles/1').subscribe((r) => (actualResponse = r));

        // Result
        const req = httpTestingController.expectOne(`${environment.apiUrl}/vehicles/1`);
        req.flush({ name: expectedVehicleName });
        expect(actualResponse!.name).toBe(expectedVehicleName);
    });

    it('get_WithUndefinedParams_SendsRequestWithNoQueryString', () => {
        // Setup & Act
        service.get('/vehicles').subscribe();

        // Result
        const req = httpTestingController.expectOne(`${environment.apiUrl}/vehicles`);
        expect(req.request.params.keys().length).toBe(0);
        req.flush([]);
    });

    it('post_WithBody_SendsPostRequestWithBody', () => {
        // Setup
        const expectedBody = { message: 'Hello', conversationId: 'abc-123' };

        // Act
        service.post('/chat', expectedBody).subscribe();

        // Result
        const req = httpTestingController.expectOne(`${environment.apiUrl}/chat`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(expectedBody);
        req.flush({});
    });

    it('post_WhenResponseReturned_EmitsTypedResponse', () => {
        // Setup
        const expectedResponseText = 'There are 35 vehicles';
        let actualResponse: { response: string } | undefined;

        // Act
        service.post<{ response: string }>('/chat', {}).subscribe((r) => (actualResponse = r));

        // Result
        const req = httpTestingController.expectOne(`${environment.apiUrl}/chat`);
        req.flush({ response: expectedResponseText });
        expect(actualResponse!.response).toBe(expectedResponseText);
    });
});
