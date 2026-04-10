using Application.Common.Models;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    // ── LECTURA BÁSICA ────────────────────────────────────────────
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Overload con includes dinámicos (params = número variable de argumentos)
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

    // ── CONSULTAS AVANZADAS ───────────────────────────────────────
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<PagedResult<T>> GetPagedAsync(
        QueryParameters parameters,
        CancellationToken cancellationToken = default);

    // ── UTILIDADES ────────────────────────────────────────────────
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    // ── ESCRITURA ─────────────────────────────────────────────────
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
 
}