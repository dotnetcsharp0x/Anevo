using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Anevo.Models.User;

namespace Anevo.Controllers;

[Authorize]
[ApiController]
[Route("api/Test")]
public class TestController : ControllerBase
{
    private readonly JWTSettings _options;
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger,IOptions<JWTSettings> optAccess)
    {
        _logger = logger;
        _options = optAccess.Value;
    }

    [HttpGet("getuser")]
    public IEnumerable<Users> GetUser()
    {
        List<Users> usr = new List<Users>();
        Users us = new Users(); // Создаем пустышку для вывода в API
        us.Id=1;
        us.FirstName="Dmitry";
        us.LastName="Sotnikov";
        us.UserName="rubi";
        us.Password="rubi1234";
        usr.Add(us);
        return usr;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Authenticate")]
    public string Authenticate(int minutes)
    {
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name,"Dmitriy")); // Передаем в токен Имя
        claims.Add(new Claim("level","123")); // Передаем в токен кастомное поле
        claims.Add(new Claim(ClaimTypes.Role,"Admin")); // Передаем в токен роль
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var jwt = new JwtSecurityToken (
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(minutes)), // Действие токена 10 минут
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256)
        );
        var resp = new JwtSecurityTokenHandler().WriteToken(jwt);
        return resp;
    }
}
