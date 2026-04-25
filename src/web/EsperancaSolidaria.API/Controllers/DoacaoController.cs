using EsperancaSolidaria.Application.Commands.Doacoes.Handlers;
using EsperancaSolidaria.Application.Commands.Doacoes.Inputs;
using EsperancaSolidaria.Application.Commands.Doacoes.Results;
using EsperancaSolidaria.Application.Queries.Doacoes;
using EsperancaSolidaria.Application.Queries.Doacoes.Handlers;
using EsperancaSolidaria.Application.Queries.Doacoes.Results;
using EsperancaSolidaria.Application.Security;
using EsperancaSolidaria.BuildingBlocks.Commands;
using EsperancaSolidaria.BuildingBlocks.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsperancaSolidaria.API.Controllers;

[ApiController]
[Authorize]
[Route("doacoes")]
public class DoacaoController : Controller
{
    private readonly ILogger<DoacaoController> _logger;
    private readonly IUserContext _userContext;
    private readonly IDoacaoCommandHandler _doacaoCommandHandler;
    private readonly IDoacaoQueryHandler _doacaoQueryHandler;

    public DoacaoController(ILogger<DoacaoController> logger,
        IUserContext userContext,
        IDoacaoCommandHandler doacaoCommandHandler,
        IDoacaoQueryHandler doacaoQueryHandler)
    {
        _logger = logger;
        _doacaoCommandHandler = doacaoCommandHandler;
        _doacaoQueryHandler = doacaoQueryHandler;
        _userContext = userContext;
    }

    /// <summary>
    /// Cria uma nova doação. Apenas usuários autenticados podem fazer doações.
    /// </summary>
    [HttpPost(Name = "CriarDoacao")]
    [ProducesResponseType(typeof(CommandResult<CriarDoacaoResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CriarDoacao([FromBody] CriarDoacaoCommand command)
    {
        if (!_userContext.Id.HasValue)
            return BadRequest(new { mensagem = "Usuário não autenticado." });

        command.PreencherDoadorId(_userContext.Id.Value);

        var resultado = await _doacaoCommandHandler.HandleAsync(command);

        return !resultado.IsValid
            ? BadRequest(resultado)
            : CreatedAtRoute("CriarDoacao", new { id = resultado.Data.Id }, resultado.Data);
    }

    /// <summary>
    /// Consulta as doações realizadas para uma campanha
    /// </summary>
    [Authorize(Roles = Roles.GestorONG)]
    [HttpGet(Name = "ConsultarDoacoes")]
    [ProducesResponseType(typeof(PaginatedResult<DoacaoResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultarDoacoes([FromQuery] ConsultarDoacoesQuery query)
    {
        var resultado = await _doacaoQueryHandler.HandleAsync(query);

        return resultado is null ? NoContent() : Ok(resultado);
    }
}
