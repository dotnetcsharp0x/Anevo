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
using System.Net;
using RestSharp;
using Nancy.Json;

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

    [HttpGet]
    [Authorize(Roles = UserRolesTemplate.Admin)]
    [Route("GetUsers")]
    public IEnumerable<Users> GetUsers()
    {
        return _context.Users;
    }
    
    [HttpGet("GetUser")]
    public IEnumerable<Users> GetUser(string email)
    {
        return _context.Users.Where(x => x.Email == email);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register(Users user)
    {
        var find_user = (from i in _context.Users where i.Email == user.Email select i).FirstOrDefault();
        if (find_user == null)
        {
            _context.Users.Add(user);
            
            LoginTemplate loginTemplate = new LoginTemplate();
            loginTemplate.users = user;
            await _context.SaveChangesAsync();
            find_user = (from i in _context.Users where i.Email == user.Email select i).FirstOrDefault();
            _context.SU001.Add(new SU001 { SU001_Id_User = user.Id, SU001_GroupNr = 2 });
            await _context.SaveChangesAsync();
            var MyUrl = Request.Host.Value;
            string url = "https://"+MyUrl+"/api/User/Login";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(loginTemplate.users);
            string jwt_resp = client.ExecuteAsync(request).Result.Content;
            return Content(jwt_resp);
        }
        else
        {
            return StatusCode(208);
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login(Users user)
    {
        LoginTemplate login_template = new LoginTemplate();
        var find_user = (from i in _context.Users where i.Email == user.Email select i).FirstOrDefault();
        if (find_user != null)
        {
            login_template.users = find_user;
            var user_group = (from i in _context.SU001 where i.SU001_Id_User == find_user.Id select i).First();
            var group_data = (from i in _context.SU010 where i.SU010_Group_Nr == user_group.SU001_GroupNr select i).First();
            login_template.SU010 = group_data;
            login_template.SU001 = user_group;
            CreateJWTToken cjwttoken = new CreateJWTToken(login_template, _options);
            return Content(await cjwttoken.CreateToken());
        }
        else
        {
            return NotFound();
        }
    }
}
