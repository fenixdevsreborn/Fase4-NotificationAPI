FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files first for better layer caching
COPY NotificationsAPI.sln ./
COPY src/Shared/Shared.csproj ./Shared/
COPY src/Shared.Contracts/Shared.Contracts.csproj ./Shared.Contracts/
COPY src/NotificationsAPI/NotificationsAPI.Worker.csproj ./NotificationsAPI/
COPY src/NotificationsAPI.Application/NotificationsAPI.Application.csproj ./NotificationsAPI.Application/
COPY src/NotificationsAPI.Domain/NotificationsAPI.Domain.csproj ./NotificationsAPI.Domain/
COPY src/NotificationsAPI.Infrastructure/NotificationsAPI.Infrastructure.csproj ./NotificationsAPI.Infrastructure/
COPY src/NotificationsAPI.Application.Tests/NotificationsAPI.Application.Tests.csproj ./NotificationsAPI.Application.Tests/
COPY src/NotificationsAPI.Domain.Tests/NotificationsAPI.Domain.Tests.csproj ./NotificationsAPI.Domain.Tests/

# Restore dependencies
RUN dotnet restore "NotificationsAPI/NotificationsAPI.Worker.csproj"

# Copy remaining source code
COPY src/. .

# Build and publish
RUN dotnet publish "NotificationsAPI/NotificationsAPI.Worker.csproj" -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /out .
ENTRYPOINT ["dotnet", "NotificationsAPI.Worker.dll"]