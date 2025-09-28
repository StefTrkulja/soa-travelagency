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
            if (user.Role == UserRole.Author) return true;
            return false;
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

    }
}
