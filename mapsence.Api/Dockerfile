# Сборка из корня репозитория: docker build -f mapsence.Api/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Gps.Transport/Gps.Transport.csproj Gps.Transport/
COPY mapsence.Domain/mapsence.Domain.csproj mapsence.Domain/
COPY mapsence.Application/mapsence.Application.csproj mapsence.Application/
COPY mapsence.Infrastructure/mapsence.Infrastructure.csproj mapsence.Infrastructure/
COPY mapsence.Api/mapsence.Api.csproj mapsence.Api/
RUN dotnet restore mapsence.Api/mapsence.Api.csproj

COPY Gps.Transport/ Gps.Transport/
COPY mapsence.Domain/ mapsence.Domain/
COPY mapsence.Application/ mapsence.Application/
COPY mapsence.Infrastructure/ mapsence.Infrastructure/
COPY mapsence.Api/ mapsence.Api/
RUN dotnet publish mapsence.Api/mapsence.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "mapsence.Api.dll"]
