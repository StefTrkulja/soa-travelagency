using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourService.Domain;

[Table("tour_key_points")]
public class TourKeyPoint
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public long TourId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,8)")]
    public decimal Latitude { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(11,8)")]
    public decimal Longitude { get; set; }
    
    public int Order { get; set; }
    
    public Tour Tour { get; set; } = null!;

    public TourKeyPoint()
    {
        Name = string.Empty;
        Description = string.Empty;
    }
}
