using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sample_app.Models;
using sample_app.Services;
using Microsoft.Extensions.Configuration;

namespace sample_app.Controllers {
    [Route("/api/auth/[action]")]
    public class AuthController : Controller {
        private readonly ToDoContext _context;
        private IConfiguration configuration;

        public AuthController(ToDoContext context, IConfiguration configuration) {
            _context = context;
            this.configuration = configuration;
        }

        class jsonResponse {
            public bool success;
            public string errMsg;
            public string token;
        }

        [HttpPost]
        public async Task<IActionResult> login(string email, string password) {
            // get user with given email and password, return default value for User object if it doesn't exist
            var user = await _context.users.FirstOrDefaultAsync(u => u.email == email && u.passwordHash == AuthService.hashPassword(password, u.passwordSalt, u.passHashIterations, u.passHashSize));
            // equivalent SQL: SELECT * FROM users WHERE email = email AND passwordHash = hashedPassword LIMIT 1;

            if (user != default(User)) {
                // if security config has changed, update salt, hash and security config for user record
                if (user.passHashIterations != Convert.ToInt32(configuration["passHashIterations"]) || user.passHashSize != Convert.ToInt32(configuration["passHashSize"]) || user.passSaltSize != Convert.ToInt32(configuration["passSaltSize"])) {
                    user.passHashIterations = Convert.ToInt32(configuration["passHashIterations"]);
                    user.passHashSize = Convert.ToInt32(configuration["passHashSize"]);
                    user.passSaltSize = Convert.ToInt32(configuration["passSaltSize"]);

                    user.passwordSalt = AuthService.generateSalt(user.passSaltSize);
                    user.passwordHash = AuthService.hashPassword(password, user.passwordSalt, user.passHashIterations, user.passHashSize);
                    // equivalent SQL: UPDATE users SET passHashIterations = config[hashIterations], passHashSize = config[hashSize], passSaltSize = config[saltSize], passwordSalt = newSalt, passwordHash = newHash;

                    _context.SaveChanges();
                }

                var userToken = AuthService.generateJWT(user, configuration["JwtSecretKey"], configuration["JwtExpiry"]); // generate a JWT token for authentication
                return Json(new jsonResponse { success = true, token = userToken });
            } else {
                return Json(new jsonResponse { success = false, errMsg = "Invalid username or password" });
            }
        }

        public async Task<IActionResult> register(string email, string password, string confirmPassword) {
            var user = await _context.users.FirstOrDefaultAsync(u => u.email == email);
            // equivalent SQL: SELECT * FROM users WHERE email = email LIMIT 1;

            if (user != default(User)) {
                return Json(new jsonResponse { success = false, errMsg = "Email already in use" } );
            }

            if (password != confirmPassword) {
                return Json(new jsonResponse { success = false, errMsg = "Passwords don't match" } );            
            }

            User newUser = new User();

            newUser.email = email;
            newUser.passHashIterations = Convert.ToInt32(configuration["passHashIterations"]);
            newUser.passHashSize = Convert.ToInt32(configuration["passHashSize"]);
            newUser.passSaltSize = Convert.ToInt32(configuration["passSaltSize"]);

            newUser.passwordSalt = AuthService.generateSalt(newUser.passSaltSize);
            newUser.passwordHash = AuthService.hashPassword(password, newUser.passwordSalt, newUser.passHashIterations, newUser.passHashSize);

            _context.users.Add(newUser);
            // equivalent SQL: INSERT INTO users VALUES (NULL,email,hashedPassword,passSalt,passSize,passIterations,saltSize,0);
            _context.SaveChanges();

            newUser = await _context.users.FirstOrDefaultAsync(u => u.email == email);

            var userToken = AuthService.generateJWT(newUser, configuration["JwtSecretKey"], configuration["JwtExpiry"]); // get a JWT token for authentication

            return Json(new jsonResponse {success = true, token = userToken}) ;
        }
    }
}