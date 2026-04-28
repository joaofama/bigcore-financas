# Estágio de Execução (.NET 9)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

# Recebe a porta interna do docker-compose (.env)
ARG App_InternalPort
# Define a porta que o ASP.NET (Kestrel) deve usar internamente
ENV ASPNETCORE_HTTP_PORTS=$App_InternalPort

# Expõe a porta dinâmica
EXPOSE $ASPNETCORE_HTTP_PORTS

# Estágio de Construção
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos de projeto
COPY ["src/Financas.API/Financas.API.csproj", "src/Financas.API/"]
COPY ["src/Financas.Application/Financas.Application.csproj", "src/Financas.Application/"]
COPY ["src/Financas.Domain/Financas.Domain.csproj", "src/Financas.Domain/"]
COPY ["src/Financas.Infrastructure/Financas.Infrastructure.csproj", "src/Financas.Infrastructure/"]

# Restaura as dependências
RUN dotnet restore "./src/Financas.API/Financas.API.csproj"

# Copia todo o código fonte
COPY . .

# Compila o projeto da API
WORKDIR "/src/src/Financas.API"
RUN dotnet build "./Financas.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica os binários
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Financas.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Financas.API.dll"]