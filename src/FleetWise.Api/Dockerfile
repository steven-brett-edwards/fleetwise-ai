# syntax=docker/dockerfile:1

# ---- build stage ----------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution + props + project files first so restore can be cached
# independently of source changes.
COPY FleetWise.sln Directory.Build.props ./
COPY src/FleetWise.Api/FleetWise.Api.csproj src/FleetWise.Api/
COPY src/FleetWise.Domain/FleetWise.Domain.csproj src/FleetWise.Domain/
COPY src/FleetWise.Infrastructure/FleetWise.Infrastructure.csproj src/FleetWise.Infrastructure/
COPY tests/FleetWise.Api.Tests/FleetWise.Api.Tests.csproj tests/FleetWise.Api.Tests/
COPY tests/FleetWise.Infrastructure.Tests/FleetWise.Infrastructure.Tests.csproj tests/FleetWise.Infrastructure.Tests/

RUN dotnet restore src/FleetWise.Api/FleetWise.Api.csproj

# Copy the rest of the source and publish.
COPY src/ src/
RUN dotnet publish src/FleetWise.Api/FleetWise.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ---- runtime stage --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Working directory matches the source layout so that Program.cs's
# `Path.Combine(ContentRootPath, "..", "..", "data", "documents")` resolves
# to `/app/data/documents` inside the image.
WORKDIR /app/src/FleetWise.Api

COPY --from=build /app/publish ./

# Bake the RAG corpus into the image so the API has documents to ingest
# on startup. These live outside the volume mount point below.
COPY data/documents /app/data/documents

# SQLite DB lives on a volume mounted at /app/data/db. Create the directory
# so EF Core can create the file on first run even before anything writes.
RUN mkdir -p /app/data/db

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    ConnectionStrings__DefaultConnection="Data Source=/app/data/db/fleetwise.db"

ENTRYPOINT ["dotnet", "FleetWise.Api.dll"]
