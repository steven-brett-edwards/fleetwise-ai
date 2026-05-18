import { test, expect } from '../fixtures/app.fixture';

test.describe('Vehicle List', () => {
    test('loads all 35 vehicles unfiltered', async ({ vehicleList }) => {
        await vehicleList.goto();
        await expect(vehicleList.rows).toHaveCount(35, { timeout: 15_000 });
    });

    test('status filter "Active" shows 29 rows with correct badges', async ({ vehicleList, page }) => {
        await vehicleList.goto();
        await vehicleList.waitForLoad();
        await vehicleList.selectStatus('Active');

        // URL updates to include the query param
        await expect(page).toHaveURL(/status=Active/);
        await expect(vehicleList.rows).toHaveCount(29, { timeout: 10_000 });

        // Every visible status badge should be status-active
        const badges = page.locator('tr.mat-mdc-row span.status-badge');
        const count = await badges.count();
        for (let i = 0; i < count; i++) {
            await expect(badges.nth(i)).toHaveClass(/status-active/);
        }
    });

    test('status filter "InShop" shows 4 rows', async ({ vehicleList, page }) => {
        await vehicleList.goto();
        await vehicleList.waitForLoad();
        await vehicleList.selectStatus('InShop');
        await expect(page).toHaveURL(/status=InShop/);
        await expect(vehicleList.rows).toHaveCount(4, { timeout: 10_000 });
    });

    test('fuel type filter "Electric" shows 3 rows', async ({ vehicleList, page }) => {
        await vehicleList.goto();
        await vehicleList.waitForLoad();
        await vehicleList.selectFuelType('Electric');
        await expect(page).toHaveURL(/fuelType=Electric/);
        // Vehicles 33 (Bolt EV), 34 (E-Transit), 35 (Tesla Model 3)
        await expect(vehicleList.rows).toHaveCount(3, { timeout: 10_000 });
    });

    test('combined query params filter works (Active + Electric)', async ({ vehicleList }) => {
        // All 3 EVs are Active, so combined filter still gives 3 rows
        await vehicleList.goto({ status: 'Active', fuelType: 'Electric' });
        await expect(vehicleList.rows).toHaveCount(3, { timeout: 10_000 });
    });

    test('Clear Filters button is hidden with no filters, appears after filtering, then resets', async ({
        vehicleList,
        page,
    }) => {
        await vehicleList.goto();
        await vehicleList.waitForLoad();
        // Button absent before any filter is applied
        await expect(vehicleList.clearFiltersButton).not.toBeAttached();

        await vehicleList.selectStatus('Active');
        await vehicleList.waitForLoad();
        await expect(vehicleList.clearFiltersButton).toBeVisible();

        await vehicleList.clearFilters();
        await expect(page).toHaveURL('/vehicles');
        await expect(vehicleList.rows).toHaveCount(35, { timeout: 10_000 });
    });

    test('clicking a row navigates to vehicle detail', async ({ vehicleList, page }) => {
        await vehicleList.goto();
        await vehicleList.waitForLoad();
        await vehicleList.clickRow(0);
        await expect(page).toHaveURL(/\/vehicles\/\d+/);
    });
});

test.describe('Vehicle Detail', () => {
    test('shows correct data for vehicle 1 (V-2019-0001)', async ({ vehicleDetail }) => {
        await vehicleDetail.goto(1);
        await vehicleDetail.waitForLoad();

        const title = await vehicleDetail.getTitle();
        expect(title).toContain('V-2019-0001');

        expect(await vehicleDetail.getInfoValue('VIN')).toBe('1FTEW1EP5KFA00001');
        expect(await vehicleDetail.getInfoValue('Status')).toBe('Active');
        expect(await vehicleDetail.getInfoValue('Department')).toBe('Public Works');
    });

    test('maintenance history tab shows records for vehicle 1', async ({ vehicleDetail }) => {
        await vehicleDetail.goto(1);
        await vehicleDetail.waitForLoad();
        await vehicleDetail.clickMaintenanceTab();
        // Vehicle 1 has 15 maintenance records (IDs 1-15)
        await expect(
            vehicleDetail.tabGroup.locator('.mat-mdc-tab-body-active tr.mat-mdc-row'),
        ).toHaveCount(15, { timeout: 10_000 });
    });

    test('work orders tab shows work orders for vehicle 1', async ({ vehicleDetail, page }) => {
        await vehicleDetail.goto(1);
        await vehicleDetail.waitForLoad();
        expect(await vehicleDetail.getWorkOrderRowCount()).toBeGreaterThan(0);

        // WO numbers start with "WO-"
        const firstWoCell = vehicleDetail.tabGroup
            .locator('.mat-mdc-tab-body-active tr.mat-mdc-row')
            .first()
            .locator('td')
            .first();
        await expect(firstWoCell).toContainText('WO-');
    });

    test('clicking a work order row in vehicle detail navigates to work order detail', async ({
        vehicleDetail,
        page,
    }) => {
        await vehicleDetail.goto(1);
        await vehicleDetail.waitForLoad();
        await vehicleDetail.clickWorkOrderRow(0);
        await expect(page).toHaveURL(/\/work-orders\/\d+/);
    });

    test('Back to Vehicles button returns to vehicle list', async ({ vehicleDetail, page }) => {
        await vehicleDetail.goto(1);
        await vehicleDetail.waitForLoad();
        await vehicleDetail.clickBack();
        await expect(page).toHaveURL('/vehicles');
    });

    test('vehicle with null assignedDriver shows "—" placeholder', async ({ vehicleDetail }) => {
        // Vehicle 12 (Ford Transit 150 Passenger) has assignedDriver = null
        await vehicleDetail.goto(12);
        await vehicleDetail.waitForLoad();
        expect(await vehicleDetail.getInfoValue('Assigned Driver')).toBe('—');
    });
});
