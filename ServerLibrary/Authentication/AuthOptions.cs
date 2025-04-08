using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ServerLibrary.Authentication
{
    public static class AuthOptions
    {
        public const string ISSUER = "SilkRoadServer";
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        private const string KEY = "2EC1EE57-1100-4347-BF22-EEB6CB8B0695";   // ключ для шифрации
        public const int LIFETIME = 90;
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
