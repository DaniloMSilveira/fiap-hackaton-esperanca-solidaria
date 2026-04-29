using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsperancaSolidaria.API.Middlewares;

namespace EsperancaSolidaria.API.Extensions;

public static class ApplicationExtensions
{
    public static void Configure(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Esperança Solidária v1");
            c.RoutePrefix = "swagger";
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCustomMiddlewares();
        app.MapControllers();
        app.MapPrometheusScrapingEndpoint();
    }

    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}