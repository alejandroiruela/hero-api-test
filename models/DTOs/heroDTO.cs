namespace Heroes.Models.DTOs;

public class HeroDTO
{
    public string Name { get; set; } = null!;
    public string HeroName { get; set; } = null!;
    public string Power { get; set; } = null!;
    public string? TeamName { get; set; }
    public ICollection<HabilityDTO> Habilities { get; set; } = null!;
}

public class HeroCreateUpdateDTO
{
    public string Name { get; set; } = null!;
    public string HeroName { get; set; } = null!;
    public string Power { get; set; } = null!;
    public ICollection<HabilityDTO> Habilities { get; set; } = [];
}
