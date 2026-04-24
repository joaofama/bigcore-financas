using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Financas.Infrastructure.Configurations;
using Financas.Domain.Interfaces.Services;

namespace Financas.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        public TokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

        public string GerarToken(Guid usuarioId, string email, string nome)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, nome)
            }),
                Expires = DateTime.UtcNow.AddHours(_settings.ExpirationInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _settings.Issuer,
                Audience = _settings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}



