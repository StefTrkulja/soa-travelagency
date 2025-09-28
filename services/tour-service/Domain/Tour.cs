using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourService.Domain;

public enum TourStatus
{
    Draft,
    Published,
    Active,
    Completed,
    Cancelled
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
    
    // Navigation property for tags
    public ICollection<TourTag> TourTags { get; set; } = new List<TourTag>();
    
    // Helper method to get tags as list of strings
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