namespace MyTicket.Application.Models;

public class JwtConfig
{
    /// <summary>
    /// Secret key minimum 16 length
    /// </summary>
    public string SecretKey { get; set; }
    /// <summary>
    /// Duration in second. Default value is 3 days (259200 seconds)
    /// </summary>
    public int Duration { get; set; }

    public string Issuer { get; set; }

    public string Audience { get; set; }
}
