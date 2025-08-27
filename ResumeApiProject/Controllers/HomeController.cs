using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        // Guest users (unauthenticated) can access
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome Guest! Please register or login to create resumes.");
        }
    }
}
