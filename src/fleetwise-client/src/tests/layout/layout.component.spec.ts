import { TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { BreakpointObserver, BreakpointState, Breakpoints } from '@angular/cdk/layout';
import { MatSidenav } from '@angular/material/sidenav';
import { ActivatedRoute } from '@angular/router';
import { LayoutComponent } from '../../app/layout/layout.component';

describe('LayoutComponent', () => {
    let component: LayoutComponent;
    let breakpointSubject: BehaviorSubject<BreakpointState>;
    let mockBreakpointObserver: { observe: jasmine.Spy };
    let mockSidenav: jasmine.SpyObj<MatSidenav>;

    beforeEach(() => {
        breakpointSubject = new BehaviorSubject<BreakpointState>({
            matches: false,
            breakpoints: {},
        });
        mockBreakpointObserver = {
            observe: jasmine.createSpy('observe').and.returnValue(breakpointSubject.asObservable()),
        };
        mockSidenav = jasmine.createSpyObj('MatSidenav', ['open', 'close']);

        TestBed.configureTestingModule({
            imports: [LayoutComponent, NoopAnimationsModule],
            providers: [
                { provide: BreakpointObserver, useValue: mockBreakpointObserver },
                { provide: ActivatedRoute, useValue: {} },
            ],
            schemas: [NO_ERRORS_SCHEMA],
        });

        const fixture = TestBed.createComponent(LayoutComponent);
        component = fixture.componentInstance;
        component.sidenav = mockSidenav;
    });

    it('ngOnInit_WhenBreakpointMatchesMobile_SetsIsMobileTrue', () => {
        // Setup
        breakpointSubject.next({ matches: true, breakpoints: {} });

        // Act
        component.ngOnInit();

        // Result
        expect(component.isMobile).toBeTrue();
    });

    it('ngOnInit_WhenBreakpointDoesNotMatch_SetsIsMobileFalse', () => {
        // Setup
        breakpointSubject.next({ matches: false, breakpoints: {} });

        // Act
        component.ngOnInit();

        // Result
        expect(component.isMobile).toBeFalse();
    });

    it('ngOnInit_WhenMobileAndSidenavExists_ClosesSidenav', () => {
        // Setup
        breakpointSubject.next({ matches: true, breakpoints: {} });

        // Act
        component.ngOnInit();

        // Result
        expect(mockSidenav.close).toHaveBeenCalled();
    });

    it('ngOnInit_WhenDesktopAndSidenavExists_OpensSidenav', () => {
        // Setup
        breakpointSubject.next({ matches: false, breakpoints: {} });

        // Act
        component.ngOnInit();

        // Result
        expect(mockSidenav.open).toHaveBeenCalled();
    });

    it('ngOnInit_WhenSidenavNotYetAvailable_DoesNotThrow', () => {
        // Setup
        component.sidenav = undefined as unknown as MatSidenav;
        breakpointSubject.next({ matches: true, breakpoints: {} });

        // Act & Result
        expect(() => component.ngOnInit()).not.toThrow();
    });

    it('ngOnInit_WhenCalled_ObservesHandsetAndTabletPortraitBreakpoints', () => {
        // Act
        component.ngOnInit();

        // Result
        expect(mockBreakpointObserver.observe).toHaveBeenCalledWith([
            Breakpoints.Handset,
            Breakpoints.TabletPortrait,
        ]);
    });

    it('ngOnDestroy_WhenCalled_CompletesDestroySubject', () => {
        // Setup
        component.ngOnInit();
        const destroySpy = spyOn(component['destroy$'], 'complete');

        // Act
        component.ngOnDestroy();

        // Result
        expect(destroySpy).toHaveBeenCalled();
    });

    it('ngOnDestroy_WhenCalled_UnsubscribesFromBreakpointObserver', () => {
        // Setup
        component.ngOnInit();
        component.isMobile = false;
        component.ngOnDestroy();

        // Act
        breakpointSubject.next({ matches: true, breakpoints: {} });

        // Result
        expect(component.isMobile).toBeFalse();
    });

    it('onNavItemClick_WhenMobile_ClosesSidenav', () => {
        // Setup
        component.isMobile = true;

        // Act
        component.onNavItemClick();

        // Result
        expect(mockSidenav.close).toHaveBeenCalled();
    });

    it('onNavItemClick_WhenDesktop_DoesNotCloseSidenav', () => {
        // Setup
        component.isMobile = false;
        mockSidenav.close.calls.reset();

        // Act
        component.onNavItemClick();

        // Result
        expect(mockSidenav.close).not.toHaveBeenCalled();
    });
});
