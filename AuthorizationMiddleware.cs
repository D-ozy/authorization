using authorization.Data;
using authorization.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace authorization
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate next;

        public AuthorizationMiddleware(RequestDelegate next) { this.next = next; }

        
        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            var request = context.Request;


            if (request.Path == "/auth/check" && request.Method == "GET")
            {
                await GetUser(response, request);
            }
            else if (request.Path == "/auth/add" && request.Method == "POST") 
            {
                await AddUser(response, request); 
            }
            else
            {
                await next.Invoke(context);
            }
        }

        private async Task GetUser(HttpResponse response, HttpRequest request)
        {
            AuthorizationDbContext db = new AuthorizationDbContext();

            var name = request.Query["name"].ToString();
            var email = request.Query["email"].ToString();

            
            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(email))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                await response.WriteAsync(JsonSerializer.Serialize(new { error = "Имя или email не могут быть пустыми." }));
                return;
            }

            bool userExists = db.Users.Any(u => u.Name == name || u.Email == email);

            response.ContentType = "application/json";
            await response.WriteAsync(JsonSerializer.Serialize(new { exists = userExists }));
        }


        private async Task AddUser(HttpResponse response, HttpRequest request)
        {
            try
            {
                User? user = await request.ReadFromJsonAsync<User>();

                if (user != null)
                {
                    using (AuthorizationDbContext db = new AuthorizationDbContext())
                    {
                        user.Id = Guid.NewGuid().ToString();
                        user.Password = Hash(user.Password);
                        db.Users.Add(user);
                        db.SaveChanges();


                        //создание токена
                        var tokenGenerator = new Token(new TokenSettings());

                        var token = tokenGenerator.GenerateToken(user.Id, user.Email);
                        response.StatusCode = 200;

                        await response.WriteAsJsonAsync(token);

                        Console.WriteLine(token);
                    }
                }
                else
                {
                    throw new Exception("Uncorrected Data");
                }
            }
            catch (Exception ex) 
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = ex });
            }
        }



        private string Hash(string pass)
        {
            byte[] temp = Encoding.UTF8.GetBytes(pass);

            using (SHA256 sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(temp);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
