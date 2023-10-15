using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Anevo;
using Anevo.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

#region Auth
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings")); // Сопоставление JWTSettings с файлом конфигурации appsettings.json

var secretKey = builder.Configuration.GetSection("JWTSettings:SecretKey").Value; // Секретный код из appsettings.json
var issuer = builder.Configuration.GetSection("JWTSettings:Issuer").Value; // Издатель токена. Можно указать любое название
var audience = builder.Configuration.GetSection("JWTSettings:Audience").Value; // Пользователь токена. Можно указать любое название

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); 


builder.Services.AddAuthentication(option => { // Указываем аутентификацию с помощью токенов
   option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
   option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; 
})
.AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters { // Задаем параметры валидации токена. Нужно проверять: Издатель, потребитель, ключ, срок действия
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        IssuerSigningKey = signingKey,
        ValidateIssuerSigningKey = true,
        LifetimeValidator = CustomLifetime.CustomLifetimeValidator
    };
});
#endregion

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
