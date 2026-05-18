import { test as base } from '@playwright/test';
import type { Route } from '@playwright/test';
import { NavigationPage } from '../pages/navigation.page';
import { DashboardPage } from '../pages/dashboard.page';
import { ChatPage } from '../pages/chat.page';
import { VehicleListPage } from '../pages/vehicle-list.page';
import { VehicleDetailPage } from '../pages/vehicle-detail.page';
import { WorkOrderListPage } from '../pages/work-order-list.page';
import { WorkOrderDetailPage } from '../pages/work-order-detail.page';

type AppFixtures = {
    nav: NavigationPage;
    dashboard: DashboardPage;
    chat: ChatPage;
    vehicleList: VehicleListPage;
    vehicleDetail: VehicleDetailPage;
    workOrderList: WorkOrderListPage;
    workOrderDetail: WorkOrderDetailPage;
    /** Call this before navigating to /chat to intercept the streaming endpoint. */
    mockChatStream: (responseText: string) => Promise<void>;
};

export const test = base.extend<AppFixtures>({
    nav: async ({ page }, use) => {
        await use(new NavigationPage(page));
    },

    dashboard: async ({ page }, use) => {
        await use(new DashboardPage(page));
    },

    chat: async ({ page }, use) => {
        await use(new ChatPage(page));
    },

    vehicleList: async ({ page }, use) => {
        await use(new VehicleListPage(page));
    },

    vehicleDetail: async ({ page }, use) => {
        await use(new VehicleDetailPage(page));
    },

    workOrderList: async ({ page }, use) => {
        await use(new WorkOrderListPage(page));
    },

    workOrderDetail: async ({ page }, use) => {
        await use(new WorkOrderDetailPage(page));
    },

    mockChatStream: async ({ page }, use) => {
        const mock = async (responseText: string): Promise<void> => {
            await page.route('**/api/chat/stream', async (route: Route) => {
                // ChatController.Stream escapes newlines and backslashes before writing
                // each token to the SSE stream. We replicate that encoding here so the
                // Angular ChatService SSE decoder sees exactly what the real backend sends.
                const escaped = responseText
                    .replace(/\\/g, '\\\\')
                    .replace(/\r/g, '\\r')
                    .replace(/\n/g, '\\n');

                // Deliver the whole response as a single data frame followed by [DONE].
                const body = `data: ${escaped}\n\ndata: [DONE]\n\n`;

                await route.fulfill({
                    status: 200,
                    contentType: 'text/event-stream',
                    headers: {
                        'Cache-Control': 'no-cache',
                        Connection: 'keep-alive',
                        'Access-Control-Allow-Origin': '*',
                    },
                    body,
                });
            });
        };

        await use(mock);

        // Tear down: remove all route intercepts so tests don't bleed into each other.
        await page.unrouteAll({ behavior: 'ignoreErrors' });
    },
});

export { expect } from '@playwright/test';
