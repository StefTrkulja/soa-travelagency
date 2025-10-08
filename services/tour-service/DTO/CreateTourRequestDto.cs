using System.ComponentModel.DataAnnotations;

namespace TourService.DTO;

public class CreateTourRequestDto
{
    [Required(ErrorMessage = "Naziv ture je obavezan")]
    [StringLength(200, ErrorMessage = "Naziv ture ne može biti duži od 200 karaktera")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Opis ture je obavezan")]
    [StringLength(2000, ErrorMessage = "Opis ture ne može biti duži od 2000 karaktera")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Težina ture je obavezna")]
    [Range(0, 3, ErrorMessage = "Težina ture mora biti između 0 i 3 (Easy, Moderate, Hard, Expert)")]
    public int Difficulty { get; set; }
    
    [Required(ErrorMessage = "Tagovi su obavezni")]
    [MinLength(1, ErrorMessage = "Mora postojati najmanje jedan tag")]
    public List<string> Tags { get; set; } = new List<string>();
    
    public List<CreateTourKeyPointRequestDto> KeyPoints { get; set; } = new List<CreateTourKeyPointRequestDto>();
    
    public List<CreateTourTransportTimeRequestDto> TransportTimes { get; set; } = new List<CreateTourTransportTimeRequestDto>();
    
    [Range(0, double.MaxValue, ErrorMessage = "Distance mora biti veća ili jednaka 0")]
    public decimal? DistanceInKm { get; set; }
}