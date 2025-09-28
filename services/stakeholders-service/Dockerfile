# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["StakeholdersService.csproj", "./"]
RUN dotnet restore "StakeholdersService.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "StakeholdersService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "StakeholdersService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

EXPOSE 80
ENTRYPOINT ["dotnet", "StakeholdersService.dll"]