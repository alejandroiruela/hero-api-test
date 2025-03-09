using Heroes.Models;
using Microsoft.EntityFrameworkCore;

namespace Heroes.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    //Defining Tables in Database
    public DbSet<Hero> Heroes { get; set; } = null!;
    public DbSet<Hability> Habilities { get; set; } = null!;
    public DbSet<Villain> Villains { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Hero>()
            .HasMany(h => h.Habilities)
            .WithOne(h => h.Hero)
            .HasForeignKey(h => h.HeroId);
        modelBuilder.Entity<Hero>().HasIndex(h => h.Hero_name);
        modelBuilder
            .Entity<Villain>()
            .HasMany(v => v.Habilities)
            .WithOne(v => v.Villain)
            .HasForeignKey(v => v.VillainId);
        modelBuilder.Entity<Villain>().HasIndex(v => v.Villain_Name);
        modelBuilder.Entity<Team>().HasIndex(t => t.TeamName);
        base.OnModelCreating(modelBuilder);
    }
}
