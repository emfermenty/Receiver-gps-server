using mapsence.Domain.Models;
using mapsencetoWebApi.Repository;
using Microsoft.AspNetCore.Identity;

namespace mapsence.Application.Services;

public class UserService(IUserRepository userRepository, JwtService jwtService) : IUserService
{
    public async Task<bool> Register(string username, string Pass)
    {
        var account = new User
        {
            Username = username,
            Pass = Pass
        };
        var passhash = new PasswordHasher<User>().HashPassword(account, Pass);
        account.Pass = passhash;
        return await userRepository.AddUserAsync(account);
    }

    public async Task<string> Login(string username, string Pass)
    {
        var account = await userRepository.FindByUsernameAsync(username);
        var result = new PasswordHasher<User>()
            .VerifyHashedPassword(account, account.Pass, Pass);
        if (result == PasswordVerificationResult.Success)
        {
            return jwtService.GenerateToken(account);
        }
        else
        {
            return "";
        }
    }

    public async Task ResetPassword(string username, string pass)
    {
        var account = await userRepository.FindByUsernameAsync(username);
        account.Pass = new PasswordHasher<User>().HashPassword(account, pass);
        await userRepository.UpdateUserAsync(account);
    }
}