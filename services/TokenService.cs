// jwt token generation service for user authentication and authorization.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Commute360.Models;

namespace Commute360.Services;

public class TokenService
{
    //readonly so that the configuration cannot be modified after initialization. This is important for security and consistency, as the token generation process relies on specific configuration values that should not change during the lifetime of the application.
    private readonly IConfiguration _config;
//// ASP.NET Core automatically passes in the application's configuration instance when TokenService is initialized.
    public TokenService(IConfiguration config)
    {
        _config = config;
    }
//claim is created for each user property that needs to be included in the token. These claims are then added to the token's payload, allowing the application to access this information when validating the token and authorizing user actions.
    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
//The key is created using a secret key from the application's configuration. This key is used to sign the token, ensuring its integrity and authenticity. The signing credentials specify the algorithm used for signing the token, in this case, HMAC-SHA256.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//The token is created with the specified issuer, audience, claims, expiration time, and signing credentials. The expiration time is set to 8 hours from the current UTC time, after which the token will no longer be valid.
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );
//Finally, the token is serialized into a string format using JwtSecurityTokenHandler and returned to the caller. This string can be sent to the client for use in subsequent requests, allowing the client to authenticate and authorize actions based on the user's claims.
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}