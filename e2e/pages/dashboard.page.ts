import type { Page, Locator } from '@playwright/test';

/**
 * Fleet dashboard: stat cards + overdue/upcoming maintenance tables.
 */
export class DashboardPage {
    readonly page: Page;

    readonly loadingSpinner: Locator;
    readonly statCards: Locator;

    // Individual stat cards (require data-testid added to dashboard.component.html)
    readonly totalCard: Locator;
    readonly activeCard: Locator;
    readonly inShopCard: Locator;
    readonly openWorkOrdersCard: Locator;

    // The two maintenance tables live inside `.tables-row mat-card`
    readonly tablesRow: Locator;

    constructor(page: Page) {
        this.page = page;
        this.loadingSpinner = page.locator('mat-spinner');
        this.statCards = page.locator('div.stat-cards mat-card');
        this.totalCard = page.locator('[data-testid="stat-total"]');
        this.activeCard = page.locator('[data-testid="stat-active"]');
        this.inShopCard = page.locator('[data-testid="stat-inshop"]');
        this.openWorkOrdersCard = page.locator('[data-testid="stat-open-work-orders"]');
        this.tablesRow = page.locator('div.tables-row');
    }

    async goto(): Promise<void> {
        await this.page.goto('/');
    }

    /** Waits for the loading spinner to disappear. */
    async waitForLoad(): Promise<void> {
        await this.loadingSpinner.waitFor({ state: 'hidden' });
    }

    /**
     * Returns the numeric value shown in a stat card's `.stat-value` div.
     */
    async getStatValue(card: Locator): Promise<number> {
        const text = await card.locator('div.stat-value').innerText();
        return parseInt(text.trim(), 10);
    }

    /**
     * Returns all `tr.mat-mdc-row` locators in the overdue maintenance table
     * (the first mat-table inside tables-row).
     */
    overdueRows(): Locator {
        return this.tablesRow.locator('mat-card').first().locator('tr.mat-mdc-row');
    }

    /**
     * Returns all `tr.mat-mdc-row` locators in the upcoming maintenance table
     * (the second mat-table inside tables-row).
     */
    upcomingRows(): Locator {
        return this.tablesRow.locator('mat-card').nth(1).locator('tr.mat-mdc-row');
    }

    async getOverdueRowCount(): Promise<number> {
        return this.overdueRows().count();
    }

    async getUpcomingRowCount(): Promise<number> {
        return this.upcomingRows().count();
    }

    /**
     * Returns the `<a>` link element in the first cell of an overdue table row.
     */
    overdueVehicleLink(rowIndex: number): Locator {
        return this.overdueRows().nth(rowIndex).locator('td').first().locator('a');
    }
}
