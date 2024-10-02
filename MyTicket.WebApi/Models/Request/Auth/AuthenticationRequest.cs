namespace MyTicket.WebApi.Models.Request.Auth;

public class AuthenticationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ApplicationCode { get; set; }
}
