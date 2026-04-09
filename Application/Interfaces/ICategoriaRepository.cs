using Domain.Entities;

namespace Application.Interfaces;

public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(int id);
    Task<IEnumerable<Categoria>> GetAllAsync();
    Task<bool> ExistsAsyncs(int id);
    Task AddAsync(Categoria categoria);
    Task UpdateAsync(Categoria categoria);
    Task DeleteAsync(int id);
}
