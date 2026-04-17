
comando para executar migrations do projeto

cd src/core/EsperancaSolidaria.Infraestructure/
dotnet ef database update -s ../../web/EsperancaSolidaria.API/ -c EsperancaSolidariaDbContext
