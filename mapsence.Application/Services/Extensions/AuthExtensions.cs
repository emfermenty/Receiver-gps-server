using System.Text;
using mapsence.Application.Services.JwtSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace mapsence.Application.Services.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection servicesCollection, IConfiguration configuration)
    {
        var authSettings = configuration.GetSection(nameof(AuthSettings))
            .Get<AuthSettings>();
        servicesCollection
            .AddAuthorization(x =>
                x.AddPolicy("EveryOne", builder => builder.RequireClaim("User", "Everyone")))
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authSettings.SecretKey))
                };
            });
        return servicesCollection;
    }
}