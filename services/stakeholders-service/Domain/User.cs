namespace StakeholdersService.Domain
{
    public class User
    {

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Blocked { get; set; }

        public UserRole Role { get; set; }


        public User() { }

        public string GetPrimaryRoleName()
        {
            return Role switch
            {
                UserRole.Administrator => "Administrator",
                UserRole.Tourist => "Tourist",
                UserRole.Author => "Author",
                _ => "Unknown"
            };

        }

        public User(string username, string password, string email, UserRole role, bool blocked = false)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            Blocked = blocked;
        }

        public void Block()
        {
            Blocked = true;
        }

        public void Unblock()
        {
            Blocked = false;
        }

        public bool IsBlocked()
        {
            return Blocked;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Username)) throw new ArgumentException("Invalid Username");
            if (string.IsNullOrWhiteSpace(Password)) throw new ArgumentException("Invalid Password");
        }


    }

    public enum UserRole
    {
        Administrator,
        Author,
        Tourist
    }
}
