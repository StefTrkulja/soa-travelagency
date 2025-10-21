namespace StakeholdersService.Domain
{
    public class User
    {

        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Blocked { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Biography { get; set; }
        public string? Motto { get; set; }
        
        // Location properties for map tracking
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? LocationUpdatedAt { get; set; }

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

        public void UpdateProfile(string? name, string? surname, string? profilePicture, string? biography, string? motto)
        {
            Name = name;
            Surname = surname;
            ProfilePicture = profilePicture;
            Biography = biography;
            Motto = motto;
        }

        public void UpdateLocation(decimal latitude, decimal longitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90 degrees");
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180 degrees");
                
            Latitude = latitude;
            Longitude = longitude;
            LocationUpdatedAt = DateTime.UtcNow;
        }

        public void ClearLocation()
        {
            Latitude = null;
            Longitude = null;
            LocationUpdatedAt = null;
        }

        public bool HasLocation()
        {
            return Latitude.HasValue && Longitude.HasValue;
        }

        public (decimal lat, decimal lng)? GetLocation()
        {
            if (HasLocation())
                return (Latitude!.Value, Longitude!.Value);
            return null;
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
