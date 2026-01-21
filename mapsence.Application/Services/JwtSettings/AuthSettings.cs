namespace mapsence.Application.Services.JwtSettings;

public class AuthSettings
{
    public TimeSpan Expires { get; set; }
    public string SecretKey { get; set; }
}