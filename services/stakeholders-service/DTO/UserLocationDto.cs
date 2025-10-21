using System.ComponentModel.DataAnnotations;

namespace StakeholdersService.DTO
{
    public class UserLocationDto
    {
        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90 degrees")]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180 degrees")]
        public decimal Longitude { get; set; }
    }

    public class UserLocationResponseDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? LocationUpdatedAt { get; set; }
        public bool HasLocation { get; set; }
    }
}