namespace MyTicket.WebApi.Endpoints.UserProfile.Models.Request;

public class UpdatePasswordModelRequest
{
    public string PasswordOld { get; set; }
    public string PasswordNew { get; set; }
    public string ConfirmPassword { get; set; }
}
