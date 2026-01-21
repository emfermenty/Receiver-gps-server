using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using mapsence.Application.Services.JwtSettings;
using mapsence.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace mapsence.Application.Services;

public class JwtService(IOptions<AuthSettings> options)
{
    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("UserName", user.Username),
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("User", "Everyone")
        };
        var JwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.Add(options.Value.Expires),
            claims: claims,
            signingCredentials:
            new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                SecurityAlgorithms.HmacSha256
            ));
        return new JwtSecurityTokenHandler().WriteToken(JwtToken);
    }   
}