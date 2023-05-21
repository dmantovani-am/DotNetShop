using Azure.Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
namespace DotNetShop.Data;

public class TokenRepository : ITokenRepository
{
    readonly IConfiguration _configuration;
    IHttpContextAccessor _httpContextAccessor;

    public TokenRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor; 
    }

    public Token GenerateToken(int days)
    {
        var user = _httpContextAccessor.HttpContext?.User ?? throw new NullReferenceException("Missing User");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecretKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
          claims: user.Claims,
          expires: DateTime.Now.AddDays(days),
          signingCredentials: creds);

        return new()
        {
            Value = new JwtSecurityTokenHandler().WriteToken(token),
            ValidFrom = token.ValidFrom,
            ValidTo = token.ValidTo,
        };
    }
}
