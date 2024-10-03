using MyTicket.Domain.Entities;

namespace MyTicket.WebApi.Endpoints.UserProfile.Models.Response;

public class GetUserProfileResponse
{
    public Guid UserId { get; set; }   
    public string Email { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public UserRoleEnum Role { get; set; }
}