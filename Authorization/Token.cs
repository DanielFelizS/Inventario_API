using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Token
{
    public static string GenerateTokenJwt(string username, string secretKey)
    {
        var audienceToken = "http://localhost:5173";
        var issuerToken = "http://localhost:5173";
        var expireTime = "30";

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