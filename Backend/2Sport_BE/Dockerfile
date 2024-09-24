FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["2Sport_BE.csproj", "2Sport_BE/"]
COPY ["2Sport_BE.Core/2Sport_BE.Core.csproj", "2Sport_BE.Core/"]
COPY ["2Sport_BE.Infrastructure/2Sport_BE.Infrastructure.csproj", "2Sport_BE.Infrastructure/"]
RUN dotnet restore "2Sport_BE/2Sport_BE.csproj"
COPY . .
WORKDIR "/src/2Sport_BE"
RUN dotnet build "2Sport_BE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "2Sport_BE.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "2Sport_BE.dll"]