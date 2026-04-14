using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsperancaSolidaria.API.Extensions;

public static class ApplicationExtensions
{
    public static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Esperança Solidária v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}