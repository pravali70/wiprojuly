using Microsoft.AspNetCore.Mvc;
using ResumeApi.Models; // <-- add this so ApplicationUser is recognized
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

        [HttpGet("test-token")]
        public IActionResult GetTestToken()
        {
            // Dummy test user token (for Swagger testing only)
            var dummyUser = new ApplicationUser
            {
                Id = "test-user-id",     // must match AspNetUsers PK format (string GUID)
                Email = "testuser@test.com",
                FullName = "Test User",
                UserType = "Admin"       // or "RegisteredUser"
            };

            var token = _jwtService.CreateToken(dummyUser);
            return Ok(new { token });
        }
    }
}
