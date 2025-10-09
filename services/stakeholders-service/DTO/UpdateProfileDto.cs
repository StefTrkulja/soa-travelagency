namespace StakeholdersService.DTO
{
    public class UpdateProfileDto
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Biography { get; set; }
        public string? Motto { get; set; }
    }
}