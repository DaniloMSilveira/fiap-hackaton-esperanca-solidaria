using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsperancaSolidaria.API.Extensions;

public static class ApplicationExtensions
{
    public static void Configure(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
    }
}