using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourService.Domain;

[Table("tour_reviews")]
public class TourReview
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public long TourId { get; set; }
    
    [Required]
    public long UserId { get; set; }
    
    [Required]
    public DateTime VisitationTime { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public string Comment { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    [Range(1, 5)]
    public int? Rating { get; set; }
    
    public Tour Tour { get; set; } = null!;

    public TourReview()
    {
        Comment = string.Empty;
    }
}