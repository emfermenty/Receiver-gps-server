namespace mapsence.Application.Services;

public interface IUserService
{
    Task<bool> Register(string username, string Pass);
    Task<string> Login(string username, string Pass);
    Task ResetPassword(string username, string Pass);
}