namespace StakeholdersService.DTO
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Motto { get; set; }
        public string? Biography { get; set; }
        public string Role { get; set; }
    }
}