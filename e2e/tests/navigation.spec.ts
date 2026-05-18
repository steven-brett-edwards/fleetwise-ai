import { test, expect } from '../fixtures/app.fixture';

/**
 * Desktop navigation tests run on chromium and firefox.
 * Mobile-specific tests (sidenav overlay) only run on the `mobile-chrome` project
 * because playwright.config.ts sets testMatch to navigation.spec.ts for that project.
 */

test.describe('Navigation - desktop', () => {
    test('sidenav is visible by default on a desktop viewport', async ({ nav, page }) => {
        await page.goto('/');
        await expect(nav.sidenav).toBeVisible();
    });

    test('active route link has active-link class and updates on navigation', async ({
        nav,
        page,
    }) => {
        await page.goto('/');

        // Dashboard link should be active on /
        await expect(nav.activeLink('/')).toBeVisible();

        // Navigate to Vehicles
        await nav.navigateToVehicles();
        await expect(page).toHaveURL('/vehicles');
        await expect(nav.activeLink('/vehicles')).toBeVisible();
        // Dashboard link should no longer be active
        await expect(nav.activeLink('/')).not.toBeVisible();

        // Navigate to AI Chat
        await nav.navigateToChat();
        await expect(page).toHaveURL('/chat');
        await expect(nav.activeLink('/chat')).toBeVisible();
        await expect(nav.activeLink('/vehicles')).not.toBeVisible();
    });

    test('each nav link loads the expected page heading', async ({ nav, page }) => {
        await page.goto('/');

        await nav.navigateToVehicles();
        await expect(page.locator('h1')).toHaveText('Vehicles');

        await nav.navigateToWorkOrders();
        await expect(page.locator('h1')).toHaveText('Work Orders');

        await nav.navigateToDashboard();
        // Dashboard has no <h1>; verify by checking a stat card is present
        await expect(page.locator('[data-testid="stat-total"]')).toBeVisible({ timeout: 10_000 });
    });
});

/**
 * These tests target the `mobile-chrome` project (Pixel 5 viewport).
 * On mobile, the sidenav starts in `mode="over"` and is closed by default.
 */
test.describe('Navigation - mobile', () => {
    // These tests assert on the sidenav overlay behaviour that only occurs on a
    // mobile viewport. They run on every project that picks up navigation.spec.ts,
    // so we skip them when we are not on the dedicated mobile-chrome project.
    test.beforeEach(async ({}, testInfo) => {
        if (testInfo.project.name !== 'mobile-chrome') {
            test.skip();
        }
    });

    test('sidenav is closed by default on a mobile viewport', async ({ nav, page }) => {
        await page.goto('/');
        // mat-drawer-opened is only present when the panel is open
        await expect(nav.sidenav).not.toHaveClass(/mat-drawer-opened/);
    });

    test('hamburger button opens the sidenav', async ({ nav, page }) => {
        await page.goto('/');
        expect(await nav.isSidenavOpen()).toBe(false);

        await nav.openSidenavOnMobile();
        expect(await nav.isSidenavOpen()).toBe(true);
    });

    test('clicking a nav item closes the sidenav and navigates', async ({ nav, page }) => {
        await page.goto('/');
        await nav.openSidenavOnMobile();
        expect(await nav.isSidenavOpen()).toBe(true);

        // onNavItemClick() closes the sidenav on mobile
        await nav.navigateToVehicles();
        await expect(page).toHaveURL('/vehicles');
        expect(await nav.isSidenavOpen()).toBe(false);
    });
});
