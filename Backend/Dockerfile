# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["./Backend/2Sport_BE/2Sport_BE.csproj", "2Sport_BE/"]
COPY ["./Backend/2Sport_BE.Core/2Sport_BE.Core.csproj", "2Sport_BE.Core/"]
COPY ["./Backend/2Sport_BE.Infrastructure/2Sport_BE.Infrastructure.csproj", "2Sport_BE.Infrastructure/"]
RUN dotnet restore "2Sport_BE/2Sport_BE.csproj"

# Copy the full source code
COPY . .
WORKDIR "/src/2Sport_BE"

# Publish the application
RUN dotnet publish "2Sport_BE.csproj" -c Release -o /app/publish

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "2Sport_BE.dll"]
