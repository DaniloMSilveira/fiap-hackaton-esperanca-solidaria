namespace EsperancaSolidaria.BuildingBlocks.Persistence;

/// <summary>
/// Contrato para unidade de trabalho, garantindo consistência transacional.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<(bool IsSuccess, string? ErrorMessage)> SaveChangesAsync(CancellationToken cancellationToken = default);
}