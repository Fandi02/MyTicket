using MyTicket.Domain.Entities.Auth;

namespace MyTicket.Application.Businesses.UserProfile.Models;

public class GetProfileResponse
{
    public Guid UserId { get; set; }   
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public DateTime BirthDate { get; set; }
    public UserRoleEnum Role { get; set; }
}
