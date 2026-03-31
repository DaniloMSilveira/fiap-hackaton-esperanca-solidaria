using System.Reflection;
using OpenTelemetry.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EsperancaSolidaria.Infra.Security.Configurations;
using Microsoft.AspNetCore.Identity;
using EsperancaSolidaria.Infra.Security.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using EsperancaSolidaria.Infra.Security.Contexts;
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

        builder.Services.AddDataContexts(builder.Configuration);
        builder.Services.AddServices(builder.Configuration);
        // builder.Services.AddIdentityAuthentication(builder.Configuration);
        builder.Services.AddCustomSwagger();
        builder.Services.AddCustomMetrics();
    }

    private static IServiceCollection AddDataContexts(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddDbContext<IdentityDataContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("FCG")));

        // services.AddDbContext<FCGDataContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("FCG")));

        // services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddScoped<IUserContext, UserContext>();

        // services.AddScoped<IIdentityService, IdentityService>();

        // services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        // services.AddScoped<IUsuarioAppService, UsuarioAppService>();
        // services.AddScoped<IAutenticacaoAppService, AutenticacaoAppService>();

        // services.AddScoped<IJogoRepository, JogoRepository>();
        // services.AddScoped<IJogoService, JogoService>();
        // services.AddScoped<IJogoAppService, JogoAppService>();

        // services.AddScoped<IPromocaoRepository, PromocaoRepository>();
        // services.AddScoped<IPromocaoService, PromocaoService>();
        // services.AddScoped<IPromocaoAppService, PromocaoAppService>();

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

    private static IServiceCollection AddCustomMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddPrometheusExporter();
            });

        return services;
    }

    public static void AddIdentityAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtAppSettingOptions = configuration.GetSection("Jwt");
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("Jwt:Key").Value));

        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtAppSettingOptions[nameof(JwtOptions.Issuer)];
            options.Audience = jwtAppSettingOptions[nameof(JwtOptions.Audience)];
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            options.AccessTokenExpiration = int.Parse(jwtAppSettingOptions[nameof(JwtOptions.AccessTokenExpiration)] ?? "0");
            options.RefreshTokenExpiration = int.Parse(jwtAppSettingOptions[nameof(JwtOptions.RefreshTokenExpiration)] ?? "0");
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
        });

        services.AddDefaultIdentity<IdentityCustomUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityDataContext>()
            .AddDefaultTokenProviders();

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtAppSettingOptions[nameof(JwtOptions.Issuer)],

            ValidateAudience = true,
            ValidAudience = jwtAppSettingOptions[nameof(JwtOptions.Audience)],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => 
        {
            options.TokenValidationParameters = tokenValidationParameters;
        });
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