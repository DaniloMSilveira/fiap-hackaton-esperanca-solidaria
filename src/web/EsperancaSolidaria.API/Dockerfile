# Esta fase é usada para compilar  o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
USER root
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["SolidarityConnection.Api/SolidarityConnection.Api.csproj", "SolidarityConnection.Api/"]
COPY ["SolidarityConnection.Application/SolidarityConnection.Application.csproj", "SolidarityConnection.Application/"]
COPY ["SolidarityConnection.Domain/SolidarityConnection.Domain.csproj", "SolidarityConnection.Domain/"]
COPY ["SolidarityConnection.Infrastructure/SolidarityConnection.Infrastructure.csproj", "SolidarityConnection.Infrastructure/"]

# Restaura as dependências
RUN dotnet restore "SolidarityConnection.Api/SolidarityConnection.Api.csproj"

# Copia todo o código fonte
COPY . .

# Compila o projeto
WORKDIR "/src/SolidarityConnection.Api"
RUN dotnet build "SolidarityConnection.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica o projeto
RUN dotnet publish "SolidarityConnection.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Esta fase é usada na produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
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
ENTRYPOINT ["dotnet", "SolidarityConnection.Api.dll"]