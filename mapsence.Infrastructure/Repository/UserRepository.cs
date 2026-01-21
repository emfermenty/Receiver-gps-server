using AutoMapper;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Data;
using mapsence.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace mapsencetoWebApi.Repository;

public class UserRepository(ApplicationContext context, IMapper _mapper) : IUserRepository
{
    public async Task<User?> FindByUsernameAsync(string username)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
        return _mapper.Map<User>(user);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await context.Users
            .AnyAsync(u => u.Username == username);
    }

    public async Task<bool> AddUserAsync(User user)
    {
        if (!await ExistsByUsernameAsync(user.Username))
        {
            await context.Users.AddAsync(_mapper.Map<UserEntity>(user));
            await context.SaveChangesAsync();
            return true;
        }
        else return false;
    }

    public async Task UpdateUserAsync(User user)
    {
        context.Users.Update(_mapper.Map<UserEntity>(user));
        await context.SaveChangesAsync();
    }
}