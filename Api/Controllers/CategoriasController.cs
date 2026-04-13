using Api.Attributes;
using Application.Common.Models;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize(Roles = "Administrador,Estudiante")]

public class CategoriasController(ICategoriaService categoriaService) : BaseApiController
{    

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoriaDto>>),200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaDto>>>> Get()
    {
        var categoriasDto = await categoriaService.GetAllAsync();        
        return Success(categoriasDto);
    }

    [HttpGet("paginado")]
    public async Task<ActionResult<ApiResponse<PagedResponse<CategoriaDto>>>> GetPaginado(
        [FromQuery] QueryParameters parameters, CancellationToken cancellationToken=default)
    {

        var categoriasPaginadas = await categoriaService.GetPagedAsync(parameters, cancellationToken);

        var pagedResponse = new PagedResponse<CategoriaDto>(categoriasPaginadas.Data, categoriasPaginadas.PageNumber,
            categoriasPaginadas.PageSize, categoriasPaginadas.TotalRecords);        

        return Success(pagedResponse);
    }


    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoriaDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> GetById(int id)
    {
        var categoriaDto = await categoriaService.GetByIdAsync(id);

        if (categoriaDto is null)
            ThrowNotFound($"Categoría con id {id} no encontrada");

        return Success(categoriaDto);
    }

    /// <summary>
    /// Obtiene únicamente las categorías marcadas como activas.
    /// Útil para poblar dropdowns en el frontend.
    /// </summary>
    [HttpGet("activas")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<CategoriaDto>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaDto>>>> GetActivas(
        CancellationToken cancellationToken = default)
    {
        var categorias = await categoriaService
            .GetAllAsync(soloActivas: true, cancellationToken);

        if (!categorias.Any())
            ThrowNotFound("No hay categorías activas disponibles");

        return Success(categorias, "Categorías activas obtenidas correctamente");
    }

    // ── GET /api/categorias/buscar?termino=web ─────────────────────────
    /// <summary>
    /// Busca categorías cuyo nombre contenga el término indicado.
    /// Retorna lista plana sin paginación — ideal para autocompletados.
    /// </summary>
    /// <param name="termino">Texto a buscar en el nombre de la categoría</param>
    [HttpGet("buscar")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ApiResponse<IEnumerable<CategoriaDto>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaDto>>>> Buscar(
        [FromQuery] string termino,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termino))
            ThrowBadRequest("El término de búsqueda es requerido");

        var categorias = await categoriaService
            .BuscarPorNombreAsync(termino, cancellationToken);

        if (!categorias.Any())
            ThrowNotFound($"No se encontraron categorías que coincidan con '{termino}'");

        return Success(categorias, $"{categorias.Count()} categoría(s) encontrada(s)");
    }

    // ── GET /api/categorias/existe?nombre=Marketing ────────────────────
    /// <summary>
    /// Verifica si existe una categoría con el nombre indicado.
    /// Siempre retorna 200 — el campo "existe" indica el resultado.
    /// Nunca retorna 404 porque la ausencia es un resultado válido, no un error.
    /// </summary>
    /// <param name="nombre">Nombre exacto a verificar</param>
    [HttpGet("existe")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ApiResponse<CategoriaExisteDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<CategoriaExisteDto>>> Existe(
        [FromQuery] string nombre,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            ThrowBadRequest("El nombre es requerido para verificar existencia");

        var resultado = await categoriaService
            .ExistePorNombreAsync(nombre, cancellationToken);

        return Success(resultado);
    }

    // ── GET /api/categorias/{id}/cursos ────────────────────────────────
    /// <summary>
    /// Obtiene una categoría con todos sus cursos incluidos.
    /// </summary>
    /// <param name="id">Id de la categoría</param>
    [HttpGet("{id:int}/cursos")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ApiResponse<CategoriaConCursosDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse<object>),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<CategoriaConCursosDto>>> GetConCursos(
        int id,
        CancellationToken cancellationToken = default)
    {
        var categoria = await categoriaService
            .GetByIdConCursosAsync(id, cancellationToken);

        if (categoria is null)
            ThrowNotFound($"Categoría con id {id} no encontrada");

        return Success(categoria);
    }

    [HttpPost]
    [Validate<CreateCategoriaDto>]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> Post([FromBody] CreateCategoriaDto categoriaDto, CancellationToken cancellationToken)
    {

        var categoria = await categoriaService.CreateAsync(categoriaDto, cancellationToken);
        return Created(categoria, nameof(GetById), new { id = categoria.Id },"Categoría Creada exitosamente");

    }

    [HttpPut("{id:int}")]
    [Validate<UpdateCategoriaDto>]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> Put(int id, UpdateCategoriaDto categoriaDto,
        CancellationToken cancellationToken)
    {
        var categoria = await categoriaService.UpdateAsync(id, categoriaDto, cancellationToken);    

        return Success(categoria, "Categoría actualizada exitosamente");
    }


    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancelationToken)
    {
        await categoriaService.DeleteAsync(id, cancelationToken);
        return Success<object>(null!, "Categoría eliminada exitosamente");
    }

}
