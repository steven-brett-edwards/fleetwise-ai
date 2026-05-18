import type { FullConfig } from '@playwright/test';

/**
 * Global setup runs once before any test. By the time this executes, both
 * webServer entries in playwright.config.ts have already started and passed
 * their health checks (webServer.url polling).
 *
 * We add a second-level guard here: verify that seed data actually loaded.
 * If SeedData.Initialize() somehow failed we fail fast with a clear message
 * rather than watching every test fail with cryptic DOM-not-found errors.
 */
async function globalSetup(_config: FullConfig): Promise<void> {
    const apiBase = 'http://localhost:5180';

    const response = await fetch(`${apiBase}/api/vehicles/summary`);
    if (!response.ok) {
        throw new Error(
            `FleetWise API health check failed: ${response.status} ${response.statusText}. ` +
                `Is the .NET API running on ${apiBase}?`,
        );
    }

    const summary = await response.json() as { totalVehicles: number };
    if (summary.totalVehicles !== 35) {
        throw new Error(
            `Seed data check failed: expected 35 vehicles, got ${summary.totalVehicles}. ` +
                `Ensure SeedData.Initialize() ran successfully (AiProvider=Groq, no DB conflicts).`,
        );
    }
}

export default globalSetup;
