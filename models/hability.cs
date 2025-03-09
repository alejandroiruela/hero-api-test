using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Heroes.Models;

public class Hability
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }

    public int? HeroId { get; set; }

    [ForeignKey("HeroId")]
    public Hero? Hero { get; set; }

    public int? VillainId { get; set; }

    [ForeignKey("VillainId")]
    public Villain? Villain { get; set; }

    public Hability()
    {
        Name = "None";
    }
}
