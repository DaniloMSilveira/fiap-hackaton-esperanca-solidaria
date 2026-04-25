using EsperancaSolidaria.Application.Commands.Campanhas.Handlers;
using EsperancaSolidaria.Application.Commands.Campanhas.Inputs;
using EsperancaSolidaria.Application.Commands.Campanhas.Results;
using EsperancaSolidaria.Application.Queries.Campanhas;
using EsperancaSolidaria.Application.Queries.Campanhas.Handlers;
using EsperancaSolidaria.Application.Queries.Campanhas.Results;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsperancaSolidaria.API.Controllers;

[ApiController]
[Authorize]
[Route("campanhas")]
public class CampanhaController : Controller
{
    private readonly ILogger<CampanhaController> _logger;
    private readonly ICampanhaCommandHandler _campanhaCommandHandler;
    private readonly ICampanhaQueryHandler _campanhaQueryHandler;
    private readonly IUserContext _userContext;

    public CampanhaController(ILogger<CampanhaController> logger,
        ICampanhaCommandHandler campanhaCommandHandler,
        ICampanhaQueryHandler campanhaQueryHandler,
        IUserContext userContext)
    {
        _logger = logger;
        _campanhaCommandHandler = campanhaCommandHandler;
        _campanhaQueryHandler = campanhaQueryHandler;
        _userContext = userContext;
    }

    /// <summary>
    /// Obtém todas as campanhas ativas (endpoint público, sem autenticação).
    /// </summary>
    [AllowAnonymous]
    [HttpGet("ativas", Name = "ConsultarCampanhasAtivas")]
    [ProducesResponseType(typeof(PaginatedResult<CampanhaPublicaResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PaginatedResult<CampanhaPublicaResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultarCampanhasAtivas([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var query = new ConsultarCampanhasAtivasQuery(pagina, tamanhoPagina);
        var resultado = await _campanhaQueryHandler.HandleAsync(query);

        return Ok(resultado);
    }

    /// <summary>
    /// Obtém todas as campanhas (apenas usuários com role GestorONG).
    /// </summary>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpGet(Name = "ConsultarCampanhas")]
    [ProducesResponseType(typeof(PaginatedResult<CampanhaListaResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PaginatedResult<CampanhaListaResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultarCampanhas([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var query = new ConsultarCampanhasQuery(pagina, tamanhoPagina, null);
        var resultado = await _campanhaQueryHandler.HandleAsync(query);

        return Ok(resultado);
    }

    /// <summary>
    /// Obtém os dados de uma campanha específica.
    /// </summary>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpGet("{id}", Name = "ObterCampanhaPorId")]
    [ProducesResponseType(typeof(CampanhaDetalhesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterCampanhaPorId([FromRoute] Guid id)
    {
        var query = new ObterCampanhaPorIdQuery(id);
        var resultado = await _campanhaQueryHandler.HandleAsync(query);

        if (resultado is null)
            return NotFound();

        return Ok(resultado);
    }

    /// <summary>
    /// Cria uma nova campanha.
    /// </summary>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpPost(Name = "CriarCampanha")]
    [ProducesResponseType(typeof(CommandResult<CriarCampanhaResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarCampanha([FromBody] CriarCampanhaCommand command)
    {
        var resultado = await _campanhaCommandHandler.HandleAsync(command);

        return !resultado.IsValid
            ? BadRequest(resultado)
            : CreatedAtRoute("ObterCampanhaPorId", new { id = resultado.Data.Id }, resultado.Data);
    }

    /// <summary>
    /// Edita uma campanha existente.
    /// </summary>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpPut("{id}", Name = "EditarCampanha")]
    [ProducesResponseType(typeof(CommandResult<EditarCampanhaResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditarCampanha([FromRoute] Guid id, [FromBody] EditarCampanhaCommand command)
    {
        command.PreencherDadosComplementares(id, _userContext.GetUserName());
        var resultado = await _campanhaCommandHandler.HandleAsync(command);

        return !resultado.IsValid
            ? BadRequest(resultado)
            : Ok(resultado);
    }

    /// <summary>
    /// Altera o status de uma campanha existente.
    /// </summary>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpPatch("{id}/alterar-status", Name = "AlterarStatusCampanha")]
    [ProducesResponseType(typeof(CommandResult<EditarCampanhaResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlterarStatusCampanha([FromRoute] Guid id, [FromBody] AlterarStatusCampanhaCommand command)
    {
        command.PreencherDadosComplementares(id, _userContext.GetUserName());
        var resultado = await _campanhaCommandHandler.HandleAsync(command);

        return !resultado.IsValid
            ? BadRequest(resultado)
            : Ok(resultado);
    }

    /// <summary>
    /// Remover uma campanha.
    /// </summary>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpDelete("{id}", Name = "RemoverCampanha")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoverCampanha([FromRoute] Guid id)
    {
        var command = new RemoverCampanhaCommand(id, _userContext.GetUserName());
        var resultado = await _campanhaCommandHandler.HandleAsync(command);

        return !resultado.IsValid ? BadRequest(resultado) : NoContent();
    }
}
