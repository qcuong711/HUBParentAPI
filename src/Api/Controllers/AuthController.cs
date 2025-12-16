using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config) { _config = config; }

    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult Token([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest();
        var issuer = _config["Jwt:Issuer"] ?? "ApiParent";
        var audience = _config["Jwt:Audience"] ?? "ApiParentClients";
        var key = _config["Jwt:Key"] ?? "dev-key-change-me";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("sub", username),
                new System.Security.Claims.Claim("name", username)
            })
        };
        var token = handler.CreateToken(descriptor);
        return Ok(new { token = handler.WriteToken(token) });
    }
}

