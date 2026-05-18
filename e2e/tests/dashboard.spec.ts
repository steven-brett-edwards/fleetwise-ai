import { test, expect } from '../fixtures/app.fixture';

test.describe('Dashboard', () => {
    test.beforeEach(async ({ dashboard }) => {
        await dashboard.goto();
        await dashboard.waitForLoad();
    });

    test('shows four stat cards after data loads', async ({ dashboard }) => {
        await expect(dashboard.statCards).toHaveCount(4);
        await expect(dashboard.totalCard).toBeVisible();
        await expect(dashboard.activeCard).toBeVisible();
        await expect(dashboard.inShopCard).toBeVisible();
        await expect(dashboard.openWorkOrdersCard).toBeVisible();
    });

    test('Total Vehicles card shows 35 from seed data', async ({ dashboard }) => {
        const count = await dashboard.getStatValue(dashboard.totalCard);
        expect(count).toBe(35);
    });

    test('Active card navigates to vehicles filtered by Active status', async ({
        dashboard,
        page,
    }) => {
        await dashboard.activeCard.click();
        await expect(page).toHaveURL(/\/vehicles\?status=Active/);
        // 35 total - 4 InShop - 2 OutOfService = 29 Active
        await expect(page.locator('tr.mat-mdc-row')).toHaveCount(29, { timeout: 10_000 });
    });

    test('InShop card navigates to vehicles filtered by InShop status', async ({
        dashboard,
        page,
    }) => {
        await dashboard.inShopCard.click();
        await expect(page).toHaveURL(/\/vehicles\?status=InShop/);
        // Vehicles 6, 10, 23, 31
        await expect(page.locator('tr.mat-mdc-row')).toHaveCount(4, { timeout: 10_000 });
    });

    test('Open Work Orders card navigates to work orders filtered by Open status', async ({
        dashboard,
        page,
    }) => {
        await dashboard.openWorkOrdersCard.click();
        await expect(page).toHaveURL(/\/work-orders\?status=Open/);
        // IDs 20, 30-36 = 8 Open work orders
        await expect(page.locator('tr.mat-mdc-row')).toHaveCount(8, { timeout: 10_000 });
    });

    test('overdue maintenance table rows each contain a vehicle link', async ({ dashboard, page }) => {
        const rowCount = await dashboard.getOverdueRowCount();
        expect(rowCount).toBeGreaterThan(0);

        // First row's asset-number cell should be an anchor
        const firstLink = dashboard.overdueVehicleLink(0);
        await expect(firstLink).toBeVisible();

        // Clicking the link navigates to vehicle detail
        await firstLink.click();
        await expect(page).toHaveURL(/\/vehicles\/\d+/);
    });

    test('upcoming maintenance table renders column headers', async ({ dashboard }) => {
        const upcomingTable = dashboard.tablesRow.locator('mat-card').nth(1).locator('table');
        await expect(upcomingTable.locator('th:has-text("Asset #")')).toBeVisible();
        await expect(upcomingTable.locator('th:has-text("Vehicle")')).toBeVisible();
        await expect(upcomingTable.locator('th:has-text("Type")')).toBeVisible();
        await expect(upcomingTable.locator('th:has-text("Due Date")')).toBeVisible();
        await expect(upcomingTable.locator('th:has-text("Due Mileage")')).toBeVisible();
        expect(await dashboard.getUpcomingRowCount()).toBeGreaterThan(0);
    });

    test('no API requests fail during dashboard load', async ({ page }) => {
        const failedRequests: string[] = [];
        page.on('requestfailed', (req) => {
            if (req.url().includes('/api/')) {
                failedRequests.push(req.url());
            }
        });
        await page.goto('/');
        await page.locator('mat-spinner').waitFor({ state: 'hidden' });
        expect(failedRequests).toHaveLength(0);
    });
});
