import { test, expect } from '../fixtures/app.fixture';

test.describe('Work Order List', () => {
    test('loads all 36 work orders unfiltered', async ({ workOrderList }) => {
        await workOrderList.goto();
        await expect(workOrderList.rows).toHaveCount(36, { timeout: 15_000 });
    });

    test('status filter "Open" shows 8 rows and updates URL', async ({
        workOrderList,
        page,
    }) => {
        await workOrderList.goto();
        await workOrderList.waitForLoad();
        await workOrderList.selectStatus('Open');
        await expect(page).toHaveURL(/status=Open/);
        // Open WOs: IDs 20, 30, 31, 32, 33, 34, 35, 36
        await expect(workOrderList.rows).toHaveCount(8, { timeout: 10_000 });
    });

    test('status filter "Completed" shows 18 rows', async ({ workOrderList, page }) => {
        await workOrderList.goto();
        await workOrderList.waitForLoad();
        await workOrderList.selectStatus('Completed');
        await expect(page).toHaveURL(/status=Completed/);
        await expect(workOrderList.rows).toHaveCount(18, { timeout: 10_000 });
    });

    test('Clear Filter button resets to all 36 work orders', async ({
        workOrderList,
        page,
    }) => {
        await workOrderList.goto();
        await workOrderList.waitForLoad();
        await workOrderList.selectStatus('Open');
        await workOrderList.waitForLoad();

        await workOrderList.clearFilter();
        await expect(page).toHaveURL('/work-orders');
        await expect(workOrderList.rows).toHaveCount(36, { timeout: 10_000 });
    });

    test('clicking a row navigates to work order detail', async ({ workOrderList, page }) => {
        await workOrderList.goto();
        await workOrderList.waitForLoad();
        await workOrderList.clickRow(0);
        await expect(page).toHaveURL(/\/work-orders\/\d+/);
    });

    test('priority badges carry the correct CSS class', async ({ workOrderList, page }) => {
        await workOrderList.goto();
        await workOrderList.waitForLoad();

        // Find any Critical priority badge in the table
        const criticalBadge = page
            .locator('tr.mat-mdc-row span.priority-badge.priority-critical')
            .first();
        await expect(criticalBadge).toBeVisible();
        await expect(criticalBadge).toHaveClass(/priority-critical/);
    });
});

test.describe('Work Order Detail', () => {
    test('shows correct data for WO-2026-00019 (Critical / InProgress)', async ({
        workOrderDetail,
    }) => {
        await workOrderDetail.goto(19);
        await workOrderDetail.waitForLoad();

        expect(await workOrderDetail.getTitle()).toContain('WO-2026-00019');
        expect(await workOrderDetail.getInfoValue('Status')).toBe('InProgress');
        expect(await workOrderDetail.getInfoValue('Priority')).toBe('Critical');

        const description = await workOrderDetail.descriptionSection.innerText();
        expect(description).toContain('Hydraulic lift cylinder failure');
    });

    test('vehicle link navigates to the correct vehicle detail page', async ({
        workOrderDetail,
        page,
    }) => {
        await workOrderDetail.goto(19);
        await workOrderDetail.waitForLoad();
        await workOrderDetail.clickVehicleLink();
        // WO 19 is on vehicleId = 23 (V-2019-0023)
        await expect(page).toHaveURL('/vehicles/23');
    });

    test('open work order with no technician shows "—" for labor and technician', async ({
        workOrderDetail,
    }) => {
        // WO 20 is Open with assignedTechnician = null, laborHours = null
        await workOrderDetail.goto(20);
        await workOrderDetail.waitForLoad();

        expect(await workOrderDetail.getInfoValue('Labor Hours')).toBe('—');
        expect(await workOrderDetail.getInfoValue('Assigned Technician')).toBe('—');
    });

    test('completed work order shows formatted total cost', async ({ workOrderDetail }) => {
        // WO 1: Completed, totalCost = 125.50
        await workOrderDetail.goto(1);
        await workOrderDetail.waitForLoad();

        const cost = await workOrderDetail.getInfoValue('Total Cost');
        expect(cost).toContain('125.50');
    });

    test('Back to Work Orders button returns to list', async ({ workOrderDetail, page }) => {
        await workOrderDetail.goto(1);
        await workOrderDetail.waitForLoad();
        await workOrderDetail.clickBack();
        await expect(page).toHaveURL('/work-orders');
    });
});
