namespace StakeholdersService.DTO
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Blocked { get; set; }
    }
}
