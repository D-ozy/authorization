using authorization.Data;
using authorization.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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


            if (request.Path == "/auth/add" && request.Method == "POST") 
            {
                await AddUser(response, request); 
            }
            else
            {
                await next.Invoke(context);
            }
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

                        if(!db.Users.Any(x => x.Name == user.Name) && !db.Users.Any(x => x.Email == user.Email))
                        {
                            db.Users.Add(user);
                            db.SaveChanges();
                        }
                        else if (request.Path == "/auth/check" && request.Method == "GET")
                        {
                            await CheckUserExists(response, request);
                        }
                        else
                        {
                            await response.WriteAsJsonAsync(new { exMessage = "userExist" });
                        }
                        


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


        private async Task CheckUserExists(HttpResponse response, HttpRequest request)
        {
            try
            {
                var name = request.Query["name"];
                var email = request.Query["email"];

                using (AuthorizationDbContext db = new AuthorizationDbContext())
                {
                    bool userExists = db.Users.Any(u => u.Name == name || u.Email == email);

                    if (userExists)
                    {
                        response.StatusCode = 200; // OK
                        await response.WriteAsJsonAsync(new { exists = true });
                    }
                    else
                    {
                        response.StatusCode = 200; // OK
                        await response.WriteAsJsonAsync(new { exists = true });
                    }
                }
            }
            catch (Exception ex) 
            {
                response.StatusCode = 500; // Internal Server Error
                await response.WriteAsJsonAsync(new { message = ex.Message });
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
