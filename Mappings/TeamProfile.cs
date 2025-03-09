using AutoMapper;
using Heroes.Models;
using Heroes.Models.DTOs;

namespace Heroes.Profiles;

public class TeamProfile : Profile
{
    public TeamProfile()
    {
        //Mapping from Team to TeamDTO
        CreateMap<Team, TeamDTO>();
        //Mapping from TeamDTO to Team
        CreateMap<TeamDTO, Team>();
        //Mapping from CreateTeamDTO to Team
        CreateMap<CreateTeamDTO, Team>();
        //Mapping from HeroDTO to Hero
        // CreateMap<HeroDTO, Hero>();
        // //Mapping from Hero to HeroDTO
        // CreateMap<Hero, HeroDTO>();
        // //Mapping from Villain to VillainDTO
        // CreateMap<Villain, VillainDTO>();
        // //Mapping from VillainDTO to Villain
        // CreateMap<VillainDTO, Villain>();
        // //Mapping from Hability to HabilityDTO
        // CreateMap<Hability, HabilityDTO>();
        // //Mapping from HabilityDTO to Hability
        // CreateMap<HabilityDTO, Hability>();
    }
}
