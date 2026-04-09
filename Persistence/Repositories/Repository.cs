using Application.Common.Models;
using Application.Interfaces;
using Persistence.Contexts;
using System.Linq.Expressions;
using System.Reflection;

namespace Persistence.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : BaseEntity
{
    protected readonly ApplicationDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    // ── LECTURA BÁSICA ────────────────────────────────────────────

    public virtual async Task<T?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);  // collection expression

    public virtual async Task<T?> GetByIdAsync(
        int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;

        // Agrega cada include de forma encadenada
        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.ToListAsync();
    }

    // ── CONSULTAS AVANZADAS ───────────────────────────────────────

    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking()
                       .Where(predicate)
                       .ToListAsync(cancellationToken);

    public async Task<PagedResult<T>> GetPagedAsync(
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        // 1. INCLUDES dinámicos desde strings (ej: ["Instructor", "Categoria"])
        foreach (var include in parameters.Includes)
            query = query.Include(include);

        // 2. BÚSQUEDA en múltiples campos de texto
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm)
            && parameters.SearchFields.Length > 0)
        {
            var term = parameters.SearchTerm.ToLower();

            // Construimos un predicado OR sobre los campos de búsqueda
            // Usamos reflexión para acceder a las propiedades por nombre
            var predicate = parameters.SearchFields
                .Select(field => BuildContainsPredicate(field, term))
                .Where(p => p is not null)
                .Cast<Expression<Func<T, bool>>>()
                .Aggregate((acc, next) => CombineOr(acc, next));

            query = query.Where(predicate);
        }

        // 3. CONTEO total (antes de paginar)
        var totalRecords = await query.CountAsync(cancellationToken);

        // 4. ORDENAMIENTO dinámico usando reflexión
        if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            var prop = typeof(T).GetProperty(
                parameters.OrderBy,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop is not null)
            {
                // Construye: x => x.Propiedad
                var param = Expression.Parameter(typeof(T), "x");
                var body = Expression.Property(param, prop);
                var keySelector = Expression.Lambda(body, param);

                // C# 13 — switch expression para dirección de ordenamiento
                query = parameters.OrderDirection?.ToLower() switch
                {
                    "desc" => Queryable.OrderByDescending(query, (dynamic)keySelector),
                    _ => Queryable.OrderBy(query, (dynamic)keySelector)
                };
            }
        }

        // 5. PAGINACIÓN con Skip / Take
        var data = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(data, totalRecords, parameters.PageNumber, parameters.PageSize);
    }

    // ── UTILIDADES ────────────────────────────────────────────────

    public async Task<bool> ExistsAsync(
        int id, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);

    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(predicate, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _dbSet.CountAsync(cancellationToken);

    // ── ESCRITURA ─────────────────────────────────────────────────

    public virtual async Task AddAsync(
        T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public virtual async Task AddRangeAsync(
        IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await _dbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(T entity)
    {
        // Verificar si ya está siendo rastreada para evitar conflicto
        var tracked = _context.ChangeTracker.Entries<T>()
            .FirstOrDefault(e => e.Entity.Id == entity.Id);

        if (tracked is null)
            _dbSet.Attach(entity);

        _context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
        => _dbSet.UpdateRange(entities);

    public virtual async Task DeleteAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
            _dbSet.Remove(entity);
    }

    public virtual async Task DeleteRangeAsync(
        IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);
        _dbSet.RemoveRange(entities);
    }


    // ── HELPERS PRIVADOS ──────────────────────────────────────────

    // Construye: x => x.Campo.ToLower().Contains(term)
    private static Expression<Func<T, bool>>? BuildContainsPredicate(
        string fieldName, string term)
    {
        var prop = typeof(T).GetProperty(
            fieldName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        // Solo buscamos en propiedades de tipo string
        if (prop is null || prop.PropertyType != typeof(string))
            return null;

        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, prop);

        // x.Campo.ToLower()
        var toLower = Expression.Call(
            property,
            typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

        // .Contains(term)
        var contains = Expression.Call(
            toLower,
            typeof(string).GetMethod("Contains", [typeof(string)])!,
            Expression.Constant(term));

        return Expression.Lambda<Func<T, bool>>(contains, param);
    }

    // Combina dos expresiones con OR lógico
    private static Expression<Func<T, bool>> CombineOr(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var leftBody = Expression.Invoke(left, param);
        var rightBody = Expression.Invoke(right, param);
        var body = Expression.OrElse(leftBody, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}