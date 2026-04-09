using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile:Profile
{
    public MappingProfile()
    {
        //Categoria
        CreateMap<Categoria, CategoriaDto>()
            .ForCtorParam("TotalCursos",
            opt=>opt.MapFrom(src=>src.Cursos!=null?src.Cursos.Count:0));

        CreateMap<CreateCategoriaDto, Categoria>();
        CreateMap<UpdateCategoriaDto, Categoria>();

    }

}
