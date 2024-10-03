using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Extensions;
using MyTicket.Application.Services;

namespace MyTicket.Application.Infrastructure;

public class CustomJwtAuthenticationHandler : AuthenticationHandler<CustomJwtAuthenticationOptions>
{
    private readonly ApplicationJwtManagerService _jwtManager;

    public CustomJwtAuthenticationHandler(
        IOptionsMonitor<CustomJwtAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ApplicationJwtManagerService jwtManager) : base(options, logger, encoder, clock)
    {
        _jwtManager = jwtManager;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endpoint = Context.GetEndpoint();
        
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            // Skip authentication
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string token = Context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!token.NotNullOrEmpty()) throw new UnauthorizeException();

        try
        {
            JwtSecurityToken jwtToken = _jwtManager.ValidateToken(token);

            ClaimsIdentity userIdentity = new ClaimsIdentity(claims: jwtToken.Claims, authenticationType: CustomJwtAuthenticationOptions.DefaultSchemeName);

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(userIdentity), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            throw new UnauthorizeException();
        }
    }
}
