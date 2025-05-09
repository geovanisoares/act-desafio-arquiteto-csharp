using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace act_ms_auth.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Usuário e senha fixos
        private const string FixedUsername = "admin";
        private const string FixedPassword = "123456";

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint para gerar o token JWT
        /// </summary>
        /// <param name="model">Model com usuário e senha</param>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model.Username == FixedUsername && model.Password == FixedPassword)
            {
                var token = GenerateToken(model.Username);
                return Ok(new { token });
            }

            return Unauthorized("Usuário ou senha inválidos.");
        }

        /// <summary>
        /// Endpoint para validar o token JWT
        /// </summary>
        /// <returns>Mensagem de sucesso ou falha</returns>
        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] TokenModel tokenModel)
        {
            try
            {
                var jwtKey = _configuration["JwtSettings:Key"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(tokenModel.Token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = key
                }, out SecurityToken validatedToken);

                var jwtToken = validatedToken as JwtSecurityToken;
                var username = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
                var jti = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                return Ok(new
                {
                    Valid = true,
                    Username = username,
                    Jti = jti,
                    Claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Valid = false, Error = ex.Message });
            }
        }

        private string GenerateToken(string username)
        {
            var jwtKey = _configuration["JwtSettings:Key"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TokenModel
    {
        public string Token { get; set; }
    }
}
