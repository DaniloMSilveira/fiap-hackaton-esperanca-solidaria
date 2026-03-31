using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EsperancaSolidaria.Infra.Security.Configurations;
using EsperancaSolidaria.Infra.Security.Constants;
using EsperancaSolidaria.Infra.Security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EsperancaSolidaria.Infra.Security.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<IdentityCustomUser> _signInManager;
        private readonly UserManager<IdentityCustomUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(SignInManager<IdentityCustomUser> signInManager,
                               UserManager<IdentityCustomUser> userManager,
                               IOptions<JwtOptions> jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<IdentityTokenResponse> Login(string email, string senha)
        {
            var result = await _signInManager.PasswordSignInAsync(email, senha, false, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut || result.IsNotAllowed)
                    return new IdentityTokenResponse("Conta bloqueada.");
                else if (result.RequiresTwoFactor)
                    return new IdentityTokenResponse("É necessário realizar o duplo fator de autenticação.");
                else
                    return new IdentityTokenResponse("Credenciais inválidas.");
            }

            return await GerarCredenciais(email);
        }

        public async Task<IdentityResponse> CriarUsuario(string nome, string email, string senha)
        {
            var identityUser = new IdentityCustomUser
            {
                Nome = nome,
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, senha);
            if (!result.Succeeded)
                return new IdentityResponse(false, result.Errors.Select(r => r.Description).ToList());

            await _userManager.SetLockoutEnabledAsync(identityUser, false);
            await _userManager.AddToRoleAsync(identityUser, Roles.DOADOR);

            return new IdentityResponse(true);
        }

        public async Task<IdentityResponse> RemoverUsuario(string email)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
                return new IdentityResponse("Usuário não encontrado.");

            var resultado = await _userManager.DeleteAsync(usuario);
            if (!resultado.Succeeded)
            {
                var errors = resultado.Errors.Select(e => e.Description).ToList();
                return new IdentityResponse(false, errors);
            }

            return new IdentityResponse(true);
        }

        public async Task<IdentityResponse> AlterarSenha(string email, string senhaAtual, string novaSenha)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
                return new IdentityResponse("Usuário não encontrado.");

            var resultado = await _userManager.ChangePasswordAsync(usuario, senhaAtual, novaSenha);
            if (!resultado.Succeeded)
            {
                var errors = resultado.Errors.Select(e => e.Description).ToList();
                if (errors.Contains("Incorrect password."))
                    return new IdentityResponse("Senha incorreta.");
                else
                    return new IdentityResponse("Não foi possível alterar a senha. Tente novamente mais tarde.");
            }

            return new IdentityResponse(true);
        }

        public async Task<IdentityResponse> AlterarAcessos(string email, List<string> roles)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null)
                return new IdentityResponse("Usuário não encontrado.");

            var rolesAtuais = await _userManager.GetRolesAsync(usuario);
            if (rolesAtuais.Any())
            {
                var resultadoRemoveRoles = await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);
                if (!resultadoRemoveRoles.Succeeded)
                    return new IdentityResponse(false, resultadoRemoveRoles.Errors.Select(r => r.Description).ToList());
            }

            var resultado = await _userManager.AddToRolesAsync(usuario, roles);
            if (!resultado.Succeeded)
                return new IdentityResponse(false, resultado.Errors.Select(r => r.Description).ToList());

            return new IdentityResponse(true);
        }

        #region PRIVATE

        private async Task<IdentityTokenResponse> GerarCredenciais(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: true);
            var refreshTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: false);

            var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
            var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

            var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            var refreshToken = GerarToken(refreshTokenClaims, dataExpiracaoRefreshToken);

            return new IdentityTokenResponse(accessToken, refreshToken);
        }

        private string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: dataExpiracao,
                signingCredentials: _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<IList<Claim>> ObterClaims(IdentityCustomUser user, bool adicionarClaimsUsuario)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Nome),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString())
            };

            if (adicionarClaimsUsuario)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                claims.AddRange(userClaims);

                foreach (var role in roles)
                    claims.Add(new Claim("role", role));
            }

            return claims;
        }

        #endregion
    }
}