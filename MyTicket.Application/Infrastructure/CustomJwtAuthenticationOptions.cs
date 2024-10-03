using Microsoft.AspNetCore.Authentication;

namespace MyTicket.Application.Infrastructure;

public class CustomJwtAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultSchemeName = "rl";
}
