using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace HandRoyal.Explorer.Jwt;

public static class JwtHelper
{
    public static string? UserId(this string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwtToken))
        {
            return null;
        }

        var token = handler.ReadJwtToken(jwtToken);
        return token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
    }

    public static string UserId(this IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var authHeader = httpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length);
            var userId = UserId(token);
            return userId ?? "Unknown";
        }

        return "No Auth Header";
    }

    public static bool IsValidToken(
        this IHttpContextAccessor httpContextAccessor, JwtValidator jwtValidator)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var authHeader = httpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length);
            var principal = jwtValidator.ValidateToken(token);
            return principal != null;
        }

        return false;
    }
}
