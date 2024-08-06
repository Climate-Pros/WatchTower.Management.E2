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
COPY ["WatchTowerManagementE2/WatchTowerManagementE2.csproj", "WatchTowerManagementE2/"]
COPY ["WatchTowerManagementE2.ServiceInterface/WatchTowerManagementE2.ServiceInterface.csproj", "WatchTowerManagementE2.ServiceInterface/"]
COPY ["WatchTowerManagementE2.ServiceModel/WatchTowerManagementE2.ServiceModel.csproj", "WatchTowerManagementE2.ServiceModel/"]
RUN dotnet restore "WatchTowerManagementE2/WatchTowerManagementE2.csproj"
COPY . .
WORKDIR "/src/WatchTowerManagementE2"
RUN dotnet build "WatchTowerManagementE2.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WatchTowerManagementE2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WatchTowerManagementE2.dll"]
