using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class MainDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ProfilePhoto> ProfilePhotos { get; set; }
    public DbSet<PersonInformation> PersonInformations { get; set; }
    public DbSet<PlaceOfResidence> PlaceOfResidences { get; set; }
     
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(a => a.PersonInformations)
            .WithOne(s => s.User)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PersonInformation>()
            .HasOne(a => a.PlaceOfResidence)
            .WithOne(s => s.PersonInformation)
            .HasForeignKey<PlaceOfResidence>(d => d.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<PersonInformation>()
            .HasOne(a => a.ProfilePhoto)
            .WithOne(s=>s.PersonInformation)
            .HasForeignKey<ProfilePhoto>(d => d.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}