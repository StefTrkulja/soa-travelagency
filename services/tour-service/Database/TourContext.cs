using Microsoft.EntityFrameworkCore;
using TourService.Domain;

namespace TourService.Database;

public class TourContext : DbContext
{
    public DbSet<Tour> Tours { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TourTag> TourTags { get; set; }
    public DbSet<TourKeyPoint> TourKeyPoints { get; set; }
    public DbSet<TourTransportTime> TourTransportTimes { get; set; }

    public TourContext(DbContextOptions<TourContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        modelBuilder.Entity<Tour>()
            .HasIndex(t => t.AuthorId);

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();

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

        modelBuilder.Entity<Tour>()
            .Property(t => t.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Tour>()
            .Property(t => t.Difficulty)
            .HasConversion<string>();

        // TourKeyPoint configuration
        modelBuilder.Entity<TourKeyPoint>()
            .HasOne(tkp => tkp.Tour)
            .WithMany(t => t.KeyPoints)
            .HasForeignKey(tkp => tkp.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TourKeyPoint>()
            .HasIndex(tkp => new { tkp.TourId, tkp.Order })
            .IsUnique();

        // TourTransportTime configuration
        modelBuilder.Entity<TourTransportTime>()
            .HasOne(ttt => ttt.Tour)
            .WithMany(t => t.TransportTimes)
            .HasForeignKey(ttt => ttt.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TourTransportTime>()
            .HasIndex(ttt => new { ttt.TourId, ttt.TransportType })
            .IsUnique();

        modelBuilder.Entity<TourTransportTime>()
            .Property(ttt => ttt.TransportType)
            .HasConversion<string>();

        modelBuilder.HasAnnotation("Relational:Collation", null);
    }
}