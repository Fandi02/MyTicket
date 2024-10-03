namespace MyTicket.WebApi.Endpoints.UserProfile.Models.Request;

public class UpdateProfileModelRequest
{
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
}
