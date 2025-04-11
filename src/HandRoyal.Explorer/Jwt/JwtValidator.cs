using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HandRoyal.Explorer.Jwt;

public class JwtValidator(IOptions<SupabaseOptions> options)
{
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var option = options.Value;
        var key = Encoding.UTF8.GetBytes(option.JwtSecret);

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = option.Issuer,

            ValidateAudience = true,
            ValidAudience = option.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            RequireSignedTokens = true,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParams, out _);
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
