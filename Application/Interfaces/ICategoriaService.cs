using Application.Common.Models;
using Application.DTOs;

namespace Application.Interfaces;

public interface ICategoriaService
{
    //Lectura
    Task<IEnumerable<CategoriaDto>> GetAllAsync(
        bool soloActivas=true,
        CancellationToken cancellationToken = default);

    Task<CategoriaConCursosDto?> GetByIdConCursosAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<CategoriaDto>> GetPagedAsync(
        QueryParameters parameters,        
        CancellationToken cancellationToken = default);

    Task<CategoriaDto?> GetByIdAsync(
        int id, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CategoriaDto>> BuscarPorNombreAsync(
            string termino,
            CancellationToken cancellationToken = default);

    Task<CategoriaExisteDto> ExistePorNombreAsync(
            string nombre,
            CancellationToken cancellationToken = default);


    //Escritura
    Task<CategoriaDto> CreateAsync(
        CreateCategoriaDto categoriaDto,
        CancellationToken cancellationToken = default);

    Task<CategoriaDto> UpdateAsync(
        int id,
        UpdateCategoriaDto categoriaDto,
        CancellationToken cancellationToken = default
        );

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

}
