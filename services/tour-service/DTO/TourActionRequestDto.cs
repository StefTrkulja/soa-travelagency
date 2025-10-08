using System.ComponentModel.DataAnnotations;

namespace TourService.DTO;

public class PublishTourRequestDto
{
    [Required(ErrorMessage = "ID ture je obavezan")]
    public long TourId { get; set; }
}

public class ArchiveTourRequestDto
{
    [Required(ErrorMessage = "ID ture je obavezan")]
    public long TourId { get; set; }
}

public class ActivateTourRequestDto
{
    [Required(ErrorMessage = "ID ture je obavezan")]
    public long TourId { get; set; }
}
