using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourService.Domain;

[Table("tour_transport_times")]
public class TourTransportTime
{
    [Key]
    public long Id { get; set; }
    
    [Required]
    public long TourId { get; set; }
    
    [Required]
    public TransportType TransportType { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Vreme mora biti veće od 0")]
    public int DurationMinutes { get; set; }
    
    public Tour Tour { get; set; } = null!;
}
