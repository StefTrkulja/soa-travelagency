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
        User BlockUser(long userId);
        User UnblockUser(long userId);
        bool IsUserBlocked(long userId);
        User? GetUserProfile(long userId);
        User UpdateUserProfile(long userId, string? name, string? surname, string? profilePicture, string? biography, string? motto);
    }
}
