using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourService.Domain;

[Table("tour_tags")]
public class TourTag
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public long TourId { get; set; }
    
    [Required]
    public long TagId { get; set; }
    
    public Tour Tour { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}