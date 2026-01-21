using mapsence.Domain.Models;

namespace mapsencetoWebApi.Repository;

public interface IUserRepository
{
    Task<User?> FindByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> AddUserAsync(User user);
    Task UpdateUserAsync(User user);
}