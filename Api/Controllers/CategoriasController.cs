using Application.Common.Models;
using Application.DTOs;
using Application.Interfaces;
using Application.Mappings;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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
            return NotFound(new { mensaje = $"Categoría con {id} no encontrada" });
                
        return Success(categoriaDto);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> Post([FromBody] CreateCategoriaDto categoriaDto, CancellationToken cancellationToken)
    {

        var categoria = await categoriaService.CreateAsync(categoriaDto, cancellationToken);
        return Created(categoria, nameof(GetById), new { id = categoria.Id },"Categoría Creada exitosamente");

    }

    [HttpPut("{id:int}")]
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
