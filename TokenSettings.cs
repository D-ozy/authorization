using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace authorization
{
    public class TokenSettings
    {
        public const string ISSUER = "MyAuthServer"; //издатель токена
        public const string AUDIENCE = "localhost"; //потребитель токена
        private const string KEY = "mysupersecret_secretsecretsecretkey!123"; //ключ для шифрации

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }
    }
}