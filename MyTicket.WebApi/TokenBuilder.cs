using System.Security.Claims;
using MyTicket.Application.Constant;
using MyTicket.WebApi.Models.Response.Auth;
using MyTicket.WebApi.Services;
using Project.Application.Businesses.Auth.Models;

namespace MyTicket.WebApi;

public static class TokenBuilder
{
    public static AuthenticationResponse Build(ApplicationJwtManager applicationJwtManager, LoginResponse user)
    {
        if (applicationJwtManager is null)
        {
            throw new ArgumentNullException(nameof(applicationJwtManager));
        }

        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        List<Claim> claims = new List<Claim>();

        claims.Add(new Claim(ApplicationClaimConstant.UserId, user.UserId.ToString(), ClaimValueTypes.String));
        claims.Add(new Claim(ApplicationClaimConstant.UserName, user.UserName, ClaimValueTypes.String));
        claims.Add(new Claim(ApplicationClaimConstant.FullName, user.FullName, ClaimValueTypes.String));
        claims.Add(new Claim(ApplicationClaimConstant.Role, user.Role, ClaimValueTypes.String));

        return new AuthenticationResponse(applicationJwtManager.GenerateJwtToken(claims));
    }
}
