using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsperancaSolidaria.Infra.Security.Constants;
using EsperancaSolidaria.Infra.Security.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace EsperancaSolidaria.Infra.Security.Seeds
{
    public static class IdentitySeed
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles.ObterListaRoles())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminUser(UserManager<IdentityCustomUser> userManager, string email, string password)
        {
            await SeedUser(userManager,
                "Administrador",
                email,
                password,
                [
                    Roles.DOADOR,
                    Roles.GESTOR_ONG
                ]);
        }

        public static async Task SeedTestingData(UserManager<IdentityCustomUser> userManager)
        {
            await SeedUser(userManager,
                "Administrador",
                "admin@oes.com",
                "Admin123!",
                [
                    Roles.DOADOR,
                    Roles.GESTOR_ONG
                ]);
            await SeedUser(userManager,
                "Danilo",
                "danilo@oes.com",
                "Danilo123!",
                [
                    Roles.DOADOR
                ]);
        }

        #region AUXILIARES

        private static async Task SeedUser(UserManager<IdentityCustomUser> userManager,
            string nome,
            string email,
            string senha,
             List<string> roles)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityCustomUser
                {
                    Nome = nome,
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, senha);
                if (result.Succeeded)
                {
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }

        #endregion
    }
}