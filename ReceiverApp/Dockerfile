# Сборка из корня репозитория: docker build -f ReceiverApp/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Gps.Transport/Gps.Transport.csproj Gps.Transport/
COPY ReceiverApp/ReceiverApp.csproj ReceiverApp/
COPY protos/ protos/
RUN dotnet restore ReceiverApp/ReceiverApp.csproj

COPY Gps.Transport/ Gps.Transport/
COPY ReceiverApp/ ReceiverApp/
RUN dotnet publish ReceiverApp/ReceiverApp.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ReceiverApp.dll"]
