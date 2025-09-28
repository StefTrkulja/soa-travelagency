namespace StakeholdersService.DTO
{
    public class UpdateUserProfileDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Motto { get; set; }
        public string? Biography { get; set; }
    }
}