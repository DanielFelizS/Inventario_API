using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Inventario.Authorization
{
public class Token
{
    public static string GenerateTokenJwt(string username, string secretKey)
    {
        var audienceToken = "http://localhost:5198/"; // URL de tu aplicación en producción
        var issuerToken = "http://localhost:5173/"; // URL de tu aplicación en producción
        var expireTime = "60"; // Token válido por 1 hora

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) });

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
            audience: audienceToken,
            issuer: issuerToken,
            subject: claimsIdentity,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
            signingCredentials: signingCredentials);

        var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
        return jwtTokenString;
    }

}
}