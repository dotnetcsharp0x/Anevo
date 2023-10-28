using Anevo.Models;
using Anevo.Models.Tables.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Anevo.Actions.JWT
{
    public class CreateJWTToken
    {
        private SU_001 _users;
        private JWTSettings _options;
        private LoginTemplate _loginTemplate;
        public CreateJWTToken(LoginTemplate users, JWTSettings options)
        {
            SU_001 c_user = new SU_001();
            c_user.Email = users.SU_001.Email;
            c_user.Password = users.SU_001.Password;
            _users = c_user;
            _options = options;
            _loginTemplate = users;
        }
        public async Task<string> CreateToken()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, _users.Email)); // Передаем в токен Имя
            claims.Add(new Claim(ClaimTypes.GroupSid, _loginTemplate.SG_001.SG001_GroupNr.ToString())); // Передаем в токен кастомное поле
            claims.Add(new Claim(ClaimTypes.Role, _loginTemplate.SG_010.SU010_Name)); // Передаем в токен роль
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1000)), // Действие токена 1000 минут
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
