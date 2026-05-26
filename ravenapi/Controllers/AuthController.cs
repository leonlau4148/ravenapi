using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BiondEocAPI.DAL.ViewModels;  // ← Import from DAL
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BiondEocAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Login to get JWT token
    /// </summary>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginViewModel login)
    {
        // TODO: Validate against your database using DAL
        // For now, hardcoded example
        if (login.Username == "admin" && login.Password == "password123")
        {
            var token = GenerateJwtToken(login.Username);

            return Ok(new LoginResponseViewModel
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]))
            });
        }

        return Unauthorized(new { message = "Invalid username or password" });
    }

    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(jwtSettings["ExpiryInMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}