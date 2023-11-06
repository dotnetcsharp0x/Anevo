using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Anevo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.OpenApi.Models;
using Anevo.Data;
using Anevo.Actions.JWT;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using Skymey_main_lib.Interfaces.JWT;

var builder = WebApplication.CreateBuilder(args);
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));


builder.WebHost.UseUrls("http://localhost:5000;https://localhost:5001;");

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(option => {
    option.SwaggerDoc("v1",new OpenApiInfo { Title = "myHoldAPI", Version = "v1"});
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            }, new string[]{}
        }
    });
});


#region Auth
builder.Services.AddOptions();
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
builder.Services.AddTransient<ITokenService, CreateJWTToken>();
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
