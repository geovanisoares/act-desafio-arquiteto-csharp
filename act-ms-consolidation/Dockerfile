FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar o .csproj
COPY ["act-ms-consolidation/act-ms-consolidation.csproj", "act-ms-consolidation/"]

# Restaurar dependências
RUN dotnet restore "./act-ms-consolidation/act-ms-consolidation.csproj"

# Copiar todos os arquivos
COPY ./act-ms-consolidation ./act-ms-consolidation

# Build
WORKDIR "/src/act-ms-consolidation"
RUN dotnet build "act-ms-consolidation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "act-ms-consolidation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "act-ms-consolidation.dll"]
