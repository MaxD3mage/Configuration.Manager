FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Configuration.Manager.Web/Configuration.Manager.Web.csproj", "Configuration.Manager.Web/"]
COPY ["Configuration.Manager.BusinessLogic/Configuration.Manager.BusinessLogic.csproj", "Configuration.Manager.BusinessLogic/"]
RUN dotnet restore "Configuration.Manager.Web/Configuration.Manager.Web.csproj"
COPY . .
WORKDIR "/src/Configuration.Manager.Web"
RUN dotnet build "./Configuration.Manager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./Configuration.Manager.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configuration.Manager.Web.dll"]
