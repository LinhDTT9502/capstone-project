# Use the ASP.NET Core runtime image as the base image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["2Sport_BE/2Sport_BE.csproj", "2Sport_BE/"]
COPY ["2Sport_BE.Core/2Sport_BE.Core.csproj", "2Sport_BE.Core/"]
COPY ["2Sport_BE.Infrastructure/2Sport_BE.Infrastructure.csproj", "2Sport_BE.Infrastructure/"]
RUN dotnet restore "2Sport_BE/2Sport_BE.csproj"

# Copy the rest of the source code
COPY . .

# Build the main project
WORKDIR /src/2Sport_BE
RUN dotnet build "2Sport_BE.csproj" -c Release -o /app/build

# Publish the main project
FROM build AS publish
RUN dotnet publish "2Sport_BE.csproj" -c Release -o /app/publish

# Final stage: set up the runtime environment
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "2Sport_BE.dll"]



