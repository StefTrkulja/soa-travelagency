using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourService.Domain;

[Table("tags")]
public class Tag
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    public ICollection<TourTag> TourTags { get; set; } = new List<TourTag>();

    public Tag()
    {
        Name = string.Empty;
    }
}