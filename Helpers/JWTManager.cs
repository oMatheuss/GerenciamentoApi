using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GerenciamentoAPI.Helpers
{
    public class JWTManager
    {
        private readonly string JwtSecret;
        public JWTManager(string jwtSecret)
        {
            JwtSecret = jwtSecret;
        }

        public string GenerateToken(List<Claim> claims)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string? GetId(ClaimsPrincipal principal)
        {
            var id = principal.FindFirst(ClaimTypes.Sid);
            return id?.Value;
        }
    }
}
