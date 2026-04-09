using Domain.Entities;

namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{    
    /// <summary>
    /// Obtiene un repositorio genérico para la entidad T.
    /// Se instancia de forma lazy la primera vez que se solicita.
    /// </summary>
    IRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>
    /// Persiste todos los cambios rastreados por EF Core.
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Inicia una transacción de base de datos explícita.
    /// Si ya hay una activa, no hace nada.
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Llama SaveChangesAsync y confirma la transacción activa.
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Revierte la transacción activa sin guardar cambios.
    /// </summary>
    Task RollbackAsync();
}