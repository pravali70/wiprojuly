using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApi.DTOs;
using ResumeApi.Models;
using ResumeApi.Services;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(AppDbContext db, JwtService jwt, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _jwt = jwt;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Email and Password required" });

            var exists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return BadRequest(new { message = "Email already registered" });

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email, // Identity requires username
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            // Assign default role
            await _userManager.AddToRoleAsync(user, "RegisteredUser");

            return Ok(new { message = "Registered" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var check = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!check)
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _jwt.CreateToken(user.Id, user.Email, user.FullName, "RegisteredUser");
            return Ok(new { token, fullName = user.FullName, role = "RegisteredUser" });
        }
    }
}