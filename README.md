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
- [ ] Semantic Kernel plugins with function calling
- [ ] RAG pipeline with maintenance documentation
- [ ] AI chat interface with streaming
- [ ] Dashboard and vehicle management UI
- [ ] Unit and integration tests
- [ ] CI/CD pipeline
- [ ] Documentation and architecture diagrams

## License

This project is a portfolio demonstration and is not intended for production use.
