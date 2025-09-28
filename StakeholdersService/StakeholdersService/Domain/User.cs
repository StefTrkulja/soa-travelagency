namespace StakeholdersService.Domain
{
    public class User
    {

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Blocked { get; set; } = false;
        
        // Profile fields
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Motto { get; set; }
        public string? Biography { get; set; }

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

        public User(string username, string password, string email, string name, string surname, UserRole role, bool blocked = false)
        {
            Username = username;
            Password = password;
            Email = email;
            Name = name;
            Surname = surname;
            Role = role;
            Blocked = blocked;
            ProfilePicture = null;
            Motto = null;
            Biography = null;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Username)) throw new ArgumentException("Invalid Username");
            if (string.IsNullOrWhiteSpace(Password)) throw new ArgumentException("Invalid Password");
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
            if (string.IsNullOrWhiteSpace(Surname)) throw new ArgumentException("Invalid Surname");
        }


    }

    public enum UserRole
    {
        Administrator,
        Author,
        Tourist
    }
}
