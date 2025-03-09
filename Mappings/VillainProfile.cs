using AutoMapper;
using Heroes.Models;
using Heroes.Models.DTOs;

namespace Heroes.Profiles;

public class VillainProfile : Profile
{
    public VillainProfile()
    {
        //Mapping from Villain to VillainDTO
        CreateMap<Villain, VillainDTO>()
            .ForMember(dest => dest.Habilities, opt => opt.MapFrom(src => src.Habilities))
            .ForMember(
                dest => dest.Team_Name,
                opt => opt.MapFrom(src => src.Team != null ? src.Team.TeamName : null)
            );
        //Mapping from VillainDTO to Villain
        CreateMap<VillainDTO, Villain>()
            .ForMember(dest => dest.Habilities, opt => opt.MapFrom(src => src.Habilities));
        //Mapping from CreateVillainDTO to Villain
        CreateMap<CreateVillainDTO, Villain>();
        //Mapping from Hability to HabilityDTO
        CreateMap<Hability, HabilityDTO>();
        //Mapping from HabilityDTO to Hability
        CreateMap<HabilityDTO, Hability>();
    }
}
