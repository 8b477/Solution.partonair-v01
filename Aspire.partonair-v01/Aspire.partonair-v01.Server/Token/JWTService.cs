using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.partonair_v01.Token
{
    public class JWTService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly string _secretKey;

        public JWTService(IOptions<JwtSettings> jwtOptions) // IOptions lis les fichiers de configuration et je précise la section que je veux lire (JwtSettings) et me bind à la classe JwtSettings
        {
            _jwtSettings = jwtOptions.Value; // injecte les valeurs récup dans mon objet

            if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JwtSettings:SecretKey is missing.");
            }

            _secretKey = _jwtSettings.SecretKey;
        }

        public string GenerateToken(string userId, string role)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyBytes = Convert.FromBase64String(_secretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    Subject = new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.Role, role)
                    ]),
                    Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(keyBytes),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Token generation failed: " + ex.Message, ex);
            }
        }
    }
}