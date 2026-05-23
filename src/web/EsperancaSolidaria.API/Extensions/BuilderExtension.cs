using System.Reflection;
using OpenTelemetry.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EsperancaSolidaria.Infraestructure.Persistence.Contexts;
using EsperancaSolidaria.Domain.Interfaces.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.Repositories;
using EsperancaSolidaria.Infraestructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EsperancaSolidaria.BuildingBlocks.Persistence;
using EsperancaSolidaria.BuildingBlocks.Events;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.Infraestructure.Security;
using EsperancaSolidaria.Application.Commands.Autenticacao.Handlers;
using EsperancaSolidaria.Application.Commands.Usuarios.Handlers;
using EsperancaSolidaria.Application.Queries.Usuarios.Handlers;
using EsperancaSolidaria.Application.Commands.Campanhas.Handlers;
using EsperancaSolidaria.Application.Queries.Campanhas.Handlers;
using EsperancaSolidaria.Application.Commands.Doacoes.Handlers;
using EsperancaSolidaria.Application.Queries.Doacoes.Handlers;
using EsperancaSolidaria.BuildingBlocks.Messaging;
using EsperancaSolidaria.Infraestructure.Messaging;
using EsperancaSolidaria.Infraestructure.Persistence.EventSourcing;
using EsperancaSolidaria.Infraestructure.Persistence.DomainEvents;
using MongoDB.Driver;
using EsperancaSolidaria.BuildingBlocks.EventSourcing;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace EsperancaSolidaria.API.Extensions;

public static class BuilderExtension
{
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHealthChecks();
        builder.Services.AddAuthentication(builder.Configuration);
        builder.Services.AddDataContexts(builder.Configuration, builder.Environment);
        builder.Services.AddEventSourcing(builder.Configuration);
        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddCustomSwagger();
        builder.Services.AddCustomMetrics(builder.Configuration);
        builder.Services.AddCustomLogging(builder);
    }

    private static IServiceCollection AddDataContexts(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddDbContext<EsperancaSolidariaDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("EsperancaSolidaria"));
            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Command Handlers
        services.AddScoped<IAutenticacaoCommandHandler, AutenticacaoCommandHandler>();
        services.AddScoped<IUsuarioCommandHandler, UsuarioCommandHandler>();
        services.AddScoped<ICampanhaCommandHandler, CampanhaCommandHandler>();
        services.AddScoped<IDoacaoCommandHandler, DoacaoCommandHandler>();

        // Query Handlers
        services.AddScoped<IUsuarioQueryHandler, UsuarioQueryHandler>();
        services.AddScoped<ICampanhaQueryHandler, CampanhaQueryHandler>();
        services.AddScoped<IDoacaoQueryHandler, DoacaoQueryHandler>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Domain Events
        services.AddScoped<IDomainEventService, DomainEventService>();

        // Repositories
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICampanhaRepository, CampanhaRepository>();
        services.AddScoped<IDoacaoRepository, DoacaoRepository>();

        // Message Bus
        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

        // RabbitMQ Configuration
        services.Configure<RabbitMqOptions>(
            configuration.GetSection("RabbitMqOptions"));

        // Security
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }

    private static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    private static IServiceCollection AddCustomMetrics(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName =
            configuration["Observability:ServiceName"]
            ?? "esperanca-solidaria-api";

        var tempoEndpoint =
            configuration["Observability:TempoOtlpEndpoint"]
            ?? throw new InvalidOperationException(
                "Tempo endpoint not configured");

        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddPrometheusExporter();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: serviceName,
                                serviceVersion: "1.0.0"))

                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;

                        options.Filter = httpContext =>
                        {
                            var path = httpContext.Request.Path;

                            return !path.StartsWithSegments("/metrics")
                                && !path.StartsWithSegments("/health")
                                && !path.StartsWithSegments("/swagger")
                                && !path.StartsWithSegments("/loki/api/v1/push");
                        };
                    })

                    .AddHttpClientInstrumentation()

                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.EnrichWithIDbCommand =
                            (activity, command) =>
                            {
                                activity.SetTag(
                                    "db.statement",
                                    command.CommandText);
                            };
                    })

                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(tempoEndpoint);
                    });
            });

        return services;
    }

    private static IServiceCollection AddEventSourcing(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar conventions do MongoDB
        MongoDbConventions.Configure();

        // Configurar opções do MongoDB
        services.Configure<MongoDbOptions>(configuration.GetSection("MongoDb"));

        // Registrar MongoDB client como singleton
        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = configuration.GetSection("MongoDb").Get<MongoDbOptions>() 
                ?? throw new InvalidOperationException("MongoDB configuration is missing");
            return new MongoClient(options.ConnectionString);
        });

        // Registrar MongoDB database como singleton
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var options = configuration.GetSection("MongoDb").Get<MongoDbOptions>()
                ?? throw new InvalidOperationException("MongoDB configuration is missing");
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        // Registrar Event Store
        services.AddScoped<IEventStore, MongoEventStore>();

        return services;
    }

    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured.");

        // JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    public static IServiceCollection AddCustomLogging(
        this IServiceCollection services,
        WebApplicationBuilder builder)
    {
        var lokiUrl = builder.Configuration["Observability:LokiUrl"]
            ?? throw new InvalidOperationException("Loki URL is not configured");

        Serilog.Debugging.SelfLog.Enable(msg =>
        {
            Console.WriteLine(msg);
        });

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.GrafanaLoki(
                lokiUrl,
                labels:
                [
                    new LokiLabel
                    {
                        Key = "app",
                        Value = "esperanca-solidaria-api"
                    },
                    new LokiLabel
                    {
                        Key = "environment",
                        Value = builder.Environment.EnvironmentName
                    }
                ])
            .CreateLogger();

        builder.Host.UseSerilog();

        return services;
    }

    // // Logs
    // builder.Services.AddSerilog(new LoggerConfiguration()
    //     .WriteTo.Console()
    //     .WriteTo.GrafanaLoki(
    //         builder.Configuration["Loki:Uri"]!,
    //         new List<LokiLabel>()
    //         {
    //             new()
    //             {
    //                 Key = "service_name",
    //                 Value = OpenTelemetryExtensions.ServiceName
    //             },
    //             new()
    //             {
    //                 Key = "using_database",
    //                 Value = "true"
    //             }
    //         })
    //     .Enrich.WithSpan(new SpanOptions() { IncludeOperationName = true, IncludeTags = true })
    //     .CreateLogger());

    // // Metrics and Tracing
    // builder.Services.AddOpenTelemetry()
    //     .WithMetrics(builder =>
    //     {
    //         builder
    //             .AddAspNetCoreInstrumentation()
    //             .AddHttpClientInstrumentation()
    //             .AddPrometheusExporter();
    //     })
    //     .WithTracing((traceBuilder) =>
    //     {
    //         traceBuilder
    //             .AddSource(OpenTelemetryExtensions.ServiceName)
    //             .SetResourceBuilder(
    //                 ResourceBuilder.CreateDefault()
    //                     .AddService(serviceName: OpenTelemetryExtensions.ServiceName,
    //                         serviceVersion: OpenTelemetryExtensions.ServiceVersion))
    //             .AddAspNetCoreInstrumentation()
    //             .AddSqlClientInstrumentation()
    //             .AddOtlpExporter()
    //             .AddConsoleExporter();
    //     });
}