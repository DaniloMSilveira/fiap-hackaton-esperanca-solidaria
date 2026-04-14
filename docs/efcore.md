
comando para executar migrations do projeto

cd src/core/EsperancaSolidaria.Infra.Data/
dotnet ef database update -s ../../web/EsperancaSolidaria.API/ -c EsperancaSolidariaDbContext
