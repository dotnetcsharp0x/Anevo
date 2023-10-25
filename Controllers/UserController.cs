using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Anevo.Models.User;
using Anevo.Data;
using Anevo.Handlers;

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

    [HttpGet("GetUser")]
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
    [Route("Register")]
    public async Task<string> Register(Users user)
    {
        _context.Users.Add(user);
        CreateJWTToken cjwttoken = new CreateJWTToken(user, _options);
        string resp = await cjwttoken.CreateToken();
        await _context.SaveChangesAsync();
        return resp;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    public async Task<string> Login(Users user)
    {
        var find_user = (from i in _context.Users where i.UserName == user.UserName select i).FirstOrDefault();
        if (find_user != null)
        {
            CreateJWTToken cjwttoken = new CreateJWTToken(user,_options);
            string resp = await cjwttoken.CreateToken();
            return resp;
        }
        else
        {
            return "User not found";
        }
    }
}
