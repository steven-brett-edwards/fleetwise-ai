import type { Page, Locator } from '@playwright/test';

/**
 * Single-vehicle detail view at /vehicles/{id}.
 * Summary card + tabbed maintenance history / work orders.
 */
export class VehicleDetailPage {
    readonly page: Page;

    readonly loadingSpinner: Locator;
    readonly backButton: Locator;
    readonly cardTitle: Locator;
    readonly infoGrid: Locator;
    readonly tabGroup: Locator;

    constructor(page: Page) {
        this.page = page;
        this.loadingSpinner = page.locator('mat-spinner');
        this.backButton = page.locator('button:has-text("Back to Vehicles")');
        this.cardTitle = page.locator('mat-card-title');
        this.infoGrid = page.locator('div.info-grid');
        this.tabGroup = page.locator('mat-tab-group');
    }

    async goto(vehicleId: number): Promise<void> {
        await this.page.goto(`/vehicles/${vehicleId}`);
    }

    async waitForLoad(): Promise<void> {
        await this.loadingSpinner.waitFor({ state: 'hidden' });
    }

    async getTitle(): Promise<string> {
        return this.cardTitle.innerText();
    }

    /**
     * Reads the `.info-value` (or `.status-badge`) text paired with a given label.
     * The template renders each info pair as:
     *   <div class="info-item">
     *     <span class="info-label">VIN</span>
     *     <span class="info-value">1FT...</span>
     *   </div>
     */
    async getInfoValue(label: string): Promise<string> {
        const item = this.infoGrid
            .locator('div.info-item')
            .filter({ has: this.page.locator('span.info-label', { hasText: label }) });
        // The value may be in .info-value or .status-badge (for Status)
        const valueSpan = item.locator('span.info-value, span.status-badge').first();
        return valueSpan.innerText();
    }

    async clickMaintenanceTab(): Promise<void> {
        await this.page.getByRole('tab', { name: /Maintenance History/ }).click();
    }

    async clickWorkOrdersTab(): Promise<void> {
        await this.page.getByRole('tab', { name: /Work Orders/ }).click();
    }

    /**
     * Row count in the currently active tab's mat-table.
     * After clicking a tab, the active panel's table becomes visible.
     */
    private activeTabRows(): Locator {
        return this.tabGroup.locator('.mat-mdc-tab-body-active tr.mat-mdc-row');
    }

    async getMaintenanceRowCount(): Promise<number> {
        await this.clickMaintenanceTab();
        return this.activeTabRows().count();
    }

    async getWorkOrderRowCount(): Promise<number> {
        await this.clickWorkOrdersTab();
        return this.activeTabRows().count();
    }

    /**
     * Clicks a work order row (they have the `clickable-row` class in the WO tab).
     * Navigates to /work-orders/{id}.
     */
    async clickWorkOrderRow(index: number): Promise<void> {
        await this.clickWorkOrdersTab();
        await this.activeTabRows().nth(index).click();
    }

    async clickBack(): Promise<void> {
        await this.backButton.click();
    }
}
