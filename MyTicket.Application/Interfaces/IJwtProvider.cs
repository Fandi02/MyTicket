using System.Security.Claims;

namespace MyTicket.Application.Interfaces;

public interface IJwtProvider
{
    string Protect(List<Claim> data, string issuer = null, string audience = null);

    ClaimsPrincipal UnProtect(string token, string issuer = null, string audience = null);

    ClaimsPrincipal UnProtectPassValidation(string token, string issuer = null, string audience = null);
}
