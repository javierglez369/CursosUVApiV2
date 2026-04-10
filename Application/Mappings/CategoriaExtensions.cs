using Application.DTOs;
using Domain.Entities;
using Domain.Enums;

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

    public static IEnumerable<CategoriaDto> ToDtoList(
        this IEnumerable<Categoria> categorias)
        => categorias.Select(c => c.ToDto());  

    public static CategoriaConCursosDto ToDtoConCursos(this Categoria categoria)
        => new(
            categoria.Id,
            categoria.Nombre,
            categoria.Descripcion,
            categoria.Activa,
            categoria.Cursos?.Select(curso=>curso.ToCursoResumenDto()) ?? []
            );

    public static CursoResumenDto ToCursoResumenDto(this Curso curso)
        => new(
            Id: curso.Id,
            Titulo: curso.Titulo,
            Estado: curso.Estado.ToString(),
            Precio: curso.Precio
            );

    public static Categoria ToEntity(this CreateCategoriaDto categoriaDto)
        => new()
        {
            Nombre = categoriaDto.Nombre,
            Descripcion = categoriaDto.Descripcion,
            Activa = categoriaDto.Activa
        };

    public static Curso ToCursoEntity(
        this CreateCursoEnCategoriaDto cursoDto, int categoriaId, int instructorId)
        => new()
        {
            Titulo = cursoDto.Titulo.Trim(),
            Descripcion = cursoDto.Descripcion?.Trim(),
            Precio = cursoDto.Precio,
            Nivel = cursoDto.Nivel,
            Estado = EstadoCurso.Borrador,
            CategoriaId = categoriaId,
            InstructorId = instructorId
        };

    public static void ApplyUpdate(this Categoria categoria, UpdateCategoriaDto categoriaDto)
    {
        categoria.Nombre = categoriaDto.Nombre;
        categoria.Descripcion = categoriaDto.Descripcion;
        categoria.Activa = categoriaDto.Activa;
    }

}
