import type { Page, Locator } from '@playwright/test';
import { expect } from '@playwright/test';

/**
 * Wraps the Material sidenav / toolbar so every test can navigate
 * without repeating sidenav-open logic.
 */
export class NavigationPage {
    readonly page: Page;

    readonly sidenav: Locator;
    readonly menuButton: Locator;
    readonly navList: Locator;

    constructor(page: Page) {
        this.page = page;
        this.sidenav = page.locator('mat-sidenav');
        this.menuButton = page.locator('button[data-testid="menu-toggle"]');
        this.navList = page.locator('mat-nav-list');
    }

    private navLink(routerLink: string): Locator {
        return this.navList.locator(`a[routerLink="${routerLink}"]`);
    }

    /**
     * Ensures the sidenav is open before interacting with nav links.
     * On desktop the sidenav is always open so this is a no-op.
     * On mobile (or in CI environments where the breakpoint fires
     * unexpectedly) it clicks the hamburger button and waits for the
     * drawer to open.
     */
    async ensureSidenavOpen(): Promise<void> {
        if (!(await this.isSidenavOpen())) {
            await this.menuButton.click();
            await expect(this.sidenav).toHaveClass(/mat-drawer-opened/, { timeout: 5_000 });
        }
    }

    async navigateToDashboard(): Promise<void> {
        await this.ensureSidenavOpen();
        await this.navLink('/').click();
    }

    async navigateToChat(): Promise<void> {
        await this.ensureSidenavOpen();
        await this.navLink('/chat').click();
    }

    async navigateToVehicles(): Promise<void> {
        await this.ensureSidenavOpen();
        await this.navLink('/vehicles').click();
    }

    async navigateToWorkOrders(): Promise<void> {
        await this.ensureSidenavOpen();
        await this.navLink('/work-orders').click();
    }

    /** Opens the sidenav on a mobile viewport where it starts collapsed. */
    async openSidenavOnMobile(): Promise<void> {
        await this.menuButton.click();
    }

    /**
     * Returns true when the sidenav has the `mat-drawer-opened` class,
     * which Angular Material adds when the panel is open.
     */
    async isSidenavOpen(): Promise<boolean> {
        const classes = await this.sidenav.getAttribute('class');
        return classes?.includes('mat-drawer-opened') ?? false;
    }

    /**
     * Asserts that the nav link for the given routerLink path has
     * the `active-link` CSS class applied by routerLinkActive.
     */
    activeLink(routerLink: string): Locator {
        return this.navList.locator(`a.active-link[routerLink="${routerLink}"]`);
    }
}
