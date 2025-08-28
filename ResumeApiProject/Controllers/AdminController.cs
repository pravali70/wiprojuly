using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApi.DTOs;   // ✅ instead of ResumeApi.Models.DTOs
using ResumeApi.Models; // keep this for ApplicationUser
using ResumeApi.Services;
using System.Threading.Tasks;

namespace ResumeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // ✅ Only admins can access
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // ✅ Get all users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Select(u => new {
                    u.Id,
                    FullName = u.FullName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    UserType = u.UserType ?? "RegisteredUser"
                })
                .ToListAsync();

            return Ok(users);
        }

        // ✅ Get single user by Id
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.UserType
            });
        }

        // ✅ Create new user (Admin can register accounts)
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                UserType = dto.UserType ?? "RegisteredUser"
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User created successfully", user.Id, user.Email });
        }

        // ✅ Update user (name, role, email)
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;
            user.UserName = dto.Email ?? user.UserName;
            user.UserType = dto.UserType ?? user.UserType;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User updated successfully" });
        }

        // ✅ Delete user
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User deleted successfully" });
        }
    }
}
