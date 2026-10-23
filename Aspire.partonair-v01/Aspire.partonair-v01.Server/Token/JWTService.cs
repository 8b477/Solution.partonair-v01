using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.partonair_v01.Token
{
    /// <summary>
    /// Génère et valide les jetons JWT de l'application.
    /// La configuration est injectée via <see cref="IOptions{JwtSettings}"/>
    /// et provient de la section <c>JWT</c>.
    /// </summary>
    public class JWTService
    {
        private readonly JwtSettings _jwtSettings;

        public JWTService(IOptions<JwtSettings> jwtOptions) 
        {
            _jwtSettings = jwtOptions.Value;

            if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JwtSettings:SecretKey is missing.");
            }
        }

        public string GenerateToken(string userId, string role)
        {
            try
            {
                var now = DateTime.UtcNow;
                var expires = now.AddHours(_jwtSettings.ExpiryHours);

                var tokenHandler = new JwtSecurityTokenHandler();
                var keyBytes = Convert.FromBase64String(_jwtSettings.SecretKey);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    Subject = new ClaimsIdentity([
                        new Claim(ClaimTypes.NameIdentifier, userId), // id
                        new Claim(ClaimTypes.Role, role)              // role
                    ]),
                    NotBefore = now,
                    IssuedAt = now,
                    Expires = expires,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(keyBytes),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                // crée le JWT
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Token generation failed: " + ex.Message, ex);
            }
        }
    }
}