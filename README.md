# FleetWise AI

> An AI-powered fleet maintenance assistant built with Angular, .NET, and Microsoft Semantic Kernel

> Under active construction!

## What is This?

FleetWise AI is a full-stack web application that helps fleet managers interact with their fleet data using natural language. It combines a traditional Angular + .NET application with Microsoft's Semantic Kernel to create an intelligent assistant that can query live fleet data and reference maintenance documentation.

A fleet manager can ask questions like:

- "Which vehicles are overdue for oil changes?"
- "What's the recommended brake inspection interval for a 2020 Ford F-150 with 60,000 miles?"
- "Show me all critical work orders"
- "Which vehicles have the highest lifetime maintenance costs?"

The AI answers these questions by querying a real database through Semantic Kernel function calling plugins and retrieving relevant maintenance documentation through RAG (Retrieval-Augmented Generation).

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
- [x] Semantic Kernel plugins with function calling (11 functions across 3 plugins)
- [x] Chat orchestration service with conversation history
- [x] Sync and streaming (SSE) chat endpoints
- [x] Provider-swap architecture (Ollama / Azure OpenAI / OpenAI via config)
- [x] 68 unit tests with 100% coverage on all API components
- [ ] RAG pipeline with maintenance documentation
- [x] Angular chat UI with SSE streaming responses
- [x] Dashboard with fleet summary, overdue/upcoming maintenance
- [x] Vehicle list and detail views with filtering
- [x] Work order list and detail views
- [ ] Mobile responsive layout
- [ ] Frontend unit tests
- [ ] RAG pipeline with maintenance documentation
- [ ] CI/CD pipeline

## Running Locally

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [Ollama](https://ollama.com/) with a chat model pulled (e.g. `ollama pull qwen2.5:3b`)

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

Make sure Ollama is running (`ollama serve`) with a chat model pulled:

```bash
ollama pull qwen2.5:3b
```

The app defaults to `qwen2.5:3b` via Ollama. To use a different provider (Azure OpenAI, OpenAI), set `AiProvider` and the corresponding section in `appsettings.json`.

## Coming Next

**Mobile responsive layout** -- sidenav collapses to hamburger menu on small screens, tables scroll horizontally.

**RAG (Retrieval-Augmented Generation)** -- a document ingestion pipeline for fleet manuals, SOPs, and warranty documentation. The LLM will be able to cite actual maintenance docs when answering questions, not just query database tables. Vector embeddings powered by Ollama's `nomic-embed-text` model.

**Production polish** -- Azure OpenAI provider swap, integration tests, error handling, and deployment configuration.

## License

This project is a portfolio demonstration and is not intended for production use.
