FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar o .csproj
COPY ["act-ms-transaction/act-ms-transaction.csproj", "act-ms-transaction/"]

# Restaurar dependências
RUN dotnet restore "./act-ms-transaction/act-ms-transaction.csproj"

# Copiar todos os arquivos
COPY ./act-ms-transaction ./act-ms-transaction

# Build
WORKDIR "/src/act-ms-transaction"
RUN dotnet build "act-ms-transaction.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "act-ms-transaction.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "act-ms-transaction.dll"]
