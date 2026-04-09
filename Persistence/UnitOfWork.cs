using Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Contexts;
using Persistence.Repositories;
using System.Collections.Concurrent;

namespace Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    // C# 13: Lock es System.Threading.Lock — más eficiente que object para lock()
    private readonly Lock _lock = new();

    // Caché de repositorios instanciados — thread-safe
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    // Primary constructor — C# 12/13
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

    }

    // ─── Repositorios ────────────────────────────────────────────────────────

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        // GetOrAdd es atómico en ConcurrentDictionary — thread-safe sin lock adicional
        return (IRepository<T>)_repositories.GetOrAdd(
            typeof(T),
            _ => new Repository<T>(_context));
    }

    // ─── Persistencia ────────────────────────────────────────────────────────

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    // ─── Transacciones ───────────────────────────────────────────────────────

    public async Task BeginTransactionAsync()
    {
        // Idempotente: si ya hay transacción activa, no abre otra
        if (_currentTransaction is not null)
            return;

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            // Guardamos primero, luego confirmamos la transacción
            await _context.SaveChangesAsync();

            if (_currentTransaction is not null)
                await _currentTransaction.CommitAsync();
        }
        catch
        {
            // Si algo falla, revertimos automáticamente
            await RollbackAsync();
            throw; // Re-lanzamos para que el middleware lo capture
        }
        finally
        {
            DisposeTransaction();
        }
    }

    public async Task RollbackAsync()
    {
        if (_currentTransaction is not null)
            await _currentTransaction.RollbackAsync();

        DisposeTransaction();
    }

    // ─── Dispose ─────────────────────────────────────────────────────────────

    private void DisposeTransaction()
    {
        if (_currentTransaction is null) return;
        _currentTransaction.Dispose();
        _currentTransaction = null;
    }

    public void Dispose()
    {
        DisposeTransaction();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
