import type { Page, Locator } from '@playwright/test';

/**
 * Single work order detail card at /work-orders/{id}.
 */
export class WorkOrderDetailPage {
    readonly page: Page;

    readonly loadingSpinner: Locator;
    readonly backButton: Locator;
    readonly cardTitle: Locator;
    readonly infoGrid: Locator;
    /**
     * The vehicle link uses `(click)="goToVehicle()"` and `role="link"`,
     * not a standard `href`. We target it by its CSS class.
     */
    readonly vehicleLink: Locator;
    readonly descriptionSection: Locator;

    constructor(page: Page) {
        this.page = page;
        this.loadingSpinner = page.locator('mat-spinner');
        this.backButton = page.locator('button:has-text("Back to Work Orders")');
        this.cardTitle = page.locator('mat-card-title');
        this.infoGrid = page.locator('div.info-grid');
        this.vehicleLink = page.locator('a.vehicle-link');
        this.descriptionSection = page.locator('div.description-section p');
    }

    async goto(workOrderId: number): Promise<void> {
        await this.page.goto(`/work-orders/${workOrderId}`);
    }

    async waitForLoad(): Promise<void> {
        await this.loadingSpinner.waitFor({ state: 'hidden' });
    }

    async getTitle(): Promise<string> {
        return this.cardTitle.innerText();
    }

    /**
     * Reads the `.info-value` (or `.status-badge` / `.priority-badge`) text
     * paired with the given label.
     */
    async getInfoValue(label: string): Promise<string> {
        const item = this.infoGrid
            .locator('div.info-item')
            .filter({ has: this.page.locator('span.info-label', { hasText: label }) });
        const valueEl = item
            .locator('span.info-value, span.status-badge, span.priority-badge')
            .first();
        return valueEl.innerText();
    }

    async clickVehicleLink(): Promise<void> {
        await this.vehicleLink.click();
    }

    async clickBack(): Promise<void> {
        await this.backButton.click();
    }
}
