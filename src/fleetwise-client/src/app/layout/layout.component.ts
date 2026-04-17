import { Component, OnInit, OnDestroy, inject, signal, viewChild } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { MatSidenavModule, MatSidenav } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'app-layout',
    standalone: true,
    imports: [
        RouterOutlet,
        RouterLink,
        RouterLinkActive,
        MatSidenavModule,
        MatToolbarModule,
        MatListModule,
        MatIconModule,
        MatButtonModule,
    ],
    templateUrl: './layout.component.html',
    styleUrl: './layout.component.scss',
})
export class LayoutComponent implements OnInit, OnDestroy {
    private breakpointObserver = inject(BreakpointObserver);

    readonly sidenav = viewChild<MatSidenav>('sidenav');

    readonly isMobile = signal(false);

    private destroy$ = new Subject<void>();

    ngOnInit(): void {
        this.breakpointObserver
            .observe([Breakpoints.Handset, Breakpoints.TabletPortrait])
            .pipe(takeUntil(this.destroy$))
            .subscribe((result) => {
                this.isMobile.set(result.matches);
                const sidenav = this.sidenav();
                if (sidenav) {
                    if (result.matches) {
                        sidenav.close();
                    } else {
                        sidenav.open();
                    }
                }
            });
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    onNavItemClick(): void {
        if (this.isMobile()) {
            this.sidenav()?.close();
        }
    }
}
