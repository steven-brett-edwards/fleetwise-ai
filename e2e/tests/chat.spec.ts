import { test, expect } from '../fixtures/app.fixture';

/**
 * All chat tests mock POST /api/chat/stream via the `mockChatStream` fixture
 * so they never require a running LLM service.
 */
test.describe('Chat', () => {
    test('shows welcome screen with example prompts on first load', async ({ chat }) => {
        await chat.goto();
        await expect(chat.welcomePanel).toBeVisible();
        await expect(chat.page.locator('div.welcome h2')).toHaveText('FleetWise AI Assistant');
        // Template has three <li> example prompts
        await expect(chat.examplePrompts).toHaveCount(3);
    });

    test('send button is disabled when input is empty', async ({ chat }) => {
        await chat.goto();
        await expect(chat.sendButton).toBeDisabled();

        await chat.typeMessage('Hello');
        await expect(chat.sendButton).toBeEnabled();

        await chat.textarea.clear();
        await expect(chat.sendButton).toBeDisabled();
    });

    test('submitting a message via Enter shows user bubble then assistant response', async ({
        chat,
        mockChatStream,
    }) => {
        await mockChatStream('There are 35 vehicles in the fleet.');
        await chat.goto();

        await chat.sendViaEnter('How many vehicles are in the fleet?');

        // User message bubble appears immediately
        await expect(chat.userMessages.last()).toContainText('How many vehicles are in the fleet?');

        // Wait for the full streaming response
        await chat.waitForStreamingComplete();

        // Assistant message contains the mocked response
        const lastAssistant = await chat.getLastAssistantMessage();
        expect(lastAssistant).toContain('35 vehicles');
    });

    test('progress bar appears during streaming and disappears when done', async ({
        chat,
        page,
    }) => {
        // Hold the route open so we can assert on the in-progress state before
        // releasing the response. This avoids a race where an instant mock response
        // resolves before the progress bar has a chance to render.
        let releaseRoute!: () => void;
        const routeHeld = new Promise<void>((r) => (releaseRoute = r));

        await page.route('**/api/chat/stream', async (route) => {
            await routeHeld;
            await route.fulfill({
                status: 200,
                contentType: 'text/event-stream',
                body: 'data: Fleet is healthy.\n\ndata: [DONE]\n\n',
            });
        });

        await chat.goto();
        await chat.typeMessage('Fleet status?');
        await chat.send();

        // While route is held open, streaming() is true and the bar is rendered
        await expect(chat.progressBar).toBeVisible();

        // Release the response and wait for streaming to complete
        releaseRoute();
        await chat.waitForStreamingComplete();
        await expect(chat.progressBar).not.toBeAttached();
    });

    test('assistant message renders markdown as HTML', async ({ chat, mockChatStream }) => {
        await mockChatStream('## Fleet Status\n\nThere are **35** vehicles.');
        await chat.goto();

        await chat.sendViaEnter('Summary?');
        await chat.waitForStreamingComplete();

        const lastBubble = chat.assistantMessages.last();
        // ngx-markdown renders ## as <h2> and **bold** as <strong>
        await expect(lastBubble.locator('h2')).toBeVisible();
        await expect(lastBubble.locator('strong')).toContainText('35');
    });

    test('send button is disabled while streaming is in progress', async ({ chat, page }) => {
        // Hold the route open so streaming() stays true long enough to assert on.
        let releaseRoute!: () => void;
        const routeHeld = new Promise<void>((r) => (releaseRoute = r));

        await page.route('**/api/chat/stream', async (route) => {
            await routeHeld;
            await route.fulfill({
                status: 200,
                contentType: 'text/event-stream',
                body: 'data: Hello\n\ndata: [DONE]\n\n',
            });
        });

        await chat.goto();
        await chat.typeMessage('Test');
        await chat.send();

        // While route is held, streaming() is true so button must be disabled
        await expect(chat.sendButton).toBeDisabled();

        // Release and wait for streaming to finish
        releaseRoute();
        await chat.waitForStreamingComplete();

        // The input is cleared after send, so type something to confirm the button
        // re-enables on input alone -- proving streaming is no longer blocking it.
        await chat.typeMessage('follow-up');
        await expect(chat.sendButton).toBeEnabled();
    });

    test('Shift+Enter inserts a newline instead of submitting', async ({ chat }) => {
        await chat.goto();

        await chat.textarea.fill('Hello');
        await chat.textarea.press('Shift+Enter');
        await chat.textarea.type(' World');

        // No message was sent -- welcome screen still visible
        await expect(chat.welcomePanel).toBeVisible();
        await expect(chat.messages).toHaveCount(0);
    });

    test('conversation accumulates across multiple turns', async ({ chat, page }) => {
        // Use a single route handler that serves different responses per call.
        const responses = ['First response', 'Second response'];
        let callIndex = 0;

        await page.route('**/api/chat/stream', async (route) => {
            const text = responses[callIndex++] ?? 'fallback';
            await route.fulfill({
                status: 200,
                contentType: 'text/event-stream',
                body: `data: ${text}\n\ndata: [DONE]\n\n`,
            });
        });

        await chat.goto();

        await chat.sendViaEnter('First question');
        // Wait for the assistant message to actually appear in the DOM
        await expect(chat.assistantMessages).toHaveCount(1);

        await chat.sendViaEnter('Second question');
        await expect(chat.assistantMessages).toHaveCount(2);

        // 2 user + 2 assistant = 4 total messages
        expect(await chat.getMessageCount()).toBe(4);
    });
});
