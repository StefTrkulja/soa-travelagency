using TourService.Domain;

namespace TourService.DTO;

public class TourDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public long AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public decimal? DistanceInKm { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public List<TourKeyPointDto> KeyPoints { get; set; } = new List<TourKeyPointDto>();
    public List<TourTransportTimeDto> TransportTimes { get; set; } = new List<TourTransportTimeDto>();
}