using MyTicket.Domain.Entities;

namespace MyTicket.WebApi.Models.Request.Auth;

public class RegistrationRequest
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public UserRoleEnum UserRole { get; set; }
}
