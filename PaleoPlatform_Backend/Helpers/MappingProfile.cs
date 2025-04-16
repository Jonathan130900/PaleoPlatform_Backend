using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Models;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaleoPlatform_Backend.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Articolo, ArticoloReadDto>()
                .ForMember(dest => dest.AutoreEmail, opt => opt.MapFrom(src => src.Autore.Email));
            CreateMap<ArticoloCreateDto, Articolo>();
        }
    }
}
