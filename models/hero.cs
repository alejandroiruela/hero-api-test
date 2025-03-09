using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Heroes.Models;

[Index(nameof(Hero_name), IsUnique = true)]
public class Hero
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Hero_name { get; set; }
    public string Power { get; set; }
    public int? TeamId { get; set; }
    public ICollection<Hability> Habilities { get; set; }

    [ForeignKey("TeamId")]
    public Team? Team { get; set; }

    public Hero()
    {
        Name = "None";
        Hero_name = "NoneMan";
        Power = "flying";
        Habilities = [];
    }
}
