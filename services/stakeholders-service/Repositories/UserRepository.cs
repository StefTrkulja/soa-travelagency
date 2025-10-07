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

        public User UpdateUserProfile(long userId, string? name, string? surname, string? profilePicture, string? biography, string? motto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found");

            user.UpdateProfile(name, surname, profilePicture, biography, motto);
            _dbContext.SaveChanges();
            return user;
        }
    }
}
