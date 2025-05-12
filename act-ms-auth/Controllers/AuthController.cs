using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;

namespace act_ms_auth.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // User and password fixed
        private const string FixedUsername = "admin";
        private const string FixedPassword = "123456";

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint to generate the JWT token
        /// </summary>
        /// <param name="model">Login model</param>
        /// <returns>JWT token</returns>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Performs login and generates a JWT token")]
        [SwaggerResponse(200, "Token successfully generated")]
        [SwaggerResponse(401, "Invalid username or password")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model.Username == FixedUsername && model.Password == FixedPassword)
            {
                var token = GenerateToken(model.Username);
                return Ok(new { token });
            }

            return Unauthorized("Invalid username or password.");
        }

        /// <summary>
        /// Validates the JWT token
        /// </summary>
        /// <param name="tokenModel">Model containing the JWT token</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate")]
        [SwaggerOperation(Summary = "Validates the JWT token")]
        [SwaggerResponse(200, "Token is valid")]
        [SwaggerResponse(401, "Token is invalid")]
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
        [Required]
        [SwaggerSchema("Username for login")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue("admin")]
        public string? Username { get; set; }

        [Required]
        [SwaggerSchema("Password for login")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue("123456")]
        public string? Password { get; set; }
    }

    public class TokenModel
    {
        [SwaggerSchema("JWT token generated during login")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [DefaultValue("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")]
        public string? Token { get; set; }
    }
}
