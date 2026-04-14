using EsperancaSolidaria.Domain.Interfaces.Repositories;

namespace EsperancaSolidaria.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUsuarioRepository UsuarioRepository { get; }
    Task<bool> Commit();
}