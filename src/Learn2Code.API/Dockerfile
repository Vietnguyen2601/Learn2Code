# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY Learn2Code.Common/Learn2Code.Domain.csproj Learn2Code.Common/
COPY Learn2Code.Repositories/Learn2Code.Infrastructure.csproj Learn2Code.Repositories/
COPY Learn2Code.Services/Learn2Code.Application.csproj Learn2Code.Services/
COPY Learn2Code.API/Learn2Code.API.csproj Learn2Code.API/

RUN dotnet restore Learn2Code.API/Learn2Code.API.csproj

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish Learn2Code.API/Learn2Code.API.csproj -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Learn2Code.API.dll"]
