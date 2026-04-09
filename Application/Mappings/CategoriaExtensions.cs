using Application.DTOs;
using Domain.Entities;

namespace Application.Mappings;

public static class CategoriaExtensions
{
    public static CategoriaDto ToDto(this Categoria categoria)
        => new(
            Id: categoria.Id,
            Nombre: categoria.Nombre,
            Descripcion: categoria.Descripcion,
            Activa: categoria.Activa,
            TotalCursos: categoria.Cursos?.Count ?? 0
            );

    public static Categoria ToEntity(this CreateCategoriaDto categoriaDto)
        => new()
        {
            Nombre = categoriaDto.Nombre,
            Descripcion = categoriaDto.Descripcion,
            Activa = categoriaDto.Activa
        };

    public static void ApplyUpdate(this Categoria categoria, UpdateCategoriaDto categoriaDto)
    {
        categoria.Nombre = categoriaDto.Nombre;
        categoria.Descripcion = categoriaDto.Descripcion;
        categoria.Activa = categoriaDto.Activa;
    }

}
