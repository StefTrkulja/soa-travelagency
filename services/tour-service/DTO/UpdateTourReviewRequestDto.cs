using System.ComponentModel.DataAnnotations;

namespace TourService.DTO;

public class UpdateTourReviewRequestDto
{
    [Required]
    [MaxLength(2000)]
    public string Comment { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [Range(1, 5)]
    public int? Rating { get; set; }
}