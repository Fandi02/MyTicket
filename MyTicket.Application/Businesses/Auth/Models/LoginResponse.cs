namespace MyTicket.Application.Businesses.Auth.Models;

public class LoginResponse
{
    public Guid UserId { get; set; }   
    public string Email { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }
}
