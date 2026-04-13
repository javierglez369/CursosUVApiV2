using Application.Common.Models;
using Application.DTOs;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Application.Services;

public class CategoriaService
    (IUnitOfWork uow,
    ILogger<CategoriaService> logger)
    : ICategoriaService
{
   

    //private readonly IRepository<Categoria> _categoriasRepository= uow.Repository<Categoria>();
    public async Task<IEnumerable<CategoriaDto>> GetAllAsync(bool soloActivas = true, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Obteniendo categorías: soloActivas: "+ soloActivas);

        var categorias = soloActivas
            ? await uow.Repository<Categoria>()
            .FindAsync(c => c.Activa, cancellationToken)
            : await uow.Repository<Categoria>()
            .GetAllAsync(cancellationToken);

        return categorias.ToDtoList();
    }

    public async Task<CategoriaDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Buscando Categoria con Id: " + id);

        var categoria = await uow.Repository<Categoria>()
            .GetByIdAsync(id, cancellationToken);

        return categoria?.ToDto();

    }

    public async Task<CategoriaConCursosDto?> GetByIdConCursosAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug(
            "Buscando categoría {CategoriaId} con cursos", id);

        // Carga la navegación Cursos en la misma query — sin lazy loading
        var categoria = await uow.Repository<Categoria>()
            .GetByIdAsync(id, c => c.Cursos);

        return categoria?.ToDtoConCursos();
    }

    public async Task<IEnumerable<CategoriaDto>> BuscarPorNombreAsync(
    string termino,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            // Término vacío → devuelve todo en lugar de lanzar excepción
            // El controlador puede preferir este comportamiento para autocompletados
            logger.LogDebug("BuscarPorNombre llamado sin término — devolviendo todas");
            return await GetAllAsync(cancellationToken: cancellationToken);
        }

        var terminoNormalizado = termino.Trim().ToLower();

        logger.LogDebug(
            "Buscando categorías con término '{Termino}'", terminoNormalizado);

        var categorias = await uow.Repository<Categoria>()
            .FindAsync(
                c => c.Nombre.ToLower().Contains(terminoNormalizado),
                cancellationToken);

        // Ordenar por nombre en memoria — el conjunto ya está filtrado y es pequeño
        var resultado = categorias
            .OrderBy(c => c.Nombre)
            .ToDtoList();

        logger.LogDebug(
            "Búsqueda '{Termino}' → {Total} resultado(s)",
            terminoNormalizado,
            resultado.Count());

        return resultado;
    }



    public async Task<CategoriaExisteDto> ExistePorNombreAsync(
        string nombre,
        CancellationToken cancellationToken = default)
    {
        var nombreNormalizado = nombre.Trim().ToLower();

        var categorias = await uow.Repository<Categoria>()
            .FindAsync(
                c => c.Nombre.ToLower() == nombreNormalizado,
                cancellationToken);

        var categoria = categorias.FirstOrDefault();

        return categoria is not null
            ? new CategoriaExisteDto(true, categoria.Id, categoria.Nombre)
            : new CategoriaExisteDto(false, null, null);
    }

    public async Task<PagedResult<CategoriaDto>> GetPagedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Obteniendo categorías paginadas: soloActivas");

        var categorias = await uow.Repository<Categoria>()
            .GetPagedAsync(parameters, cancellationToken);

        return new PagedResult<CategoriaDto>(
            categorias.Data.ToDtoList(),
            categorias.TotalRecords,
            categorias.PageNumber,
            categorias.PageSize
        );
    }

    public async Task<CategoriaDto> CreateAsync(CreateCategoriaDto categoriaDto, CancellationToken cancellationToken = default)
    {

        await ValidarNombreUnicoAsync(categoriaDto.Nombre, null, cancellationToken);

        var categoria = categoriaDto.ToEntity();

        await uow.Repository<Categoria>()
            .AddAsync(categoria, cancellationToken);

        await uow.SaveChangesAsync();

        return categoria.ToDto();

    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoria = await ObtenerOEnviarExcepcion(id, cancellationToken);

        var tieneCursos = await uow.Repository<Curso>()
            .ExistsAsync(c => c.CategoriaId == id, cancellationToken);

        if (tieneCursos)
        {
            throw new AppException("No se puede eliminar la categoría porque tiene cursos asociados",
                HttpStatusCode.BadRequest);
        }

        await uow.Repository<Categoria>().DeleteAsync(id, cancellationToken);
        await uow.SaveChangesAsync();
    }

    public async Task<CategoriaDto> UpdateAsync(int id, UpdateCategoriaDto categoriaDto, CancellationToken cancellationToken = default)
    {
        var categoria = await ObtenerOEnviarExcepcion(id, cancellationToken);

        // Si el nombre ha cambiado, validar que el nuevo nombre sea único
        if (!categoria.Nombre.Equals(categoriaDto.Nombre, StringComparison.OrdinalIgnoreCase))
        {
            await ValidarNombreUnicoAsync(categoriaDto.Nombre, id, cancellationToken);
        }

        categoria.ApplyUpdate(categoriaDto);

        uow.Repository<Categoria>().Update(categoria);
        await uow.SaveChangesAsync();

        return categoria.ToDto();   
    }

    private async Task ValidarNombreUnicoAsync(
        string nombre,
        int? excludeId,
        CancellationToken cancellationToken = default)
    {

        var nombreNormalizado = nombre.Trim().ToLower();

        var existe = await uow.Repository<Categoria>()
            .ExistsAsync(c => c.Nombre.ToLower() == nombreNormalizado
                            && (excludeId == null || c.Id != excludeId),
                cancellationToken);

        if (existe)
            throw new AppException("Ya existe una categoría con el nombre " + nombre,
                HttpStatusCode.Conflict);

    }


    private async Task<Categoria> ObtenerOEnviarExcepcion(int id,
        CancellationToken cancellationToken = default)
    {
        var categoria = await uow.Repository<Categoria>()
            .GetByIdAsync(id, cancellationToken);

        if(categoria == null)
            throw new AppException("Categoría no encontrada con Id: " + id,HttpStatusCode.NotFound);

        return categoria;

    }


}
