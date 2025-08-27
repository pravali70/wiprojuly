using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.Users.Select(u => new { u.Id, u.Email }).ToList();
        return Ok(users);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found");

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok($"Role {role} assigned to {user.Email}");
    }
}
