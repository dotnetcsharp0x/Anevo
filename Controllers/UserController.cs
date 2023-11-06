using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Anevo.Data;
using System.Net;
using RestSharp;
using Nancy.Json;
using Anevo.Actions.Groups;
using Anevo.Actions.Users;
using Microsoft.EntityFrameworkCore;
using Anevo.Actions.JWT;
using System;
using Newtonsoft.Json.Serialization;
using Skymey_main_lib.Models.JWT;
using Skymey_main_lib.Models.Tables.User;
using Skymey_main_lib.Interfaces.JWT;
using Skymey_main_lib.Models;
using Skymey_main_lib.Enums.SU0010;

namespace Anevo.Controllers;

[Authorize]
[ApiController]
[Route("api/User")]
public class UserController : ControllerBase
{
    private readonly JWTSettings _options;
    private readonly ILogger<UserController> _logger;
    private UserActions _userActions;
    private UserGroups _userGroups;
    private readonly ApplicationContext _context;
    private readonly ITokenService _tokenService;
    private readonly IOptions<JWTSettings> _config;

    public UserController(ILogger<UserController> logger,IOptions<JWTSettings> optAccess,ApplicationContext context, ITokenService tokenService, IOptions<JWTSettings> config)
    {
        _logger = logger;
        _options = optAccess.Value;
        _context = context;
        _userActions = new UserActions(_context);
        _userGroups = new UserGroups(_context);
        _tokenService = tokenService;
        _config = config;
    }

    [HttpGet]
    [Authorize(Roles = UserRolesTemplate.Admin)]
    [Route("GetUsers")]
    public async Task<HashSet<SU_001>> GetUsers()
    {
        var resp = await _userActions.GetUsers();
        return resp.ToHashSet();
    }
    
    [HttpGet("GetUserByEmail")]
    public async Task<SU_001> GetUserByEmail(string email)
    {
        return await _userActions.GetUserByEmail(email);
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register(SU_001 user)
    {
        var find_user = await _userActions.GetUserByEmail(user.Email);
        if (find_user == null)
        {
            await _userActions.CreateUser(user);
            LoginTemplate loginTemplate = new LoginTemplate();
            loginTemplate.SU_001 = user;
            find_user = await _userActions.GetUserByEmail(user.Email);
            await _userGroups.AddUserToGroup(loginTemplate.SU_001,SU010_Types.User);
            var jwt_resp = Login(loginTemplate.SU_001).Result.ExecuteResultAsync;
            var resp = (OkObjectResult)jwt_resp.Target;
            var aresp = resp.Value;
            return Ok(aresp);
        }
        else
        {
            return StatusCode(208);
        }
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login(SU_001 user)
    {
        LoginTemplate login_template = new LoginTemplate();
        var find_user = await _userActions.GetUserByEmail(user.Email);
        if (find_user != null)
        {
            login_template.SU_001 = find_user;
            var user_group = await _userGroups.GetUserInGroup(find_user.Id);
            var group_data = await _userGroups.GetGroup(user_group.SG001_GroupNr);
            login_template.SG_010 = group_data;
            login_template.SG_001 = user_group;
            CreateJWTToken cjwttoken = new CreateJWTToken(_config);
            AuthenticatedResponse aresp = new AuthenticatedResponse();
            List<Claim> claims = new List<Claim>
            {
                new Claim("Name", find_user.Email),
                new Claim(ClaimTypes.Name, find_user.Email),
                new Claim("GroupSid", login_template.SG_001.SG001_GroupNr.ToString()), 
                new Claim(ClaimTypes.GroupSid, login_template.SG_001.SG001_GroupNr.ToString()), 
                new Claim("Role", login_template.SG_010.SU010_Name),
                new Claim(ClaimTypes.Role, login_template.SG_010.SU010_Name)
            };
            aresp.AccessToken = cjwttoken.GenerateAccessToken(claims);
            aresp.RefreshToken = cjwttoken.GenerateRefreshToken();
            find_user.RefreshToken = aresp.AccessToken;
            find_user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(43200);
            await _context.SaveChangesAsync();
            return Ok(aresp);
        }
        else
        {
            return NotFound();
        }
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("ReadJWT")]
    public IEnumerable<JWTModel> ReadJWT(string jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoic3RyaW5nIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9ncm91cHNpZCI6IjEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsIm5iZiI6MTY5ODgyOTE2NywiZXhwIjoxNjk4OTE1NTY3LCJpc3MiOiJteWhvbGQiLCJhdWQiOiJteWhvbGQifQ.SeX-GrYtMEDdm-nzjystXbQjdLpccndGrODQJHJv7k8")
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        var resp = (from i in token.Claims select new JWTModel { type = i.Type, value = i.Value}).Where(x=>x.type.Contains("identity")).ToList();
        foreach(var x in resp)
        {
            x.type = new Uri(x.type).Segments.Last();
        }

        return resp;
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(TokenApiModel tokenApiModel)
    {
        if (tokenApiModel is null)
            return BadRequest("Invalid client request");
        string accessToken = tokenApiModel.AccessToken;
        string refreshToken = tokenApiModel.RefreshToken;
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        var email = (from i in principal.Claims where i.Type=="Name" select i.Value).First();
        var user = _userActions.GetUserByEmail(email).Result;
        if (user is null || user.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("Invalid client request");
        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newAccessToken;
        await _context.SaveChangesAsync();
        return Ok(new AuthenticatedResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
    [HttpPost]
    [Route("revoke")]
    public async Task<IActionResult> Revoke()
    {
        var email = User.Identity.Name;
        var user = _userActions.GetUserByEmail(email).Result;
        if (user == null) return BadRequest();
        user.RefreshToken = null;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
