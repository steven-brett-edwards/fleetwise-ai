import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
    testDir: './tests',
    fullyParallel: true,
    forbidOnly: !!process.env['CI'],
    retries: process.env['CI'] ? 2 : 0,
    workers: process.env['CI'] ? 2 : undefined,
    reporter: process.env['CI']
        ? [['github'], ['html', { open: 'never', outputFolder: 'playwright-report' }]]
        : [['list'], ['html', { open: 'on-failure', outputFolder: 'playwright-report' }]],

    use: {
        baseURL: 'http://localhost:4200',
        trace: 'on-first-retry',
        screenshot: 'only-on-failure',
        video: 'on-first-retry',
    },

    projects: [
        {
            name: 'chromium',
            use: {
                ...devices['Desktop Chrome'],
                // Explicit screen dimensions ensure window.screen is unambiguously
                // desktop-sized in headless Linux CI, preventing Angular CDK's
                // BreakpointObserver from mis-classifying the viewport as mobile.
                screen: { width: 1920, height: 1080 },
            },
        },
        {
            name: 'firefox',
            use: {
                ...devices['Desktop Firefox'],
                screen: { width: 1920, height: 1080 },
            },
        },
        {
            name: 'mobile-chrome',
            use: { ...devices['Pixel 5'] },
            testMatch: '**/navigation.spec.ts',
        },
    ],

    globalSetup: './global-setup.ts',

    webServer: [
        {
            // .NET API -- must start before Angular so the frontend can reach it
            command:
                'dotnet run --project ../src/FleetWise.Api/FleetWise.Api.csproj --launch-profile http',
            url: 'http://localhost:5180/api/vehicles/summary',
            reuseExistingServer: !process.env['CI'],
            timeout: 90_000,
            stdout: 'ignore',
            stderr: 'pipe',
            env: {
                // Groq mode: skips embedding/RAG setup and boots fast without Ollama.
                // Chat endpoints are mocked at the Playwright network layer so the
                // placeholder API key is never actually used.
                AiProvider: 'Groq',
                Groq__ApiKey: 'e2e-placeholder',
                Groq__ChatModel: 'llama3-8b-8192',
            },
        },
        {
            // Angular dev server
            command: 'npm run start --prefix ../src/fleetwise-client',
            url: 'http://localhost:4200',
            reuseExistingServer: !process.env['CI'],
            timeout: 120_000,
            stdout: 'ignore',
            stderr: 'pipe',
        },
    ],
});
