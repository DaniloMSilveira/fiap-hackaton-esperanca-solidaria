using EsperancaSolidaria.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
BuilderExtension.Configure(builder);

var app = builder.Build();
ApplicationExtensions.Configure(app);

await app.RunAsync();