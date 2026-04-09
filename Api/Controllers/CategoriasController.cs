using Application.DTOs;
using Application.Interfaces;
using Application.Mappings;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController(IRepository<Categoria> repository,
    IMapper mapper,
    CategoriaFactoria categoriaFactoria) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categorias = await repository.GetAllAsync();
        var categoriasDto = categoriaFactoria.ToDtos(categorias);
        //var categoriasDto = mapper.Map<IEnumerable<CategoriaDto>>(categorias);
        return Ok(categoriasDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var categoria = await repository.GetByIdAsync(id);

        if (categoria is null)
            return NotFound(new { mensaje = $"Categoría con {id} no encontrada" });

        //var categoriaDto = mapper.Map<CategoriaDto>(categoria);
        var categoriaDto = categoriaFactoria.ToDto(categoria);

        return Ok(categoriaDto);
    }

    [HttpPost]    
    public async Task<IActionResult> Post([FromBody] CreateCategoriaDto categoriaDto,CancellationToken cancellationToken)
    {
        //var categoria = mapper.Map<Categoria>(categoriaDto);
        var categoria = categoriaFactoria.ToEntity(categoriaDto);

        if (ModelState.IsValid)
        {
            await repository.AddAsync(categoria);
            await repository.SaveChangesAsync(cancellationToken);

            //var categoriaCreadaDto = mapper.Map<CategoriaDto>(categoria);
            var categoriaCreadaDto = categoriaFactoria.ToDto(categoria);

            return CreatedAtAction(nameof(GetById), new { id = categoriaCreadaDto.Id }, categoriaCreadaDto);        
        }
        return NotFound();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, UpdateCategoriaDto categoriaDto,
        CancellationToken cancellationToken)
    {
        var existeCategoria = await repository.ExistsAsync(id,cancellationToken);
        if (!existeCategoria)
            return NotFound();
                
        var categoriaBd = await repository.GetByIdAsync(id);        

        categoriaBd!.Nombre = categoriaDto.Nombre;
        categoriaBd.Descripcion = categoriaDto.Descripcion;
        
        repository.Update(categoriaBd);
        await repository.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancelationToken)
    {
        var categoriaBd = await repository.ExistsAsync(id, cancelationToken);
        if (categoriaBd is false)
            return NotFound();

        await repository.DeleteAsync(id);
        await repository.SaveChangesAsync(cancelationToken);
        return NoContent();
    }
    
}
