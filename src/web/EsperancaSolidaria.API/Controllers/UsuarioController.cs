using EsperancaSolidaria.Application.Commands.Usuarios.Handlers;
using EsperancaSolidaria.Application.Commands.Usuarios.Inputs;
using EsperancaSolidaria.Application.Commands.Usuarios.Results;
using EsperancaSolidaria.Application.Queries.Usuarios;
using EsperancaSolidaria.Application.Queries.Usuarios.Handlers;
using EsperancaSolidaria.Application.Queries.Usuarios.Results;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsperancaSolidaria.API.Controllers;

[ApiController]
[Authorize]
[Route("usuarios")]
public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IUsuarioCommandHandler _usuarioCommandHandler;
    private readonly IUsuarioQueryHandler _usuarioQueryHandler;
    private readonly IUserContext _userContext;

    public UsuarioController(ILogger<UsuarioController> logger,
        IUsuarioCommandHandler usuarioCommandHandler,
        IUsuarioQueryHandler usuarioQueryHandler,
        IUserContext userContext)
    {
        _logger = logger;
        _usuarioCommandHandler = usuarioCommandHandler;
        _usuarioQueryHandler = usuarioQueryHandler;
        _userContext = userContext;
    }

    /// <summary>
    /// Consulta usuários cadastrados no sistema com filtros opcionais.
    /// </summary>
    /// <remarks>
    /// Permite que gestores de ONG consultem a lista de usuários cadastrados.
    /// Os usuários são retornados paginados.
    /// </remarks>
    /// <param name="pagina">Número da página (padrão: 1)</param>
    /// <param name="tamanhoPagina">Quantidade de itens por página (padrão: 10, máximo: 100)</param>
    /// <param name="nome">Filtro opcional por nome completo do usuário</param>
    /// <param name="email">Filtro opcional por email do usuário</param>
    /// <response code="200">Retorna a lista paginada de usuários encontrados.</response>
    /// <response code="400">Parâmetros de paginação inválidos.</response>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpGet(Name = "ConsultarUsuarios")]
    [ProducesResponseType(typeof(PaginatedResult<UsuarioListaQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PaginatedResult<UsuarioListaQueryResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultarUsuarios([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10, 
        [FromQuery] string? nome = null, [FromQuery] string? email = null)
    {
        var query = new ConsultarUsuariosQuery(pagina, tamanhoPagina, nome, email);
        var resultado = await _usuarioQueryHandler.HandleAsync(query);

        return resultado is null
            ? BadRequest(new { Message = "Parâmetros de paginação inválidos." })
            : Ok(resultado);
    }

    /// <summary>
    /// Obtém os dados completos de um usuário pelo seu identificador.
    /// </summary>
    /// <remarks>
    /// Requer acesso de gestor. 
    /// Retorna os dados completos do usuário.
    /// </remarks>
    /// <param name="id">Identificador único do usuário.</param>
    /// <response code="200">Usuário encontrado com sucesso. Retorna os dados completos do usuário.</response>
    /// <response code="404">Usuário não encontrado.</response>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpGet("{id}", Name = "ObterUsuarioPorId")]
    [ProducesResponseType(typeof(UsuarioQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterUsuarioPorId([FromRoute] Guid id)
    {
        var query = new ObterUsuarioPorIdQuery(id);
        var resultado = await _usuarioQueryHandler.HandleAsync(query);

        if (resultado == null)
            return NotFound(new { Message = "Usuário não encontrado." });

        return Ok(resultado);
    }


    /// <summary>
    /// Cria um novo usuário no sistema.
    /// </summary>
    /// <remarks>
    /// Requer acesso de gestor. 
    /// É necessário informar nome, e-mail válido e uma senha que atenda aos critérios de segurança definidos: 
    /// mínimo de 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo.
    /// </remarks>
    /// <param name="command">Dados necessários para o registro do usuário.</param>
    /// <response code="201">Usuário criado com sucesso. Retorna os dados do usuário.</response>
    /// <response code="400">Requisição inválida ou senha incorreta.</response>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpPost(Name = "CriarUsuario")]
    [ProducesResponseType(typeof(CommandResult<CriarUsuarioResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioCommand command)
    {
        command.PreencherUsuario(_userContext.GetUserName());
        var resultado = await _usuarioCommandHandler.HandleAsync(command);

        return !resultado.IsValid 
            ? BadRequest(resultado) 
            : CreatedAtRoute("CriarUsuario", new { id = resultado.Data.Id }, resultado.Data);
    }

    /// <summary>
    /// Edita os dados de um usuário existente.
    /// </summary>
    /// <remarks>
    /// Requer acesso de gestor. 
    /// É necessário informar nome, cpf e perfil de acesso.
    /// A senha é opcional, mas se fornecida, deve atender aos critérios de segurança definidos: 
    /// mínimo de 8 caracteres, com pelo menos uma letra maiúscula, uma minúscula, um número e um símbolo.
    /// </remarks>
    /// <param name="command">Dados necessários para edição do usuário.</param>
    /// <response code="200">Usuário editado com sucesso. Retorna os dados do usuário.</response>
    /// <response code="400">Requisição inválida.</response>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpPut("{id}", Name = "EditarUsuario")]
    [ProducesResponseType(typeof(CommandResult<EditarUsuarioResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditarUsuario([FromRoute] Guid id, [FromBody] EditarUsuarioCommand command)
    {
        command.PreencherDadosComplementares(id, _userContext.GetUserName());
        var resultado = await _usuarioCommandHandler.HandleAsync(command);

        return !resultado.IsValid 
            ? BadRequest(resultado) 
            : Ok(resultado);
    }

    /// <summary>
    /// Remove um usuário do sistema.
    /// </summary>
    /// <remarks>
    /// Requer acesso de gestor. 
    /// Essa operação é irreversível e remove permanentemente o usuário da base de dados.
    /// </remarks>
    /// <param name="id">Identificador do usuário.</param>
    /// <response code="204">Usuário removido com sucesso.</response>
    /// <response code="400">Requisição inválida ou usuário não encontrado.</response>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpDelete("{id}", Name = "RemoverUsuario")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoverUsuario([FromRoute] Guid id)
    {
        var command = new RemoverUsuarioCommand(id, _userContext.GetUserName());
        var resultado = await _usuarioCommandHandler.HandleAsync(command);

        return !resultado.IsValid ? BadRequest(resultado) : NoContent();
    }
}