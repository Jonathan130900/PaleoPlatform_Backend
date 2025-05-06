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
                .ForMember(dest => dest.AutoreUserName, opt => opt.MapFrom(src => src.Autore.UserName))
                .ForMember(dest => dest.CopertinaUrl, opt => opt.MapFrom(src => src.CopertinaUrl))
                .ForMember(dest => dest.Commenti, opt => opt.MapFrom(src => src.Commenti));

            CreateMap<Commento, CommentoReadDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Utente.UserName));

            CreateMap<ArticoloCreateDto, Articolo>();
            CreateMap<ArticoloUpdateDto, Articolo>()
                .ForMember(dest => dest.CopertinaUrl, opt => opt.Ignore())
                .ForMember(dest => dest.DataUltimaModifica, opt => opt.Ignore());

            CreateMap<Discussione, DiscussioneReadDto>()
                .ForMember(dest => dest.AutoreUsername, opt => opt.MapFrom(src => src.Autore.UserName))
                .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.TopicId));

            CreateMap<DiscussioneCreateDto, Discussione>();

            CreateMap<Topics, TopicReadDto>();
            CreateMap<TopicCreateDto, Topics>();

            CreateMap<BigliettoCreateDto, Biglietto>(); 
            CreateMap<Biglietto, BigliettoReadDto>();

            CreateMap<EventoCreateDto, Evento>();
            CreateMap<Evento, EventoReadDto>();
            CreateMap<EventoUpdateDto, Evento>();

            CreateMap<EventoPartecipazioneCreateDto, EventoPartecipazione>();


            CreateMap<Prodotto, ProdottoReadDto>();
            CreateMap<ProdottoCreateDto, Prodotto>();


            
            CreateMap<Carrello, CarrelloReadDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CarrelloItem, CarrelloItemReadDto>()
                .ForMember(dest => dest.NomeProdotto, opt => opt.MapFrom(src => src.Prodotto.Nome))
                .ForMember(dest => dest.Prezzo, opt => opt.MapFrom(src => src.Prodotto.Prezzo))
                .ForMember(dest => dest.ProdottoId, opt => opt.MapFrom(src => src.ProdottoId))
                .ForMember(dest => dest.Quantità, opt => opt.MapFrom(src => src.Quantità));


        }
    }
}
