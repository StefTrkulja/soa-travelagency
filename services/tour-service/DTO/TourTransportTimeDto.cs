using System.ComponentModel.DataAnnotations;

namespace TourService.DTO;

public class TourTransportTimeDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    
    [Required(ErrorMessage = "Tip prevoza je obavezan")]
    public string TransportType { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Trajanje je obavezno")]
    [Range(1, int.MaxValue, ErrorMessage = "Trajanje mora biti veće od 0")]
    public int DurationMinutes { get; set; }
}

public class CreateTourTransportTimeRequestDto
{
    [Required(ErrorMessage = "Tip prevoza je obavezan")]
    [Range(0, 2, ErrorMessage = "Tip prevoza mora biti između 0 i 2 (Walking, Bicycle, Car)")]
    public int TransportType { get; set; }
    
    [Required(ErrorMessage = "Trajanje je obavezno")]
    [Range(1, int.MaxValue, ErrorMessage = "Trajanje mora biti veće od 0")]
    public int DurationMinutes { get; set; }
}
