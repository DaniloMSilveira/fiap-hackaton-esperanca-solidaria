namespace EsperancaSolidaria.BuildingBlocks.Commands;

/// <summary>
/// Contrato para manipuladores de comandos.
/// </summary>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
