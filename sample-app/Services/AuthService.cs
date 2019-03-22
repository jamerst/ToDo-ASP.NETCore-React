using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace sample_app.Services {
    public class AuthService {
        private IConfiguration configuration;

        public AuthService(IConfiguration configuration) {
            this.configuration = configuration;
        }

        public static int getCurrentUser(IHttpContextAccessor context) {
            var stringId = context?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            int.TryParse(stringId ?? "0", out int userId);

            return userId;
        }

        public string hashPassword(string password, string salt, int iterations, int size) {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: iterations,
                numBytesRequested: size / 8));
        }

        public string generateSalt(int size) {
            byte[] salt = new byte[size / 8];
            using (var RNG = RandomNumberGenerator.Create()) {
                RNG.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        public string generateJWT(int id) {
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Jti, id.ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(Convert.ToInt32(configuration["JwtExpiry"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecretKey"])), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}