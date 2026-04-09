using Application.Interfaces;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class CategoriaRepository(ApplicationDbContext context) : ICategoriaRepository
{
    public async Task AddAsync(Categoria categoria)
    {
        categoria.Activa = true;
        await context.Categorias.AddAsync(categoria);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var categoria = await GetByIdAsync(id);
        if(categoria is not null)
        {
            context.Categorias.Remove(categoria);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsyncs(int id) => await context.Categorias.AnyAsync(c => c.Id == id);

    public async Task<IEnumerable<Categoria>> GetAllAsync()
        => await context.Categorias
                .Where(c => c.Activa)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

    public async Task<Categoria?> GetByIdAsync(int id)
        => await context.Categorias.FindAsync(id);

    public async Task UpdateAsync(Categoria categoria)
    {
        context.Categorias.Update(categoria);
        await context.SaveChangesAsync();
    }
}
