using System.ComponentModel.DataAnnotations;

namespace TourService.DTO;

public class TourKeyPointDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    
    [Required(ErrorMessage = "Naziv ključne tačke je obavezan")]
    [StringLength(200, ErrorMessage = "Naziv ključne tačke ne može biti duži od 200 karaktera")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Opis ključne tačke ne može biti duži od 1000 karaktera")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Latitude je obavezna")]
    [Range(-90, 90, ErrorMessage = "Latitude mora biti između -90 i 90")]
    public decimal Latitude { get; set; }
    
    [Required(ErrorMessage = "Longitude je obavezna")]
    [Range(-180, 180, ErrorMessage = "Longitude mora biti između -180 i 180")]
    public decimal Longitude { get; set; }
    
    public int Order { get; set; }
}

public class CreateTourKeyPointRequestDto
{
    [Required(ErrorMessage = "Naziv ključne tačke je obavezan")]
    [StringLength(200, ErrorMessage = "Naziv ključne tačke ne može biti duži od 200 karaktera")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Opis ključne tačke ne može biti duži od 1000 karaktera")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Latitude je obavezna")]
    [Range(-90, 90, ErrorMessage = "Latitude mora biti između -90 i 90")]
    public decimal Latitude { get; set; }
    
    [Required(ErrorMessage = "Longitude je obavezna")]
    [Range(-180, 180, ErrorMessage = "Longitude mora biti između -180 i 180")]
    public decimal Longitude { get; set; }
}
