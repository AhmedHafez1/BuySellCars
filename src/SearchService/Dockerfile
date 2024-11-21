# Use the official .NET SDK image to build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

# Copy the main microservice project files
COPY src/SearchService/SearchService.csproj src/SearchService/
# Copy the library project files
COPY src/BuildingBlocks/BuildingBlocks.csproj src/BuildingBlocks/

# Restore dependencies for all projects
RUN dotnet restore src/SearchService/SearchService.csproj

# Copy the remaining source code into the container
COPY . .

# Build and publish the application
WORKDIR /app/src/SearchService
RUN dotnet publish -c Release -o /app/publish

# Use the runtime-only image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=base /app/publish .

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "SearchService.dll"]
