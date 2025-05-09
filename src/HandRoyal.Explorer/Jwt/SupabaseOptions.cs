namespace HandRoyal.Explorer.Jwt;

public class SupabaseOptions
{
    public string JwtSecret { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;
}
