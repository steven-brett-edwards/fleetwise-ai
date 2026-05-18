import type { Page, Locator } from '@playwright/test';

/**
 * AI chat interface: SSE streaming, markdown rendering, message bubbles.
 */
export class ChatPage {
    readonly page: Page;

    readonly welcomePanel: Locator;
    readonly examplePrompts: Locator;
    readonly messageList: Locator;
    readonly messages: Locator;
    readonly userMessages: Locator;
    readonly assistantMessages: Locator;
    readonly textarea: Locator;
    readonly sendButton: Locator;
    readonly progressBar: Locator;

    constructor(page: Page) {
        this.page = page;
        this.welcomePanel = page.locator('div.welcome');
        this.examplePrompts = page.locator('div.welcome ul li');
        this.messageList = page.locator('div.message-list');
        this.messages = page.locator('div.message');
        this.userMessages = page.locator('div.message.user');
        this.assistantMessages = page.locator('div.message.assistant');
        this.textarea = page.locator('textarea[matInput]');
        this.sendButton = page.locator('button[mat-fab]');
        this.progressBar = page.locator('mat-progress-bar');
    }

    async goto(): Promise<void> {
        await this.page.goto('/chat');
    }

    async typeMessage(text: string): Promise<void> {
        await this.textarea.fill(text);
    }

    async send(): Promise<void> {
        await this.sendButton.click();
    }

    /**
     * Types a message and presses Enter to submit (the keyboard path).
     */
    async sendViaEnter(text: string): Promise<void> {
        await this.textarea.fill(text);
        await this.textarea.press('Enter');
    }

    /**
     * Waits until the progress bar detaches from the DOM, signalling that
     * the streaming response has finished.
     */
    async waitForStreamingComplete(): Promise<void> {
        await this.progressBar.waitFor({ state: 'detached', timeout: 15_000 });
    }

    async getLastAssistantMessage(): Promise<string> {
        const all = await this.assistantMessages.all();
        const last = all.at(-1);
        if (!last) throw new Error('No assistant message found');
        return last.innerText();
    }

    async getMessageCount(): Promise<number> {
        return this.messages.count();
    }

    async isSendButtonDisabled(): Promise<boolean> {
        return this.sendButton.isDisabled();
    }
}
