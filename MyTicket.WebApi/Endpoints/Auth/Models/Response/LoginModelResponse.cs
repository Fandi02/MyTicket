namespace MyTicket.WebApi.Endpoints.Auth.Models.Response;

public class LoginModelResponse
{
    public LoginModelResponse()
    {
        Scheme = "bearer";
    }

    public LoginModelResponse(string token) : this()
    {
        Token = token;
    }

    public LoginModelResponse(string scheme, string token, string refreshToken, DateTime? refreshTokenExpiredOn)
    {
        Scheme = scheme;
        Token = token;
        RefreshToken = refreshToken;
        RefreshTokenExpiredOn = refreshTokenExpiredOn;
    }

    public string Scheme { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiredOn { get; set; }
}
