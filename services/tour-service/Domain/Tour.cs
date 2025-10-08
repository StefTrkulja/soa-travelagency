using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourService.Domain;

public enum TourStatus
{
    Draft,
    Published,
    Active,
    Completed,
    Cancelled,
    Archived
}

public enum TransportType
{
    Walking,
    Bicycle,
    Car
}

public enum TourDifficulty
{
    Easy,
    Moderate,
    Hard,
    Expert
}

[Table("tours")]
public class Tour
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; }
    
    [Required]
    public TourDifficulty Difficulty { get; set; }
    
    [Required]
    public TourStatus Status { get; set; } = TourStatus.Draft;
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; } = 0;
    
    [Required]
    public long AuthorId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    public DateTime? ArchivedAt { get; set; }
    
    [Column(TypeName = "decimal(10,3)")]
    public decimal? DistanceInKm { get; set; }
    
    public ICollection<TourTag> TourTags { get; set; } = new List<TourTag>();
    
    public ICollection<TourKeyPoint> KeyPoints { get; set; } = new List<TourKeyPoint>();
    
    public ICollection<TourTransportTime> TransportTimes { get; set; } = new List<TourTransportTime>();
    
    public List<string> GetTags()
    {
        return TourTags.Select(tt => tt.Tag.Name).ToList();
    }

    public Tour()
    {
        Name = string.Empty;
        Description = string.Empty;
    }
}