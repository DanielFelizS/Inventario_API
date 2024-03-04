using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Inventario.Authorization;

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
            string userRol = model.userRol;

            if (!ValidateUser(userRol))
            {
                return Unauthorized();
            }

            string token = Token.GenerateTokenJwt(username, _configuration["JwtSettings:Secret"]); // Generar el token

            return Ok(new { token });
        }

        private bool ValidateUser(string userRol)
        {
            return !string.IsNullOrEmpty(userRol); // Si el rol no es nulo ni vacío, considerarlo válido
        }

        public class LoginRequestModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string userRol { get; set; }
        }
    }
}