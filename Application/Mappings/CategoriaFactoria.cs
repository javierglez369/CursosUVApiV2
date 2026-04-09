using Application.DTOs;
using Domain.Entities;

namespace Application.Mappings;

public class CategoriaFactoria
{

    public CategoriaDto ToDto(Categoria categoria)
        => new(categoria.Id, categoria.Nombre, categoria.Descripcion, 
            categoria.Cursos != null ? categoria.Cursos.Count : 0);

    public IEnumerable<CategoriaDto> ToDtos(IEnumerable<Categoria> categorias)
        => categorias.Select(c => ToDto(c));

    public Categoria ToEntity(CreateCategoriaDto categoriaDto)
        => new() { Nombre = categoriaDto.Nombre, Descripcion = categoriaDto.Descripcion };

    public void Update(Categoria categoria, UpdateCategoriaDto categoriaDto)
    {
        categoria.Nombre = categoriaDto.Nombre;
        categoria.Descripcion = categoriaDto.Descripcion;
    }


}
