using Microsoft.AspNetCore.Mvc;
using ResumeApi.Services;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Route("api/Test")]
    public class TestController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public TestController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpGet("admin-token")]
        public IActionResult GetAdminToken()
        {
            // Admin user Id is a string (GUID or nvarchar(450) in AspNetUsers)
            string userId = "admin-id-placeholder"; // Replace with actual Id from DB if needed
            string email = "admin@test.com";
            string fullName = "Admin User";
            string role = "Admin";

            var token = _jwtService.CreateToken(userId, email, fullName, role);
            return Ok(token);
        }
    }
}
