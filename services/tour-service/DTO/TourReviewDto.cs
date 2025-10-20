namespace TourService.DTO;

public class TourReviewDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long UserId { get; set; }
    public DateTime VisitationTime { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? Rating { get; set; }
    public string? TourName { get; set; }
}