using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Heroes.Models;

[Index(nameof(Villain_Name), IsUnique = true)]
public class Villain
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Villain_Name { get; set; } = null!;

    [Required]
    public string Power { get; set; } = null!;

    public ICollection<Hability> Habilities { get; set; } = [];

    public int? TeamId { get; set; }

    [ForeignKey("TeamId")]
    public Team? Team { get; set; }

    public Villain()
    {
        Name = "None";
        Villain_Name = "John Doe Dark";
        Habilities = [];
    }
}
