# FleetWise AI

[![CI](https://github.com/steven-brett-edwards/fleetwise-ai/actions/workflows/ci.yml/badge.svg)](https://github.com/steven-brett-edwards/fleetwise-ai/actions/workflows/ci.yml)

> An AI-powered fleet maintenance assistant built with Angular, .NET, and Microsoft Semantic Kernel

## What is This?

FleetWise AI is a full-stack web application that helps fleet managers interact with their fleet data using natural language. It combines a traditional Angular + .NET application with Microsoft's Semantic Kernel to create an intelligent assistant that can query live fleet data and reference maintenance documentation.

A fleet manager can ask questions like:

- "Which vehicles are overdue for oil changes?"
- "What's the recommended brake inspection interval for a 2020 Ford F-150 with 60,000 miles?"
- "Show me all critical work orders"
- "Which vehicles have the highest lifetime maintenance costs?"

The AI answers these questions by querying a real database through Semantic Kernel function calling plugins and retrieving relevant fleet management documentation (maintenance procedures, safety policies, SOPs) through RAG (Retrieval-Augmented Generation) with vector embeddings.

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 18, TypeScript, Angular Material |
| Backend | ASP.NET Core, .NET 9, Entity Framework Core |
| AI Orchestration | Microsoft Semantic Kernel 1.74.0 |
| AI Abstractions | Microsoft.Extensions.AI (IChatClient) |
| Database | SQLite (dev) / SQL Server (prod) |
| LLM | Ollama (local/free) or Azure OpenAI (production) |
| Vector Store | In-Memory (Semantic Kernel) |
| Observability | .NET Aspire |

## Status

This project is under active development.

- [x] .NET solution structure with clean architecture
- [x] Domain entities and EF Core data model (vehicles, work orders, maintenance, parts)
- [x] Seed data representing a realistic 35-vehicle municipal fleet
- [x] REST API with 9 endpoints
- [x] Angular frontend scaffold with Material, routing, and services
- [x] Semantic Kernel plugins with function calling (12 functions across 4 plugins)
- [x] Chat orchestration service with conversation history
- [x] Sync and streaming (SSE) chat endpoints
- [x] Provider-swap architecture (Ollama / Azure OpenAI / OpenAI via config)
- [x] RAG pipeline with fleet management documentation (vector embeddings + semantic search)
- [x] 86 unit tests with 100% coverage on all API components
- [x] Angular chat UI with SSE streaming responses
- [x] Dashboard with fleet summary, overdue/upcoming maintenance
- [x] Vehicle list and detail views with filtering
- [x] Work order list and detail views
- [x] Mobile responsive layout (sidenav, dashboard grid, chat bubbles, list filter bars)
- [x] Frontend unit tests (128 tests, 100% coverage on services + components)
- [x] CI/CD pipeline (GitHub Actions: parallel backend + frontend jobs with coverage)

## Running Locally

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Ollama](https://ollama.com/) with a chat model pulled (e.g. `ollama pull qwen2.5:7b`)

### Backend

```bash
cd src/FleetWise.Api
dotnet run
```

The API starts at `http://localhost:5100`. On first run, EF Core automatically creates and seeds the SQLite database with a 35-vehicle municipal fleet.

### Frontend

```bash
cd src/fleetwise-client
npm install
npx ng serve
```

The Angular app starts at `http://localhost:4200` and proxies API requests to the backend.

### Ollama

Make sure Ollama is running (`ollama serve`) with both a chat model and an embedding model pulled:

```bash
ollama pull qwen2.5:7b        # chat model (tool calling)
ollama pull nomic-embed-text  # embedding model (for RAG document search)
```

The app defaults to `qwen2.5:7b` for chat and `nomic-embed-text` (768 dimensions) for embeddings. To use a different provider (Azure OpenAI, OpenAI), set `AiProvider` and the corresponding section in `appsettings.json`.

## Known Issues

**Angular 18 security advisories.** `npm audit` reports 43 vulnerabilities, almost all of which are in Angular 18 itself (XSS via SVG/i18n, XSRF via protocol-relative URLs) or in its build-tool transitives (`esbuild`, `vite`, `rollup`, `webpack`, `tar`, etc.). Fixes require an Angular major-version upgrade (18 -> 19+), which is deferred. The runtime framework CVEs require the app to render attacker-controlled SVG/i18n content or make cross-origin HTTP calls to attacker-influenced URLs -- FleetWise does neither. Build-tool advisories are dev-only and don't ship to production.

## Coming Next

**Python edition.** A parallel rewrite using FastAPI, LangGraph, and Anthropic Claude is in progress as a separate repository.

**Angular 19+ upgrade.** Clears the framework-level advisories listed above.

**Production polish.** Integration tests, error handling, and deployment configuration.

## License

This project is a portfolio demonstration and is not intended for production use.
