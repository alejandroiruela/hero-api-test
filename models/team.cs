using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Heroes.Models;

[Index(nameof(TeamName), IsUnique = true)]
public class Team
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string TeamName { get; set; } = null!;
    public ICollection<Hero> Heroes { get; set; } = [];
    public ICollection<Villain> Villains { get; set; } = [];
}
