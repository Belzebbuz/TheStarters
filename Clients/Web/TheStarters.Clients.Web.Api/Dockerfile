FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Clients/Web/TheStarters.Clients.Web.Api/TheStarters.Clients.Web.Api.csproj", "Clients/Web/TheStarters.Clients.Web.Api/"]
RUN dotnet restore "Clients/Web/TheStarters.Clients.Web.Api/TheStarters.Clients.Web.Api.csproj"
COPY . .
WORKDIR "/src/Clients/Web/TheStarters.Clients.Web.Api"
RUN dotnet build "TheStarters.Clients.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TheStarters.Clients.Web.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheStarters.Clients.Web.Api.dll"]
