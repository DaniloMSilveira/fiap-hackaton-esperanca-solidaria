using EsperancaSolidaria.Worker.DoacaoRealizada.Extensions;

var builder = Host.CreateApplicationBuilder(args);
BuilderExtension.Configure(builder);

var host = builder.Build();
host.Run();
