FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar o .csproj
COPY ["act-ms-auth/act-ms-auth.csproj", "act-ms-auth/"]

# Restaurar dependências
RUN dotnet restore "./act-ms-auth/act-ms-auth.csproj"

# Copiar todos os arquivos do projeto
COPY ./act-ms-auth ./act-ms-auth

# Build do projeto
WORKDIR "/src/act-ms-auth"
RUN dotnet build "act-ms-auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "act-ms-auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "act-ms-auth.dll"]
