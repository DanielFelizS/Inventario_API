using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Inventario.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetLogin()
        {
            var loginData = new { message = "Login endpoint" };
            return Ok(loginData);
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid username or password.");
            }

            string username = model.Username;
            string password = model.Password;
            string userRol = model.userRol;

            if (!ValidateUser(userRol))
            {
                return Unauthorized();
            }

            string token = GenerateToken(username, userRol); // Generar el token

            return Ok(new { token });
        }

        private bool ValidateUser(string userRol)
        {
            if (userRol == "admin")
            {
                return true;
            }

            else{
                return false;
            }
            // return true;
        }

        private string GenerateToken(string username, string userRol)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, userRol),
                // Agrega otras claims si es necesario
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JwtSettings:ExpirationHours"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string userRol { get; set; }
    }
}