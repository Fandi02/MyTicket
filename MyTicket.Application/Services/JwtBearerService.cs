using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Models;

namespace MyTicket.Application.JwtBearer;

public class JwtBearerService : IJwtProvider
{
    public readonly JwtConfig _config;
    private readonly DateTime _dateObjectCreated;
    private readonly IConfiguration _configuration;

    public JwtBearerService(JwtConfig config, IConfiguration configuration)
    {
        _config = config;
        _configuration = configuration;
        _config.Duration = _configuration.GetValue<int>("JwtConfig:Duration");
        _config.SecretKey = _configuration.GetValue<string>("JwtConfig:SecretKey");
        _config.Issuer = _configuration.GetValue<string>("JwtConfig:Issuer");
        _config.Audience = _configuration.GetValue<string>("JwtConfig:Audience");
        _dateObjectCreated = DateTime.UtcNow;
    }

    //public JwtBearerService(IOptions<JwtConfig> config)
    //{
    //    _config = config.Value;
    //    _dateObjectCreated = DateTime.UtcNow;
    //}

    public string Protect(List<Claim> data, string issuer = null, string audience = null)
    {
        if (data is null || data.Count < 1)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (!data.Any(x => x.Type == ClaimTypes.NameIdentifier && x.ValueType == ClaimValueTypes.String))
        {
            throw new ArgumentException("Claim data must contains name identifier");
        }

        byte[] b = Encoding.UTF8.GetBytes(_config.SecretKey);
        SymmetricSecurityKey secretKey = new SymmetricSecurityKey(b);
        SigningCredentials credential = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512);
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: data,
            notBefore: _dateObjectCreated,
            expires: _dateObjectCreated.AddSeconds(_config.Duration),
            signingCredentials: credential);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal UnProtect(string token, string issuer = null, string audience = null)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
            ValidAudience = audience,

            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
            ValidIssuer = issuer,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey)),

            // most important
            ValidateLifetime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token: token, validationParameters: tokenValidationParameters, out var securityToken);

        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public ClaimsPrincipal UnProtectPassValidation(string token, string issuer = null, string audience = null)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = !string.IsNullOrWhiteSpace(audience),
            ValidAudience = audience,

            ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
            ValidIssuer = issuer,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey)),

            // most important
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token: token, validationParameters: tokenValidationParameters, out var securityToken);

        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
