# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy all the solution files and restore dependencies
COPY . .
WORKDIR /app/2Sport_BE
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# Stage 2: Serve the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "2Sport_BE.dll"]
