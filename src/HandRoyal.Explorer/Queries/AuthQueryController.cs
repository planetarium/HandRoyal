using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using HandRoyal.Explorer.Jwt;
using HandRoyal.Wallet.Interfaces;
using Libplanet.Crypto;
using Microsoft.AspNetCore.Http;

namespace HandRoyal.Explorer.Queries;

internal sealed class AuthQueryController(
    IWalletService walletService,
    IHttpContextAccessor httpContextAccessor,
    JwtValidator jwtValidator)
    : GraphController
{
    [QueryRoot("getUserAddress")]
    public async Task<Address?> GetUserAddress()
    {
        // Validate the JWT token
        if (!httpContextAccessor.IsValidToken(jwtValidator))
        {
            throw new UnauthorizedAccessException("Invalid or missing authentication token");
        }

        try
        {
            return await walletService.GetAddressAsync(httpContextAccessor.UserId());
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
