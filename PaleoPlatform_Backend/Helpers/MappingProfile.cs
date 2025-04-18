﻿using PaleoPlatform_Backend.Models.DTOs;
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
            .ForMember(dest => dest.AutoreUserName, opt => opt.MapFrom(src => src.Autore.UserName))
            .ForMember(dest => dest.Commenti, opt => opt.MapFrom(src => src.Commenti));

            CreateMap<Commento, CommentoReadDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Utente.UserName));

            CreateMap<ArticoloCreateDto, Articolo>();
            CreateMap<ArticoloUpdateDto, Articolo>()
                .ForMember(dest => dest.CopertinaUrl, opt => opt.Ignore())
                .ForMember(dest => dest.DataUltimaModifica, opt => opt.Ignore());
        }
    }
}
