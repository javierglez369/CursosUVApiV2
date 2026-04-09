using Application.Common.Models;
using Application.DTOs;
using Application.Interfaces;
using Application.Mappings;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class CategoriasController(IUnitOfWork uow) : BaseApiController
{
    private readonly IRepository<Categoria> repository=uow.Repository<Categoria>();

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoriaDto>>),200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaDto>>>> Get()
    {
        var categorias = await uow.Repository<Categoria>().GetAllAsync();
        var categoriasDto = categorias.Select(c=>c.ToDto());        
        return Success(categoriasDto);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoriaDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> GetById(int id)
    {
        var categoria = await repository.GetByIdAsync(id);

        if (categoria is null)
            return NotFound(new { mensaje = $"Categoría con {id} no encontrada" });
        
        var categoriaDto = categoria.ToDto();

        return Success(categoriaDto);
    }

    [HttpPost]    
    public async Task<IActionResult> Post([FromBody] CreateCategoriaDto categoriaDto,CancellationToken cancellationToken)
    {
        
        var categoria = categoriaDto.ToEntity();

        if (ModelState.IsValid)
        {
            await repository.AddAsync(categoria);
            await uow.SaveChangesAsync();

            var categoriaCreadaDto = categoria.ToDto();

            return CreatedAtAction(nameof(GetById), new { id = categoriaCreadaDto.Id }, categoriaCreadaDto);        
        }
        return NotFound();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, UpdateCategoriaDto categoriaDto,
        CancellationToken cancellationToken)
    {
        
            var existeCategoria = await repository.ExistsAsync(id, cancellationToken);
            if (!existeCategoria)
                return NotFound();

            var categoria = await repository.GetByIdAsync(id, cancellationToken);

            if (categoria is null)
                return NotFound();

            categoria.ApplyUpdate(categoriaDto);

            repository.Update(categoria);
            await uow.SaveChangesAsync();
            return NoContent();        
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancelationToken)
    {
        var categoriaBd = await repository.ExistsAsync(id, cancelationToken);
        if (categoriaBd is false)
            return NotFound();

        await repository.DeleteAsync(id);
        await uow.SaveChangesAsync();
        return NoContent();
    }
    
}
