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
        User UpdateUserProfile(long userId, string email, string? name, string? surname, string? profilePicture, string? biography, string? motto);
        User UpdatePassword(long userId, string currentPassword, string newPassword);
        User UpdateEmail(long userId, string email);
        List<User> GetAllUsers();
        
        // Location methods
        User UpdateUserLocation(long userId, decimal latitude, decimal longitude);
        User? GetUserLocation(long userId);
        User ClearUserLocation(long userId);
        List<User> GetUsersWithLocationInRadius(decimal centerLat, decimal centerLng, double radiusKm);
    }
}
