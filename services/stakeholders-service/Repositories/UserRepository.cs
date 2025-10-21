using StakeholdersService.Database;
using StakeholdersService.Domain;
using StakeholdersService.Domain.RepositoryInterfaces;

namespace StakeholdersService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly StakeholdersContext _dbContext;

        public UserRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }
        public bool Exists(string username)
        {
            return _dbContext.Users.Any(user => user.Username == username);
        }

        public User Create(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }


        public User? GetById(long userId)
        {
            var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId);
            return user;
        }

        public bool IsAuthor(long userId)
        {
            var user = _dbContext.Users.FirstOrDefault(i => i.Id == userId);
            return user?.Role == UserRole.Author;
        }

        public List<User> GetPaged(int page, int pageSize, out int totalCount)
        {
            totalCount = _dbContext.Users.Count();

            return _dbContext.Users
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }


        public User? GetActiveByName(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(i => i.Username == username);
            return user;
        }


        public User Update(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
            return user;
        }

        public User BlockUser(long userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");
            
            user.Block();
            _dbContext.SaveChanges();
            return user;
        }

        public User UnblockUser(long userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");
            
            user.Unblock();
            _dbContext.SaveChanges();
            return user;
        }

        public bool IsUserBlocked(long userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            return user?.IsBlocked() ?? false;
        }

        public User? GetUserProfile(long userId)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        public User UpdateUserProfile(long userId, string email, string? name, string? surname, string? profilePicture, string? biography, string? motto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            user.Email = email;
            user.UpdateProfile(name, surname, profilePicture, biography, motto);
            _dbContext.SaveChanges();
            return user;
        }

        public User UpdatePassword(long userId, string currentPassword, string newPassword)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            if (user.Password != currentPassword)
                throw new ArgumentException("Current password is incorrect");

            user.Password = newPassword;
            _dbContext.SaveChanges();
            return user;
        }

        public User UpdateEmail(long userId, string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            user.Email = email;
            _dbContext.SaveChanges();
            return user;
        }

        public List<User> GetAllUsers()
        {
            return _dbContext.Users.ToList();
        }

        public User UpdateUserLocation(long userId, decimal latitude, decimal longitude)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            user.UpdateLocation(latitude, longitude);
            _dbContext.SaveChanges();
            return user;
        }

        public User? GetUserLocation(long userId)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        }

        public User ClearUserLocation(long userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            user.ClearLocation();
            _dbContext.SaveChanges();
            return user;
        }

        public List<User> GetUsersWithLocationInRadius(decimal centerLat, decimal centerLng, double radiusKm)
        {
            // Get all users with location data
            var usersWithLocation = _dbContext.Users
                .Where(u => u.Latitude.HasValue && u.Longitude.HasValue)
                .ToList();

            // Filter by distance using Haversine formula
            return usersWithLocation.Where(user =>
            {
                var distance = CalculateDistance(
                    (double)centerLat, (double)centerLng,
                    (double)user.Latitude!.Value, (double)user.Longitude!.Value);
                return distance <= radiusKm;
            }).ToList();
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;
            
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return EarthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
