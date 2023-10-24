using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Anevo.Models.User;
using Anevo.Data;

namespace Anevo.Controllers;

[Authorize]
[ApiController]
[Route("api/User")]
public class UserController : ControllerBase
{
    private readonly JWTSettings _options;
    private readonly ILogger<UserController> _logger;

    private readonly ApplicationContext _context;

    public UserController(ILogger<UserController> logger,IOptions<JWTSettings> optAccess,ApplicationContext context)
    {
        _logger = logger;
        _options = optAccess.Value;
        _context = context;
    }

    [HttpGet("getuser")]
    [AllowAnonymous]
    public IEnumerable<Users> GetUser(string username)
    {
        return _context.Users.Where(x => x.UserName == username);
    }
    
    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("admin")]
    public string Admin() {
        
        return "Ok";
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Authenticate")]
    public string Authenticate(Users user)
    {
        var find_user = (from i in _context.Users where i.UserName == user.UserName select i).FirstOrDefault();
        if (find_user != null)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, find_user.UserName)); // Передаем в токен Имя
            claims.Add(new Claim("level", "123")); // Передаем в токен кастомное поле
            claims.Add(new Claim(ClaimTypes.Role, find_user.UserName)); // Передаем в токен роль
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1000)), // Действие токена 10 минут
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            var resp = new JwtSecurityTokenHandler().WriteToken(jwt);
            return resp;
        }
        else
        {
            return "User not found";
        }
    }
}
