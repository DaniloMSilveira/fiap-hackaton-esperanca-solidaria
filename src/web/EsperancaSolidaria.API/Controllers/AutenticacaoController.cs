using EsperancaSolidaria.Application.Commands.Autenticacao.Handlers;
using EsperancaSolidaria.Application.Commands.Autenticacao.Inputs;
using EsperancaSolidaria.Application.Commands.Autenticacao.Results;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.BuildingBlocks.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsperancaSolidaria.Users.API.Controllers;

[ApiController]
[Authorize]
[Route("autenticacao")]
public class AutenticacaoController : Controller
{
    private readonly ILogger<AutenticacaoController> _logger;
    private readonly IAutenticacaoCommandHandler _autenticacaoCommandHandler;
    private readonly IUserContext _userContext;

    public AutenticacaoController(ILogger<AutenticacaoController> logger, IAutenticacaoCommandHandler autenticacaoCommandHandler, IUserContext userContext)
    {
        _logger = logger;
        _autenticacaoCommandHandler = autenticacaoCommandHandler;
        _userContext = userContext;
    }

    /// <summary>
    /// Registrar um novo usuário no sistema.
    /// </summary>
    /// <remarks>
    /// É necessário informar nome completo, e-mail válido, cpf válido e uma senha que atenda aos critérios de segurança definidos: 
    /// mínimo de 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo.
    /// </remarks>
    /// <param name="command">Dados necessários para o registro do usuário.</param>
    /// <response code="201">Usuário criado com sucesso. Retorna os dados do usuário.</response>
    /// <response code="400">Requisição inválida ou senha incorreta.</response>
    [AllowAnonymous]
    [HttpPost("registrar", Name = "RegistrarUsuario")]
    [ProducesResponseType(typeof(CommandResult<RegistrarUsuarioResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarUsuario([FromBody] RegistrarUsuarioCommand command)
    {
        var resultado = await _autenticacaoCommandHandler.HandleAsync(command);

        return !resultado.IsValid ? BadRequest(resultado) : Ok(resultado);
    }

    /// <summary>
    /// Realizar o login no sistema.
    /// </summary>
    /// <remarks>
    /// É necessário informar o e-mail e senha validos.
    /// </remarks>
    /// <param name="command">Dados necessários para o login do usuário.</param>
    /// <response code="201">Usuário autenticado com sucesso. Retorna o token de acesso.</response>
    /// <response code="400">Dados de acesso inválidos.</response>
    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    [ProducesResponseType(typeof(CommandResult<LoginResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var resultado = await _autenticacaoCommandHandler.HandleAsync(command);

        return !resultado.IsValid ? BadRequest(resultado) : Ok(resultado);
    }

    /// <summary>
    /// Obtém informações do usuário autenticado.
    /// </summary>
    /// <response code="200">Retorna as informações do usuário logado.</response>
    [HttpGet("perfil", Name = "ObterPerfilUsuario")]
    [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
    public IActionResult ObterUsuarioLogado()
    {
        var userProfile = _userContext.GetProfile();

        return Ok(userProfile);
    }

    /// <summary>
    /// Altera a senha do usuário autenticado.
    /// </summary>
    /// <remarks>
    /// O usuário deve estar autenticado para realizar esta operação.  
    /// É necessário informar a senha atual e a nova senha, que deve atender aos critérios de segurança definidos: 
    /// mínimo de 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo.
    /// </remarks>
    /// <param name="command">Dados necessários para alteração da senha.</param>
    /// <response code="204">Senha alterada com sucesso.</response>
    /// <response code="400">Requisição inválida ou senha incorreta.</response>
    [Authorize]
    [HttpPatch("alterar-senha", Name = "AlterarSenha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaCommand command)
    {
        command.PreencherUsuario(_userContext.GetUserName() ?? string.Empty);
        var resultado = await _autenticacaoCommandHandler.HandleAsync(command);

        return !resultado.IsValid ? BadRequest(resultado) : NoContent();
    }
}