using Microsoft.EntityFrameworkCore;
using TourService.Domain;

namespace TourService.Database;

public class TourContext : DbContext
{
    public DbSet<Tour> Tours { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TourTag> TourTags { get; set; }

    public TourContext(DbContextOptions<TourContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        // Tour configuration
        modelBuilder.Entity<Tour>()
            .HasIndex(t => t.AuthorId);

        // Tag configuration
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

        // TourTag configuration (Many-to-Many)
        modelBuilder.Entity<TourTag>()
            .HasKey(tt => tt.Id);
            
        modelBuilder.Entity<TourTag>()
            .HasIndex(tt => new { tt.TourId, tt.TagId })
            .IsUnique();

        modelBuilder.Entity<TourTag>()
            .HasOne(tt => tt.Tour)
            .WithMany(t => t.TourTags)
            .HasForeignKey(tt => tt.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TourTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TourTags)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enum configurations
        modelBuilder.Entity<Tour>()
            .Property(t => t.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Tour>()
            .Property(t => t.Difficulty)
            .HasConversion<string>();

        modelBuilder.HasAnnotation("Relational:Collation", null);
    }
}