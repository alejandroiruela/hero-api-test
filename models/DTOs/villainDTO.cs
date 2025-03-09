namespace Heroes.Models.DTOs;

public class VillainDTO
{
    public string Name { get; set; } = null!;
    public string Villain_Name { get; set; } = null!;
    public string Team_Name { get; set; } = null!;
    public string Power { get; set; } = null!;
    public ICollection<HabilityDTO> Habilities { get; set; } = [];
}

public class CreateVillainDTO
{
    public string Name { get; set; } = null!;
    public string Villain_Name { get; set; } = null!;
    public string Power { get; set; } = null!;
    public ICollection<HabilityDTO> Habilities { get; set; } = [];
}
