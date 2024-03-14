// using Microsoft.AspNetCore.Mvc;
// using Inventario.Models;
// using Inventario.Data;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.IdentityModel.Tokens;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;

// namespace Inventario.Controllers
// {
//     [AllowAnonymous]
//     [Route("api/users")]
//     public class UserController : Controller
//     {
//         private readonly ILogger<UserController> _logger;
//         private readonly DataContext _context;
//         private readonly JwtSettings _jwtSettings;

//         public UserController(ILogger<UserController> logger, DataContext context, JwtSettings jwtSettings)
//         {
//             _logger = logger;
//             _context = context;
//             _jwtSettings = jwtSettings;
//         }

//         [HttpPost("authenticate")]
//         public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
//         {
//             var user = await _context.usuarios.SingleOrDefaultAsync(x => x.UserName == model.UserName && x.Password == model.Password);

//             if (user == null)
//             {
//                 return Unauthorized();
//             }

//             var token = GenerateJwtToken(user);

//             return Ok(new AuthenticateResponse { Token = token });
//         }

//         private string GenerateJwtToken(User user)
//         {
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

//             var tokenDescriptor = new SecurityTokenDescriptor
//             {
//                 Subject = new ClaimsIdentity(new[]
//                 {
//                     new Claim(ClaimTypes.Name, user.Id.ToString())
//                 }),
//                 Expires = DateTime.UtcNow.AddMinutes(3),
//                 SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//             };

//             var token = tokenHandler.CreateToken(tokenDescriptor);
//             return tokenHandler.WriteToken(token);
//         }
//         public class JwtSettings
//         {
//             public string Secret { get; set; }
//             public int ExpirationDays { get; set; }
//         }
//         public class AuthenticateRequest
//         {
//             public string UserName { get; set; }
//             public string Password { get; set; }
//         }
//         public class AuthenticateResponse
//         {
//             public string Token { get; set; }
//         }
//     }
// }