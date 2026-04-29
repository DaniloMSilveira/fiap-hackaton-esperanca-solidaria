# Esta fase é usada para compilar  o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
USER root
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos de projeto mantendo a estrutura de diretórios
COPY ["buildingblocks/EsperancaSolidaria.BuildingBlocks/EsperancaSolidaria.BuildingBlocks.csproj", "buildingblocks/EsperancaSolidaria.BuildingBlocks/"]
COPY ["core/EsperancaSolidaria.Domain/EsperancaSolidaria.Domain.csproj", "core/EsperancaSolidaria.Domain/"]
COPY ["core/EsperancaSolidaria.Application/EsperancaSolidaria.Application.csproj", "core/EsperancaSolidaria.Application/"]
COPY ["core/EsperancaSolidaria.Infraestructure/EsperancaSolidaria.Infraestructure.csproj", "core/EsperancaSolidaria.Infraestructure/"]
COPY ["workers/EsperancaSolidaria.Worker.DoacaoRealizada/EsperancaSolidaria.Worker.DoacaoRealizada.csproj", "workers/EsperancaSolidaria.Worker.DoacaoRealizada/"]

# Restaura as dependências
RUN dotnet restore "workers/EsperancaSolidaria.Worker.DoacaoRealizada/EsperancaSolidaria.Worker.DoacaoRealizada.csproj"

# Copia todo o código fonte
COPY . .

# Compila o projeto
WORKDIR "/src/workers/EsperancaSolidaria.Worker.DoacaoRealizada"
RUN dotnet build "EsperancaSolidaria.Worker.DoacaoRealizada.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica o projeto
RUN dotnet publish "EsperancaSolidaria.Worker.DoacaoRealizada.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
USER root

# Configurando timezone para America/Sao_Paulo
RUN apk add --no-cache tzdata icu-libs \
    && cp /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime \
    && echo "America/Sao_Paulo" > /etc/timezone \
    && apk del tzdata

ENV TZ=America/Sao_Paulo
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY --from=build /app/publish .

RUN chown -R $APP_UID /app

EXPOSE 8080
EXPOSE 8081

USER $APP_UID
ENTRYPOINT ["dotnet", "EsperancaSolidaria.Worker.DoacaoRealizada.dll"]