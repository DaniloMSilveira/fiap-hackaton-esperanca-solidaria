using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsperancaSolidaria.Infra.Security.Models;

namespace EsperancaSolidaria.Infra.Security.Services
{
    public interface IIdentityService
    {
        Task<IdentityTokenResponse> Login(string email, string senha);
        Task<IdentityResponse> CriarUsuario(string nome, string email, string senha);
        Task<IdentityResponse> RemoverUsuario(string email);
        Task<IdentityResponse> AlterarSenha(string email, string senhaAtual, string novaSenha);
        Task<IdentityResponse> AlterarAcessos(string email, List<string> roles);
    }
}