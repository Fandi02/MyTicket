using MyTicket.Domain.Entities;

namespace MyTicket.WebApi.Models.Response.UserProfile;

public class UserProfileResponse
{
    public Guid UserId { get; set; }   
    public string Email { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public UserRoleEnum Role { get; set; }
}