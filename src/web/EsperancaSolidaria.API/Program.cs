using EsperancaSolidaria.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
BuilderExtension.Configure(builder);

var app = builder.Build();
ApplicationExtensions.Configure(app);

// Apply migrations and seed admin user on application startup
await app.SeedDatabaseAsync();

await app.RunAsync();