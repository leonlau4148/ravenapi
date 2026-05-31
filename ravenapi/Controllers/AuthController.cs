using BiondEocAPI.DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ravenapi.DAL.Data;
using ravenapi.DAL.Entities;
using ravenapi.DAL.ViewModels.Auth;
using ravenapi.Services;

namespace ravenapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(ApplicationDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (await _db.Users.AnyAsync(u => u.Email == model.Email))
            return BadRequest("Email already in use.");

        var user = new User
        {
            Email = model.Email,
            FullName = model.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { token = _tokenService.GenerateToken(user) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        return Ok(new { token = _tokenService.GenerateToken(user) });
    }
}