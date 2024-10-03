namespace MyTicket.WebApi.Models.Request.UserProfile;

public class UpdatePasswordRequest
{
    public string PasswordOld { get; set; }
    public string PasswordNew { get; set; }
    public string ConfirmPassword { get; set; }
}
