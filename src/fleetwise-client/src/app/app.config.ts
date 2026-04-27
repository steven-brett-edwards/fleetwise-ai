import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideMarkdown } from 'ngx-markdown';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

export const appConfig: ApplicationConfig = {
    providers: [
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideAnimationsAsync(),
        provideHttpClient(),
        // Markdown rendering for assistant chat replies. The LangChain-side
        // (well, Semantic Kernel here) emits GH-flavored markdown -- tables,
        // bold, bullets, headers -- and we used to render it as raw text,
        // which made tables of vehicles look like `|---|---|`. ngx-markdown
        // wraps `marked` and gives us a `<markdown>` component to drop in.
        provideMarkdown(),
    ],
};
