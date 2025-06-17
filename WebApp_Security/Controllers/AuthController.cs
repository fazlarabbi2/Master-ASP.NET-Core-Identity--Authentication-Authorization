using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApp_Security.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : Controller
    {
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "admin" && credential.Password == "Password")
            {
                // Creating the security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, credential.UserName),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmployementDate", "2025-02-01")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt
                });
            }

            ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");
            return Unauthorized(ModelState);
        }

        private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
        {
            var secreteKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey") ?? "");

            // Generate the JWT
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secreteKey),
                SecurityAlgorithms.HmacSha256Signature));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

    }
    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
