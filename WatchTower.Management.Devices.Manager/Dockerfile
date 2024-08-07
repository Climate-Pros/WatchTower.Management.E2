FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["WatchTower.Management.Devices.E2/WatchTower.Management.Devices.E2.csproj", "WatchTower.Management.Devices.E2/"]
COPY ["WatchTower.Management.Devices.E2.ServiceInterface/WatchTower.Management.Devices.E2.ServiceInterface.csproj", "WatchTower.Management.Devices.E2.ServiceInterface/"]
COPY ["WatchTower.Management.Devices.E2.ServiceModel/WatchTower.Management.Devices.E2.ServiceModel.csproj", "WatchTower.Management.Devices.E2.ServiceModel/"]
RUN dotnet restore "WatchTower.Management.Devices.E2/WatchTower.Management.Devices.E2.csproj"
COPY . .
WORKDIR "/src/WatchTower.Management.Devices.E2"
RUN dotnet build "WatchTower.Management.Devices.E2.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WatchTower.Management.Devices.E2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WatchTower.Management.Devices.E2.dll"]
