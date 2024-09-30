using Microsoft.AspNetCore.Http;
using MyTicket.Application.Interfaces;
using System.Security.Claims;

namespace MyTicket.Application.Services
{
    public class ContextService : IContext
    {
        public ContextService(IHttpContextAccessor httpContextAccessor)
        {
            var claimPrincipal = httpContextAccessor.HttpContext?.User;
            UserId = claimPrincipal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        public string UserId { get; }
        public Guid UserIdGuid => UserId != string.Empty ? Guid.Parse(UserId) : Guid.Empty;
        public bool IsAuthenticated { get; }
        public string UserName { get; }
        public string FullName { get; }
        public string Email { get; }
        public string Roles { get; }
    }
}
