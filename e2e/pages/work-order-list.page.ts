import type { Page, Locator } from '@playwright/test';

/**
 * Work order table with status filter at /work-orders.
 */
export class WorkOrderListPage {
    readonly page: Page;

    readonly heading: Locator;
    readonly loadingSpinner: Locator;
    readonly table: Locator;
    readonly rows: Locator;
    readonly emptyMessage: Locator;
    readonly clearFilterButton: Locator;

    constructor(page: Page) {
        this.page = page;
        this.heading = page.locator('h1');
        this.loadingSpinner = page.locator('mat-spinner');
        this.table = page.locator('table[mat-table]');
        this.rows = page.locator('tr.mat-mdc-row');
        this.emptyMessage = page.locator('p.empty-message');
        // Template uses "Clear Filter" (singular) for the work order list
        this.clearFilterButton = page.locator('button:has-text("Clear Filter")');
    }

    async goto(queryParams?: Record<string, string>): Promise<void> {
        const params = queryParams ? '?' + new URLSearchParams(queryParams).toString() : '';
        await this.page.goto(`/work-orders${params}`);
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

    async clearFilter(): Promise<void> {
        await this.clearFilterButton.click();
    }

    async clickRow(index: number): Promise<void> {
        await this.rows.nth(index).click();
    }

    /**
     * Returns the `.status-badge` span for a given row (3rd column, 0-based index 2).
     */
    statusBadge(rowIndex: number): Locator {
        return this.rows.nth(rowIndex).locator('td').nth(2).locator('span.status-badge');
    }

    /**
     * Returns the `.priority-badge` span for a given row (4th column, 0-based index 3).
     */
    priorityBadge(rowIndex: number): Locator {
        return this.rows.nth(rowIndex).locator('td').nth(3).locator('span.priority-badge');
    }
}
