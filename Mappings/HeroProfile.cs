using AutoMapper;
using Heroes.Models;
using Heroes.Models.DTOs;

namespace Heroes.Profiles
{
    public class HeroProfile : Profile
    {
        public HeroProfile()
        {
            //Mapping Hero to HeroDTO
            CreateMap<Hero, HeroDTO>()
                .ForMember(dest => dest.Habilities, opt => opt.MapFrom(src => src.Habilities))
                .ForMember(
                    dest => dest.TeamName,
                    opt => opt.MapFrom(src => src.Team != null ? src.Team.TeamName : null)
                )
                .ForMember(dest => dest.HeroName, opt => opt.MapFrom(src => src.Hero_name));
            //Mapping HeroDTO to Hero
            CreateMap<HeroDTO, Hero>()
                .ForMember(dest => dest.Habilities, opt => opt.MapFrom(src => src.Habilities))
                .ForMember(dest => dest.Hero_name, opt => opt.MapFrom(src => src.HeroName));
            //Mapping HeroCreateUpdateDTO to Hero
            CreateMap<HeroCreateUpdateDTO, Hero>()
                .ForMember(dest => dest.Habilities, opt => opt.MapFrom(src => src.Habilities))
                .ForMember(dest => dest.Hero_name, opt => opt.MapFrom(src => src.HeroName));
            //Mapping HeroCreateUpdateDTO to HeroDTO
            CreateMap<HeroCreateUpdateDTO, HeroDTO>();
            //Mapping Hability to HabilityDTO
            CreateMap<Hability, HabilityDTO>();
            //Mapping HabilityDTO to Hability
            CreateMap<HabilityDTO, Hability>();
        }
    }
}
