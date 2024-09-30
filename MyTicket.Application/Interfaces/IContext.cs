namespace MyTicket.Application.Interfaces;

public interface IContext
{
    bool IsAuthenticated { get; }
    public string UserId { get; }
    public Guid UserIdGuid { get; }
    public string UserName { get; }
    public string FullName { get; }
    public string Email { get; }
    public string Roles { get; }
}