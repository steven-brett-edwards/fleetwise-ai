import type { Page, Locator } from '@playwright/test';

/**
 * Filterable vehicle table at /vehicles.
 *
 * Angular Material `<mat-select>` does not render as a native `<select>`.
 * The interaction pattern used here is:
 *   1. page.getByLabel('Status').click()  -- opens the overlay panel
 *   2. page.getByRole('option', { name: value }).click()  -- selects the option
 */
export class VehicleListPage {
    readonly page: Page;

    readonly heading: Locator;
    readonly loadingSpinner: Locator;
    readonly table: Locator;
    readonly rows: Locator;
    readonly emptyMessage: Locator;
    readonly clearFiltersButton: Locator;

    constructor(page: Page) {
        this.page = page;
        this.heading = page.locator('h1');
        this.loadingSpinner = page.locator('mat-spinner');
        this.table = page.locator('table[mat-table]');
        this.rows = page.locator('tr.mat-mdc-row');
        this.emptyMessage = page.locator('p.empty-message');
        this.clearFiltersButton = page.locator('button:has-text("Clear Filters")');
    }

    async goto(queryParams?: Record<string, string>): Promise<void> {
        const params = queryParams ? '?' + new URLSearchParams(queryParams).toString() : '';
        await this.page.goto(`/vehicles${params}`);
    }

    async waitForLoad(): Promise<void> {
        await this.loadingSpinner.waitFor({ state: 'hidden' });
    }

    async getRowCount(): Promise<number> {
        return this.rows.count();
    }

    async selectStatus(value: string): Promise<void> {
        await this.page.getByLabel('Status').click();
        await this.page.getByRole('option', { name: value }).click();
    }

    async selectDepartment(value: string): Promise<void> {
        await this.page.getByLabel('Department').click();
        await this.page.getByRole('option', { name: value }).click();
    }

    async selectFuelType(value: string): Promise<void> {
        await this.page.getByLabel('Fuel Type').click();
        await this.page.getByRole('option', { name: value }).click();
    }

    async clearFilters(): Promise<void> {
        await this.clearFiltersButton.click();
    }

    /**
     * Returns the asset number text from the first `<td>` of every visible row.
     */
    async getRowAssetNumbers(): Promise<string[]> {
        const tds = this.rows.locator('td').first();
        return tds.allInnerTexts();
    }

    /**
     * Clicks the row at `index` (0-based). The click navigates to /vehicles/{id}.
     */
    async clickRow(index: number): Promise<void> {
        await this.rows.nth(index).click();
    }

    /**
     * Returns the `.status-badge` span in the Status column for a given row.
     */
    statusBadge(rowIndex: number): Locator {
        // Status is the 3rd column (0-based index 2)
        return this.rows.nth(rowIndex).locator('td').nth(2).locator('span.status-badge');
    }
}
