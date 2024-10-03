namespace MyTicket.WebApi.Endpoints.Auth.Models.Request;

public class LoginModelRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ApplicationCode { get; set; }
}
