using EsperancaSolidaria.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
BuilderExtension.Configure(builder);

var app = builder.Build();
ApplicationExtensions.Configure(app);

// Seed admin user on application startup
await app.SeedAdminUserAsync();

await app.RunAsync();