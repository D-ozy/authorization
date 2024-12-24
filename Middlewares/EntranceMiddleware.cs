using System.Text;
using System.Security.Cryptography;
using authorization.Data;
using System.Text.Json;

namespace authorization.Middlewares
{
    public class EntranceMiddleware
    {
        private readonly RequestDelegate next;

        public EntranceMiddleware(RequestDelegate next) => this.next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            var request = context.Request;

            if (request.Path == "/entrance/get" && request.Method == "GET")
            {
                await GetUser(response, request);
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
            var password = request.Query["password"].ToString();

            Console.WriteLine($"Данные: {name}, {email}, {password}");

            string hashPass = Hash(password);

            Console.WriteLine($"Хешированный пароль: {hashPass}");

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(hashPass))
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                await response.WriteAsync(JsonSerializer.Serialize(new { error = "Имя или email не могут быть пустыми." }));
                return;
            }

            bool userExists = db.Users.Any(u => (u.Name == name || u.Email == email) && u.Password == hashPass);

            Console.WriteLine($"Ответ: {userExists}");

            response.ContentType = "application/json";
            await response.WriteAsync(JsonSerializer.Serialize(new { exists = userExists }));
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
