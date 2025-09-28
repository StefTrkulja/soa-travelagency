namespace StakeholdersService.Domain.RepositoryInterfaces
{
    public interface IUserRepository
    {
        bool Exists(string username);
        User Create(User user);
        User? GetById(long userId);
        bool IsAuthor(long userId);
        List<User> GetPaged(int page, int pageSize, out int totalCount);
        User Update(User user);
        User? GetActiveByName(string username);
        bool BlockUser(long userId);
        bool UnblockUser(long userId);
        User? GetProfileById(long userId);
        User UpdateProfile(User user);


    }
}
