using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyTicket.Application.Extensions;
using MyTicket.Application.Interfaces;

namespace MyTicket.WebApi.Services;

public class ApplicationJwtManager
{
    private readonly WebAppConfig _option;
    private readonly IClock _clock;
    private readonly IConfiguration _configuration;

    public ApplicationJwtManager(IOptions<WebAppConfig> options, IClock clock, IConfiguration configuration)
    {
        _configuration = configuration;
        _option = options.Value;
        _option.SecretKey = _configuration.GetValue<string>("JwtConfig:SecretKey");
        if (string.IsNullOrWhiteSpace(_option.SecretKey))
        {
            throw new ArgumentException("Secret key is null");
        }

        _clock = clock;
    }

    public string GenerateJwtToken(List<Claim> claims)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        byte[] key = Encoding.UTF8.GetBytes(_option.SecretKey);

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims: claims);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,

            // generate token that is valid for 1 days
            Expires = _clock.CurrentServerDate().AddDays(1),

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateStudentJwtToken(List<Claim> claims)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        byte[] key = Encoding.UTF8.GetBytes(_option.SecretKey);

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims: claims);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,

            // generate token that is valid for 15 minutes
            Expires = _clock.CurrentServerDate().AddMinutes(15),

            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public JwtSecurityToken ValidateToken(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes(_option.SecretKey);
        _ = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;

        return jwtToken;
    }
}
