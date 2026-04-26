
Executas os comandos na pasta raiz do projeto

1 - Comando para criar uma nova migration
dotnet ef migrations add TabelaDoacao \
    -p src/core/EsperancaSolidaria.Infraestructure/ \
    -s src/web/EsperancaSolidaria.API/ \
    -c EsperancaSolidariaDbContext \
    -o Persistence/Migrations


2 - comando para executar migrations do projeto
dotnet ef database update \
    -p src/core/EsperancaSolidaria.Infraestructure/ \
    -s src/web/EsperancaSolidaria.API/ \
    -c EsperancaSolidariaDbContext 