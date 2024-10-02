using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyTicket.Application.Constant;

namespace MyTicket.Application.Models;
public class BaseAuthenticatedUser
{
    private readonly HttpContext _httpContext;

    public BaseAuthenticatedUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
        InitializeWithUserContext();
    }

    private void InitializeWithUserContext()
    {
        Id = Guid.Parse(_httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        Email = _httpContext.User.FindFirst(ApplicationClaimConstant.Email).Value;
        Username = _httpContext.User.FindFirst(ApplicationClaimConstant.UserName).Value;
        Fullname = _httpContext.User.FindFirst(ApplicationClaimConstant.FullName).Value;
        Role = _httpContext.User.FindFirst(ApplicationClaimConstant.Role).Value;
    }

    public BaseAuthenticatedUser(Guid id, string username)
    {
        if (id == null)
        {
            throw new ArgumentException("message", nameof(id));
        }
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("message", nameof(username));
        }

        Id = id;
        Username = username;
    }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Fullname { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
}
