using authorization.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.StaticFiles;
using authorization.Middlewares;

namespace authorization
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();

            //параметры валидации токена
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // указывает, будет ли валидироваться издатель при валидации токена
                        ValidateIssuer = true,
                        // строка, представляющая издателя
                        ValidIssuer = TokenSettings.ISSUER,
                        // будет ли валидироваться потребитель токена
                        ValidateAudience = true,
                        // установка потребителя токена
                        ValidAudience = TokenSettings.AUDIENCE,
                        // будет ли валидироваться время существования
                        ValidateLifetime = true,
                        // установка ключа безопасности
                        IssuerSigningKey = TokenSettings.GetSymmetricSecurityKey(),
                        // валидация ключа безопасности
                        ValidateIssuerSigningKey = true,
                    };
                });

            WebApplication app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseMiddleware<RegistrationMiddleware>();
            app.UseMiddleware<EntranceMiddleware>();

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html; charset=utf-8";

                if(context.Request.Path == "/")
                {
                    context.Response.Redirect("/Front/registration/reg.html");
                }

                await context.Response.WriteAsync("Дароу");
               
            });

            app.Run();
        }

    }
}
