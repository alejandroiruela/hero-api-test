namespace Heroes.Models.DTOs;

public class TeamDTO
{
    public string TeamName { get; set; } = null!;
    public ICollection<HeroDTO> Heroes { get; set; } = [];
    public ICollection<VillainDTO> Villains { get; set; } = [];
}

public class CreateTeamDTO
{
    public string TeamName { get; set; } = null!;
}
