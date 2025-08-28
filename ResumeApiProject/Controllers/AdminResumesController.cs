using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeApi.Models;
using System.Threading.Tasks;

namespace ResumeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // 👈 Only Admins can access this controller
    public class AdminResumesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminResumesController(AppDbContext db)
        {
            _db = db;
        }

        // ✅ Admin: Get all resumes
        [HttpGet]
        public async Task<IActionResult> GetAllResumes()
        {
            var list = await _db.Resumes
                                .Include(r => r.User) // Include User info if needed
                                .ToListAsync();

            return Ok(list);
        }

        // ✅ Admin: Get resume by Id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetResumeById(int id)
        {
            var resume = await _db.Resumes
                                  .Include(r => r.User)
                                  .FirstOrDefaultAsync(r => r.ResumeId == id);

            if (resume == null) return NotFound("Resume not found");

            return Ok(resume);
        }

        // ✅ Admin: Delete any resume
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteResume(int id)
        {
            var resume = await _db.Resumes.FirstOrDefaultAsync(r => r.ResumeId == id);
            if (resume == null) return NotFound("Resume not found");

            _db.Resumes.Remove(resume);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Resume deleted successfully" });
        }
    }
}
